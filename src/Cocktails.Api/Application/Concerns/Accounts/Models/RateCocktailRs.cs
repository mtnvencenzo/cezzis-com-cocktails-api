namespace Cocktails.Api.Application.Concerns.Accounts.Models;

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

#pragma warning disable format

[type: Description("The cocktail rating response from an account supplied cocktail rating")]
public record RateCocktailRs
(
    [property: Required()]
    [property: Description("The cocktail ratings")]
    List<AccountCocktailRatingsModel> Ratings,

    [property: Required()]
    [property: Description("The cocktail identifier")]
    string CocktailId,

    [property: Required()]
    [property: Description("The cocktail rating values")]
    AccountCocktailRatingModel CocktailRating
);