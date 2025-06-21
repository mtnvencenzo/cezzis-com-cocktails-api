namespace Cocktails.Api.Application.Concerns.Accounts.Models;

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

#pragma warning disable format

[type: Description("A cocktail reference with management action within an owned account faviorites list")]
public record CocktailFavoriteActionModel
(
    [property: Required()]
    [property: Description("The cocktail identifier")]
    string CocktailId,

    [property: Required()]
    [property: Description("The action to take on the cocktail within the favorites list")]
    CocktailFavoritingActionModel Action
);