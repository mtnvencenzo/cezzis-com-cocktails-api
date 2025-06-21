namespace Cocktails.Api.Application.Concerns.Accounts.Queries;

using global::Cocktails.Api.Application.Concerns.Accounts.Models;

public interface IAccountsQueries
{
    Task<AccountOwnedProfileRs> GetAccountOwnedProfile(HttpContext httpContext);

    Task<AccountCocktailRatingsRs> GetAccountOwnedCocktailRatings(HttpContext httpContext);
}
