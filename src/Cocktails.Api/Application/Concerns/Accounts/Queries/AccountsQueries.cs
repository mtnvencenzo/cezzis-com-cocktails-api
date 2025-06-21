namespace Cocktails.Api.Application.Concerns.Accounts.Queries;

using Cezzi.Applications;
using global::Cocktails.Api.Application.Concerns.Accounts.Models;
using global::Cocktails.Api.Domain.Aggregates.AccountAggregate;
using global::Cocktails.Api.Domain.Aggregates.CocktailAggregate;
using global::Cocktails.Api.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

public class AccountsQueries(IAccountRepository accountRepository, IAccountCocktailRatingsRepository accountCocktailRatingsRepository) : IAccountsQueries
{
    public async Task<AccountOwnedProfileRs> GetAccountOwnedProfile(HttpContext httpContext)
    {
        var account = await accountRepository.GetOrCreateLocalAccountFromIdentity(
            claimsIdentity: httpContext.User?.Identity as ClaimsIdentity,
            cancellationToken: httpContext.RequestAborted);

        var rs = new AccountOwnedProfileRs(
            SubjectId: account.SubjectId,
            GivenName: account.GivenName,
            FamilyName: account.FamilyName,
            Email: account.Email,
            LoginEmail: account.LoginEmail ?? account.Email,
            AvatarUri: account.AvatarUri,
            DisplayName: account.DisplayName,
            PrimaryAddress: account.PrimaryAddress != null
                ? new AccountAddressModel(
                    AddressLine1: account.PrimaryAddress.AddressLine1 ?? string.Empty,
                    AddressLine2: account.PrimaryAddress.AddressLine2 ?? string.Empty,
                    City: account.PrimaryAddress.City ?? string.Empty,
                    Region: account.PrimaryAddress.Region ?? string.Empty,
                    SubRegion: account.PrimaryAddress.SubRegion ?? string.Empty,
                    PostalCode: account.PrimaryAddress.PostalCode ?? string.Empty,
                    Country: account.PrimaryAddress.Country ?? string.Empty)
                : null,
            Accessibility: account.Accessibility != null
                ? new AccountAccessibilitySettingsModel(Theme: (DisplayThemeModel)account.Accessibility.Theme)
                : null,
            FavoriteCocktails: account.FavoriteCocktails ?? []);

        return rs;
    }

    public async Task<AccountCocktailRatingsRs> GetAccountOwnedCocktailRatings(HttpContext httpContext)
    {
        var account = await accountRepository.GetOrCreateLocalAccountFromIdentity(
            claimsIdentity: httpContext.User?.Identity as ClaimsIdentity,
            cancellationToken: httpContext.RequestAborted);

        Guard.NotNull(account, nameof(account));

        var ratings = await accountCocktailRatingsRepository.Items
            .WithPartitionKey(account.SubjectId)
            .Where(x => x.Id == account.RatingsId)
            .FirstOrDefaultAsync(httpContext.RequestAborted);

        return new AccountCocktailRatingsRs(
            Ratings: ratings != null
                ? [.. ratings.Ratings.Select(x => new AccountCocktailRatingsModel(CocktailId: x.CocktailId, Stars: x.Stars))]
                : []);
    }
}