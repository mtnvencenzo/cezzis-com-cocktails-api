namespace Cocktails.Api.Application.Concerns.Accounts.Queries;

using global::Cocktails.Api.Application.Concerns.Accounts.Models;
using System.Security.Claims;

public interface IAccountsQueries
{
    Task<AccountOwnedProfileRs> GetAccountOwnedProfile(ClaimsIdentity claimsIdentity, CancellationToken cancellationToken);

    Task<AccountCocktailRatingsRs> GetAccountOwnedCocktailRatings(ClaimsIdentity claimsIdentity, CancellationToken cancellationToken);
}