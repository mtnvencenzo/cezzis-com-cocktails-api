namespace Cocktails.Api.Application.Concerns.Accounts.Commands;

using Cezzi.Applications;
using global::Cocktails.Api.Application.Concerns.Accounts.Models;
using global::Cocktails.Api.Domain;
using global::Cocktails.Api.Domain.Aggregates.AccountAggregate;
using global::Cocktails.Api.Domain.Config;
using global::Cocktails.Api.Domain.Services;
using global::Cocktails.Common;
using MediatR;
using Microsoft.Extensions.Options;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

public record ManageFavoriteCocktailsCommand
(
    ManageFavoriteCocktailsRq Request,

    ClaimsIdentity Identity

) : IRequest<AccountOwnedProfileRs>;

public class ManageFavoriteCocktailsCommandHandler(IAccountRepository accountRepository)
    : IRequestHandler<ManageFavoriteCocktailsCommand, AccountOwnedProfileRs>
{
    public async Task<AccountOwnedProfileRs> Handle(ManageFavoriteCocktailsCommand command, CancellationToken cancellationToken)
    {
        Guard.NotNull(command, nameof(command));
        Guard.NotNull(command.Request, nameof(command.Request));

        var account = await accountRepository.GetOrCreateLocalAccountFromIdentity(
            claimsIdentity: command.Identity,
            cancellationToken: cancellationToken);

        Guard.NotNull(account);

        if (command.Request.CocktailActions != null && command.Request.CocktailActions.Count > 0)
        {
            account.ManageFavoriteCocktails(
                add: [.. command.Request.CocktailActions
                    .Where(x => x.Action == CocktailFavoritingActionModel.Add)
                    .Select(x => x.CocktailId)],
                remove: [.. command.Request.CocktailActions
                    .Where(x => x.Action == CocktailFavoritingActionModel.Remove)
                    .Select(x => x.CocktailId)]);

            accountRepository.Update(account);

            _ = await accountRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }

        return new AccountOwnedProfileRs(
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
    }
}
