namespace Cocktails.Api.Application.Concerns.Cocktails.Commands;

using global::Cocktails.Api.Domain.Aggregates.CocktailAggregate;
using global::Cocktails.Api.Infrastructure.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

public record PublishCocktailsCommand(int BatchItemCount = 1) : IRequest<bool>;

public class PublishCocktailsCommandHandler(
    ICocktailRepository cocktailRepository,
    ICocktailPublisher cocktailPublisher,
    ILogger<PublishCocktailsCommandHandler> logger) : IRequestHandler<PublishCocktailsCommand, bool>
{
    public async Task<bool> Handle(PublishCocktailsCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("Publishing cocktails with batch size of {BatchItemCount}", command.BatchItemCount);

        var offset = 0;
        var cocktails = await cocktailRepository.Items
            .Skip(offset)
            .Take(command.BatchItemCount)
            .ToListAsync(cancellationToken);

        while (cocktails.Count > 0)
        {
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
        }

        logger.LogInformation("Finished publishing cocktails.");

        return true;
    }
}