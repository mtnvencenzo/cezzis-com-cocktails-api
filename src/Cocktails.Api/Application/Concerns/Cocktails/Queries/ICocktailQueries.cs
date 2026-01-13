namespace Cocktails.Api.Application.Concerns.Cocktails.Queries;

using global::Cocktails.Api.Application.Concerns.Cocktails.Models;

public interface ICocktailQueries
{
    Task<CocktailRs> GetCocktail(
        string id,
        CancellationToken cancellationToken = default);

    Task<string> GetCocktailsSiteMap(CancellationToken cancellationToken = default);

    Task<CocktailIngredientFiltersRs> GetCocktailIngredientFilters(CancellationToken cancellationToken = default);

    Task<CocktailIndexNowRs> GetCocktailsIndexNowResult(CancellationToken cancellationToken = default);
}
