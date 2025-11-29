namespace Cocktails.Api.Application.Concerns.Accounts.Queries;

using global::Cocktails.Api.Application.Concerns.Accounts.Models;
using global::Cocktails.Api.Domain.Aggregates.AccountAggregate;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

public class AccountsQueries(IAccountRepository accountRepository, IAccountCocktailRatingsRepository accountCocktailRatingsRepository) : IAccountsQueries
{
    public async Task<(AccountOwnedProfileRs profile, bool created)> GetAccountOwnedProfile(ClaimsIdentity claimsIdentity, bool createIfNotExists, CancellationToken cancellationToken)
    {
        if (createIfNotExists)
        {
            var (account, created) = await accountRepository.GetOrCreateLocalAccountFromIdentity(
                claimsIdentity: claimsIdentity,
                cancellationToken: cancellationToken);

            return (AccountOwnedProfileRs.FromAccount(account), created);
        }
        else
        {
            var account = await accountRepository.GetLocalAccountFromIdentity(
                claimsIdentity: claimsIdentity,
                cancellationToken: cancellationToken);

            if (account != null)
            {
                return (AccountOwnedProfileRs.FromAccount(account), false);
            }
        }

        return (null, false);
    }

    public async Task<AccountCocktailRatingsRs> GetAccountOwnedCocktailRatings(ClaimsIdentity claimsIdentity, CancellationToken cancellationToken)
    {
        var account = await accountRepository.GetLocalAccountFromIdentity(
            claimsIdentity: claimsIdentity,
            cancellationToken: cancellationToken);

        if (account == null)
        {
            throw new ArgumentNullException(nameof(account), "Failed to get account from identity.");
        }

        var ratings = await accountCocktailRatingsRepository.Items
            .WithPartitionKey(account.SubjectId)
            .Where(x => x.Id == account.RatingsId)
            .FirstOrDefaultAsync(cancellationToken);

        return new AccountCocktailRatingsRs(
            Ratings: ratings != null
                ? [.. ratings.Ratings.Select(x => new AccountCocktailRatingsModel(CocktailId: x.CocktailId, Stars: x.Stars))]
                : []);
    }
}