namespace Cocktails.Api.Application.Concerns.Cocktails.Commands;

using global::Cocktails.Api.Domain.Aggregates.CocktailAggregate;
using global::Cocktails.Api.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

public record SeedCocktailsCommand(bool OnlyIfEmpty = false) : IRequest<bool>;

public class SeedCocktailsCommandHandler(
    ICocktailRepository cocktailRepository,
    CocktailDataStore cocktailsDataStore,
    IMediator mediator,
    ILogger<SeedCocktailsCommandHandler> logger) : IRequestHandler<SeedCocktailsCommand, bool>
{
    public async Task<bool> Handle(SeedCocktailsCommand command, CancellationToken cancellationToken)
    {
        var availableCocktails = cocktailsDataStore.Cocktails;
        var modifiedCocktails = new List<string>();
        var any = (await cocktailRepository.Items.FirstOrDefaultAsync(cancellationToken)) != null;

        if (command.OnlyIfEmpty && any)
        {
            return false;
        }

        foreach (var cocktail in availableCocktails)
        {
            var existing = await cocktailRepository.Items.FirstOrDefaultAsync(x => x.Id == cocktail.Id, cancellationToken);

            if (existing == null)
            {
                logger.LogInformation("Adding cocktail {cocktail_id}", cocktail.Id);
                cocktailRepository.Add(cocktail);
                modifiedCocktails.Add(cocktail.Id);
            }
            else
            {
                if (existing.IsSameAs(cocktail) == false)
                {
                    logger.LogInformation("Updating cocktail {cocktail_id}", cocktail.Id);
                    existing.MergeUpdate(cocktail);
                    modifiedCocktails.Add(cocktail.Id);
                }
                else
                {
                    logger.LogInformation("No update needed for cocktail {cocktail_id}", cocktail.Id);
                }
            }
        }

        if (modifiedCocktails.Count > 0)
        {
            await cocktailRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
            cocktailRepository.ClearCache();

            // If the update occured successfully, republish the cocktail
            // to the event broker for external system integrations (re-embed the cocktail)
            var commandResult = await mediator.Send(
                request: new PublishCocktailsCommand(
                    BatchItemCount: 1,
                    CocktailIds: [.. modifiedCocktails]
                ),
                cancellationToken: cancellationToken);

            if (!commandResult)
            {
                logger.LogError("Failed to publish cocktails batch");
                return false;
            }
        }

        return true;
    }
}