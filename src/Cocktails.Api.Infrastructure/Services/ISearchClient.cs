namespace Cocktails.Api.Infrastructure.Services;

using Cocktails.Api.Domain.Aggregates.CocktailAggregate;

public interface ISearchClient
{
    /// <summary>Searches cocktails based on the provided query.</summary>
    /// <param name="cocktails"></param>
    /// <param name="query"></param>
    /// <param name="skip"></param>
    /// <param name="take"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<List<Cocktail>> SearchAsync(
        List<Cocktail> cocktails,
        string query,
        int skip = 0,
        int take = 20,
        CancellationToken cancellationToken = default);
}
