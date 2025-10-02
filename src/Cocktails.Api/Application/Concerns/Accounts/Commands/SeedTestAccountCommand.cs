namespace Cocktails.Api.Application.Concerns.Accounts.Commands;

using MediatR;
using global::Cocktails.Api.Domain.Config;
using Microsoft.Extensions.Options;
using global::Cocktails.Api.Application.Concerns.Accounts.Models;
using System.Security.Claims;
using global::Cocktails.Api.Application.Concerns.Accounts.Queries;
using global::Cocktails.Api.Infrastructure.Resources.Test;
using global::Cocktails.Api.Domain.Aggregates.AccountAggregate;

public record SeedTestAccountCommand() : IRequest<bool>;

public class SeedTestAccountCommandHandler(
    IMediator mediator,
    IAccountsQueries accountQueries,
    IOptions<TestAccountConfig> testAccountConfig,
    IAccountsQueries accountsQueries) : IRequestHandler<SeedTestAccountCommand, bool>
{
    public async Task<bool> Handle(SeedTestAccountCommand command, CancellationToken cancellationToken)
    {
        var identity = new ClaimsIdentity([
            new (ClaimTypes.NameIdentifier, testAccountConfig.Value.SubjectId),
            new (ClaimTypes.Name, testAccountConfig.Value.SubjectId),
            new (ClaimsAccount.ClaimType_GivenName, testAccountConfig.Value.GivenName),
            new (ClaimsAccount.ClaimType_FamilyName, testAccountConfig.Value.FamilyName),
            new (ClaimsAccount.ClaimType_DisplayName, testAccountConfig.Value.DisplayName),
            new (ClaimsAccount.ClaimType_Email, testAccountConfig.Value.LoginEmail ?? testAccountConfig.Value.LoginEmail)
        ]);

        await accountQueries.GetAccountOwnedProfile(
            claimsIdentity: identity,
            createIfNotExists: true,
            cancellationToken: cancellationToken);

        var updateAccountProfileCommand = new UpdateAccountOwnedProfileCommand(
            Request: new UpdateAccountOwnedProfileRq(
                GivenName: testAccountConfig.Value.GivenName,
                FamilyName: testAccountConfig.Value.FamilyName,
                DisplayName: testAccountConfig.Value.DisplayName,
                PrimaryAddress: new AccountAddressModel(
                    AddressLine1: testAccountConfig.Value.AddressLine1,
                    AddressLine2: "",
                    City: testAccountConfig.Value.City,
                    Region: testAccountConfig.Value.Region,
                    SubRegion: "",
                    PostalCode: testAccountConfig.Value.PostalCode,
                    Country: testAccountConfig.Value.Country)),
            UpdateIdentityProvider: false,
            Identity: identity);

        var _ = await mediator.Send(updateAccountProfileCommand, cancellationToken)
            ?? throw new InvalidOperationException("Failed to update test account profile.");

        // ------------------------------------------------------------------
        // Change the email address back to the original test account email
        // ------------------------------------------------------------------
        var updateEmailCommand = new ChangeAccountOwnedEmailCommand(
            Request: new ChangeAccountOwnedEmailRq(Email: testAccountConfig.Value.LoginEmail),
            UpdateIdentityProvider: false,
            Identity: identity);

        _ = await mediator.Send(updateEmailCommand, cancellationToken)
            ?? throw new InvalidOperationException("Failed to update test account email.");

        // ------------------------------------------------------------------
        // Change the email address back to the original test account email
        // ------------------------------------------------------------------
        var updateNotificationsCommand = new UpdateAccountOwnedNotificationSettingsCommand(
            Request: new UpdateAccountOwnedNotificationSettingsRq(OnNewCocktailAdditions: CocktailUpdatedNotificationModel.Always),
            Identity: identity);

        _ = await mediator.Send(updateNotificationsCommand, cancellationToken)
            ?? throw new InvalidOperationException("Failed to update test account notification settings.");

        // ------------------------------------------------------------------
        // Change the email address back to the original test account email
        // ------------------------------------------------------------------
        var updateAccessibilityCommand = new UpdateAccountOwnedAccessibilitySettingsCommand(
            Request: new UpdateAccountOwnedAccessibilitySettingsRq(Theme: DisplayThemeModel.Light),
            Identity: identity);

        _ = await mediator.Send(updateAccessibilityCommand, cancellationToken)
            ?? throw new InvalidOperationException("Failed to update test account accessibility settings.");

        // ------------------------------------------------------------------
        // If the account has favorite cocktails, start fresh and remove them
        // ------------------------------------------------------------------
        var (profile, _) = await accountsQueries.GetAccountOwnedProfile(identity, false, cancellationToken);

        var favorites = profile.FavoriteCocktails ?? [];
        if (favorites.Count > 0)
        {
            var manageFavoriteCocktailsCommand = new ManageFavoriteCocktailsCommand(
                Request: new ManageFavoriteCocktailsRq(CocktailActions: favorites.Select(c => new CocktailFavoriteActionModel(CocktailId: c, Action: CocktailFavoritingActionModel.Remove)).ToList()),
                Identity: identity);

            _ = await mediator.Send(manageFavoriteCocktailsCommand, cancellationToken)
                ?? throw new InvalidOperationException("Failed to remove existing favorite cocktails from test account.");
        }

        // ------------------------------------------------------------------
        // Remove all cocktail ratings from the test account
        // ------------------------------------------------------------------
        var ratings = (await accountsQueries.GetAccountOwnedCocktailRatings(identity, cancellationToken)).Ratings ?? [];
        if (ratings.Count > 0)
        {
            foreach (var rating in ratings)
            {
                var unRateCocktailCommand = new UnRateCocktailCommand(
                    Identity: identity,
                    CocktailId: rating.CocktailId);

                var removed = await mediator.Send(unRateCocktailCommand, cancellationToken);
                if (!removed)
                {
                    throw new InvalidOperationException($"Failed to remove rating for cocktail {rating.CocktailId} from test account.");
                }
            }
        }

        var profileImageCommand = new ProfileImageUploadCommand(new CypressUser(), identity);
        var result = await mediator.Send(profileImageCommand, cancellationToken);

        if (string.IsNullOrWhiteSpace(result.ImageUri))
        {
            throw new InvalidOperationException($"Failed to upload profile image for test account.");
        }

        return true;
    }
}