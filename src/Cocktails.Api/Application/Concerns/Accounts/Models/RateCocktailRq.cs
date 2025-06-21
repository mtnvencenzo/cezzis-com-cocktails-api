namespace Cocktails.Api.Application.Concerns.Accounts.Models;

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

#pragma warning disable format

[type: Description("A cocktail reference with user supplied rating")]
public record RateCocktailRq
(
    [property: Required()]
    [property: Description("The cocktail identifier")]
    string CocktailId,

    [property: Required()]
    [property: Description("The rating for the cocktail (1-5)")]
    int Stars
);