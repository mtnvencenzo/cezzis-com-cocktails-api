namespace Cocktails.Api.Application.Concerns.Cocktails.Models;

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

#pragma warning disable format

/// <summary>The cocktail list response</summary>
[type: Description("The cocktail list response")]
public record CocktailsListRs
(
    [property: Required()]
    [property: Description("A list of cocktail recipe list models")]
    IEnumerable<CocktailsListModel> Items
);