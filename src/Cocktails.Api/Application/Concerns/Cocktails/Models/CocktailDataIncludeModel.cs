namespace Cocktails.Api.Application.Concerns.Cocktails.Models;

using System.ComponentModel;

#pragma warning disable format

/// <summary>Specifies extra data that should be returned with the main cocktail data</summary>
[type: Description("Specifies extra data that should be returned with the main cocktail data")]
public enum CocktailDataIncludeModel
{
    /// <summary> The main image paths should be returned</summary>
    [field: Description("The main image paths should be returned")]
    mainImages = 1,

    /// <summary>The search tile image paths should be returned</summary>
    [field: Description("The search tile image paths should be returned")]
    searchTiles = 2,

    /// <summary>The longer descriptive title should be returned</summary>
    [field: Description("The longer descriptive title should be returned")]
    descriptiveTitle = 3
}
