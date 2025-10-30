namespace Cocktails.Api.Infrastructure.Services;

using Cocktails.Api.Domain.Aggregates.CocktailAggregate;

/// <summary>Defines a contract for publishing cocktails in batches.</summary>
public interface ICocktailPublisher
{
    /// <summary>Publishes the next batch of cocktails.</summary>
    /// <param name="cocktails">The cocktails to publish.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    Task PublishNextBatchAsync(
        List<Cocktail> cocktails,
        CancellationToken cancellationToken = default);
}