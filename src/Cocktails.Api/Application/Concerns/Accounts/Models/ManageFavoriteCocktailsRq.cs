namespace Cocktails.Api.Application.Concerns.Accounts.Models;

using System.ComponentModel;

#pragma warning disable format

[type: Description("The request to manage an owned account's favorite cocktails")]
public record ManageFavoriteCocktailsRq
(
    IList<CocktailFavoriteActionModel> CocktailActions
);