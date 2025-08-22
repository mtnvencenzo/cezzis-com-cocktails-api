namespace Cocktails.Api.Application.Concerns.Accounts.Queries;

using global::Cocktails.Api.Application.Concerns.Accounts.Models;
using System.Security.Claims;

public interface IAccountsQueries
{
    Task<(AccountOwnedProfileRs profile, bool created)> GetAccountOwnedProfile(ClaimsIdentity claimsIdentity, bool createIfNotExists, CancellationToken cancellationToken);

    Task<AccountCocktailRatingsRs> GetAccountOwnedCocktailRatings(ClaimsIdentity claimsIdentity, CancellationToken cancellationToken);
}