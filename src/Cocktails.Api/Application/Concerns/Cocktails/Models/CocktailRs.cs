namespace Cocktails.Api.Application.Concerns.Cocktails.Models;

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

#pragma warning disable format

/// <summary>The cocktail recipe response</summary>
[type: Description("The cocktail recipe response")]
public record CocktailRs
(
    [property: Required()]
    [property: Description("The cocktail recipe model")]
    CocktailModel Item
);