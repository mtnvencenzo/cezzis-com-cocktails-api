namespace Cocktails.Api.Application.Concerns.Accounts.Commands;

using MediatR;
using global::Cocktails.Api.Domain.Config;
using Microsoft.Extensions.Options;
using global::Cocktails.Api.Application.Concerns.Accounts.Models;
using System.Security.Claims;
using global::Cocktails.Api.Application.Concerns.Accounts.Queries;

public record SeedTestAccountCommand() : IRequest<bool>;

public class SeedTestAccountCommandHandler(
    IMediator mediator,
    IOptions<TestAccountConfig> testAccountConfig,
    IAccountsQueries accountsQueries) : IRequestHandler<SeedTestAccountCommand, bool>
{
    public async Task<bool> Handle(SeedTestAccountCommand command, CancellationToken cancellationToken)
    {
        var identity = new ClaimsIdentity([
            new (ClaimTypes.NameIdentifier, testAccountConfig.Value.SubjectId),
            new (ClaimTypes.Name, testAccountConfig.Value.DisplayName),
            new (ClaimTypes.GivenName, testAccountConfig.Value.GivenName),
            new (ClaimTypes.Surname, testAccountConfig.Value.FamilyName),
            new ("emails", testAccountConfig.Value.LoginEmail ?? testAccountConfig.Value.LoginEmail)
        ]);

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
            Identity: identity);

        var _ = await mediator.Send(updateAccountProfileCommand, cancellationToken)
            ?? throw new InvalidOperationException("Failed to update test account profile.");

        var updateEmailCommand = new UpdateAccountOwnedProfileEmailCommand(
            Request: new UpdateAccountOwnedProfileEmailRq(Email: testAccountConfig.Value.LoginEmail),
            Identity: identity);

        _ = await mediator.Send(updateEmailCommand, cancellationToken)
            ?? throw new InvalidOperationException("Failed to update test account email.");

        // ------------------------------------------------------------------
        // If the account has favorite cocktails, start fresh and remove them
        // ------------------------------------------------------------------
        var favorites = (await accountsQueries.GetAccountOwnedProfile(identity, cancellationToken)).FavoriteCocktails ?? [];
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

        return true;
    }
}