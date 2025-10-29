namespace Cocktails.Api.Application.Concerns.Cocktails.Commands;

using global::Cocktails.Api.Domain.Aggregates.CocktailAggregate;
using global::Cocktails.Api.Infrastructure.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

public record PublishCocktailsCommand(int BatchItemCount = 1, string[] CocktailIds = null) : IRequest<bool>;

public class PublishCocktailsCommandHandler(
    ICocktailRepository cocktailRepository,
    ICocktailPublisher cocktailPublisher,
    ILogger<PublishCocktailsCommandHandler> logger) : IRequestHandler<PublishCocktailsCommand, bool>
{
    public async Task<bool> Handle(PublishCocktailsCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("Publishing cocktails with batch size of {BatchItemCount}", command.BatchItemCount);

        var offset = 0;
        List<Cocktail> cocktails;

        var cocktailIds = command.CocktailIds != null && command.CocktailIds.Length > 0
            ? command.CocktailIds
            : [];

        do
        {
            cocktails = [.. cocktailRepository.CachedItems
                .Where(c => cocktailIds.Length == 0 || cocktailIds.Contains(c.Id))
                .Skip(offset)
                .Take(command.BatchItemCount)];

            if (cocktails.Count == 0)
            {
                break;
            }

            try
            {
                await cocktailPublisher.PublishNextBatchAsync(cocktails, cancellationToken);
                logger.LogInformation("Published batch of {Count} cocktails", cocktails.Count);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to publish batch at offset {Offset}", offset);
                throw;
            }

            offset += command.BatchItemCount;

        } while (cocktails.Count > 0);

        logger.LogInformation("Finished publishing cocktails.");

        return true;
    }
}