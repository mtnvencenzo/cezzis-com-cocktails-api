namespace Cocktails.Api.Application.Concerns.Accounts.Models;

using Microsoft.Identity.Client;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

#pragma warning disable format

[type: Description("The request to manage an owned account's favorite cocktails")]
public record ManageFavoriteCocktailsRq
(
    IList<CocktailFavoriteActionModel> CocktailActions
);