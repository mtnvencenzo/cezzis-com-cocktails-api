namespace Cocktails.Api.Application.Concerns.Accounts.Models;

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

#pragma warning disable format

[type: Description("An actiontaht can be taken on a cocktail in an owned account's favorites list")]
public enum CocktailFavoritingActionModel
{
    Add = 1,
    Remove = 2
}