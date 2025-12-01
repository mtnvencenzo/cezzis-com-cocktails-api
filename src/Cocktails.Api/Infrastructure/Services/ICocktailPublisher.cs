namespace Cocktails.Api.Infrastructure.Services;

using Cocktails.Api.Application.Concerns.Cocktails.Models;

/// <summary>Defines a contract for publishing cocktails in batches.</summary>
public interface ICocktailPublisher
{
    /// <summary>Publishes the next batch of cocktails.</summary>
    /// <param name="cocktails">The cocktails to publish.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    Task PublishNextBatchAsync(
        List<CocktailModel> cocktails,
        CancellationToken cancellationToken = default);
}