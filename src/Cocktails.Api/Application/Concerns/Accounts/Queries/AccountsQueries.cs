namespace Cocktails.Api.Application.Concerns.Accounts.Queries;

using Cezzi.Applications;
using global::Cocktails.Api.Application.Concerns.Accounts.Models;
using global::Cocktails.Api.Domain.Aggregates.AccountAggregate;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

public class AccountsQueries(IAccountRepository accountRepository, IAccountCocktailRatingsRepository accountCocktailRatingsRepository) : IAccountsQueries
{
    public async Task<AccountOwnedProfileRs> GetAccountOwnedProfile(ClaimsIdentity claimsIdentity, CancellationToken cancellationToken)
    {
        var account = await accountRepository.GetOrCreateLocalAccountFromIdentity(
            claimsIdentity: claimsIdentity,
            cancellationToken: cancellationToken);

        return AccountOwnedProfileRs.FromAccount(account);
    }

    public async Task<AccountCocktailRatingsRs> GetAccountOwnedCocktailRatings(ClaimsIdentity claimsIdentity, CancellationToken cancellationToken)
    {
        var account = await accountRepository.GetOrCreateLocalAccountFromIdentity(
            claimsIdentity: claimsIdentity,
            cancellationToken: cancellationToken);

        Guard.NotNull(account, nameof(account));

        var ratings = await (accountCocktailRatingsRepository.Items
            .WithPartitionKey(account.SubjectId)
            .Where(x => x.Id == account.RatingsId)
            .FirstOrDefaultAsync(cancellationToken));

        return new AccountCocktailRatingsRs(
            Ratings: ratings != null
                ? [.. ratings.Ratings.Select(x => new AccountCocktailRatingsModel(CocktailId: x.CocktailId, Stars: x.Stars))]
                : []);
    }
}