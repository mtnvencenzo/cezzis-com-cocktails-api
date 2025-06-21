namespace Cocktails.Api.Application.Concerns.Cocktails.Queries;

using global::Cocktails.Api.Application.Concerns.Cocktails.Models;

public interface ICocktailQueries
{
    Task<CocktailRs> GetCocktail(
        string id,
        CancellationToken cancellationToken = default);

    Task<CocktailsListRs> GetCocktailsList(
        string freeText,
        int skip = 0,
        int take = 20,
        bool allowExcessiveTake = false,
        List<string> filters = null,
        CocktailDataIncludeModel[] include = null,
        string[] matches = null,
        bool matchExclusive = false,
        bool useSearchIndex = false,
        CancellationToken cancellationToken = default);

    Task<string> GetCocktailsSiteMap(CancellationToken cancellationToken = default);

    Task<CocktailIngredientFiltersRs> GetCocktailIngredientFilters(CancellationToken cancellationToken = default);

    Task<CocktailIndexNowRs> GetCocktailsIndexNowResult(CancellationToken cancellationToken = default);
}
