namespace Cocktails.Api.Application.Concerns.Cocktails.Commands;

using global::Cocktails.Api.Domain.Aggregates.CocktailAggregate;
using MediatR;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Text;
using Microsoft.EntityFrameworkCore;
using global::Cocktails.Api.Infrastructure;

public record SeedCocktailsCommand(bool OnlyIfEmpty = false) : IRequest<bool>;

public class SeedCocktailsCommandHandler(
    ICocktailRepository cocktailRepository,
    CocktailDataStore cocktailsDataStore,
    IMediator mediator,
    ILogger<SeedCocktailsCommandHandler> logger) : IRequestHandler<SeedCocktailsCommand, bool>
{
    private readonly static JsonSerializerOptions jsonSerializerOptions;

    static SeedCocktailsCommandHandler()
    {
        jsonSerializerOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            AllowTrailingCommas = true
        };

        jsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
    }

    public async Task<bool> Handle(SeedCocktailsCommand command, CancellationToken cancellationToken)
    {
        var availableCocktails = cocktailsDataStore.Cocktails;
        var hasChanges = false;
        var any = (await cocktailRepository.Items.FirstOrDefaultAsync(cancellationToken)) != null;

        if (command.OnlyIfEmpty && any)
        {
            return false;
        }

        var modifiedCocktails = new List<string>();

        foreach (var cocktail in availableCocktails)
        {
            var existing = await cocktailRepository.Items.FirstOrDefaultAsync(x => x.Id == cocktail.Id, cancellationToken);

            if (existing == null)
            {
                logger.LogInformation("Adding cocktail {CocktailId}", cocktail.Id);
                cocktailRepository.Add(cocktail);
                modifiedCocktails.Add(cocktail.Id);
                hasChanges = true;
            }
            else
            {
                if (existing.Hash != cocktail.RegenerateHash())
                {
                    logger.LogInformation("Updating cocktail {CocktailId}", cocktail.Id);
                    existing.MergeUpdate(cocktail);
                    modifiedCocktails.Add(cocktail.Id);
                    hasChanges = true;
                }
            }
        }

        if (hasChanges)
        {
            await cocktailRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
            cocktailRepository.ClearCache();
        }

        if (modifiedCocktails.Count > 0)
        {
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