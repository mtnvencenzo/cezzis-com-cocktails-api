namespace Cocktails.Api.Application.Concerns.Accounts.Models;

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

#pragma warning disable format

[type: Description("The cocktail ratings associated with an account")]
public record AccountCocktailRatingsRs
(
    [property: Required()]
    [property: Description("The cocktail ratings")]
    List<AccountCocktailRatingsModel> Ratings
);