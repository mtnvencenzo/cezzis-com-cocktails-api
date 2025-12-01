namespace Cocktails.Api.Infrastructure.Services;

using Cocktails.Api.Domain.Aggregates.CocktailAggregate;

public interface ISearchClient
{
    /// <summary>Searches cocktails based on the provided query.</summary>
    /// <param name="cocktails">The list of cocktails to search through.</param>
    /// <param name="query">The search query string.</param>
    /// <param name="skip">The number of results to skip for pagination.</param>
    /// <param name="take">The maximum number of results to return.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of cocktails matching the search query.</returns>
    Task<List<Cocktail>> SearchAsync(
        List<Cocktail> cocktails,
        string query,
        int skip = 0,
        int take = 20,
        CancellationToken cancellationToken = default);
}
