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
        logger.LogInformation("Publishing cocktails in {BatchItemCount} batches", command.BatchItemCount);

        var offset = 0;
        var cocktails = await cocktailRepository.Items
            .Skip(offset)
            .Take(command.BatchItemCount)
            .ToListAsync(cancellationToken);

        while (cocktails.Count > 0)
        {
            await cocktailPublisher.PublishNextBatchAsync(cocktails, cancellationToken);
            offset += command.BatchItemCount;

            cocktails = await cocktailRepository.Items
                .Skip(offset)
                .Take(command.BatchItemCount)
                .ToListAsync(cancellationToken);
        }

        logger.LogInformation("Finished publishing cocktails.");

        return true;
    }
}