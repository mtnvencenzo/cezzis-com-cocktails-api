namespace Cocktails.Api.Application.Concerns.Cocktails.Models;

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

#pragma warning disable format

/// <summary>The cocktail search filters response</summary>
[type: Description("The cocktail search filters response")]
public record CocktailIngredientFiltersRs
(
    [property: Required()]
    [property: Description("The cocktail ingredient filters for searching against recommended glassware")]
    IEnumerable<IngredientFilterModel> Glassware,

    [property: Required()]
    [property: Description("The cocktail ingredient filters for searching against spirits")]
    IEnumerable<IngredientFilterModel> Spirits,

    [property: Required()]
    [property: Description("The cocktail ingredient filters for searching against liqueurs")]
    IEnumerable<IngredientFilterModel> Liqueurs,

    [property: Required()]
    [property: Description("The cocktail ingredient filters for searching against friuts")]
    IEnumerable<IngredientFilterModel> Fruits,

    [property: Required()]
    [property: Description("The cocktail ingredient filters for searching against vegetables")]
    IEnumerable<IngredientFilterModel> Vegetables,

    [property: Required()]
    [property: Description("The cocktail ingredient filters for searching against herbs and flowers")]
    IEnumerable<IngredientFilterModel> HerbsAndFlowers,

    [property: Required()]
    [property: Description("The cocktail ingredient filters for searching against syrups and sauces")]
    IEnumerable<IngredientFilterModel> SyrupsAndSauces,

    [property: Required()]
    [property: Description("The cocktail ingredient filters for searching against bitters")]
    IEnumerable<IngredientFilterModel> Bitters,

    [property: Required()]
    [property: Description("The cocktail ingredient filters for searching against proteins")]
    IEnumerable<IngredientFilterModel> Proteins,

    [property: Required()]
    [property: Description("The cocktail ingredient filters for searching against juices")]
    IEnumerable<IngredientFilterModel> Juices,

    [property: Required()]
    [property: Description("The cocktail ingredient filters for searching against dilutions")]
    IEnumerable<IngredientFilterModel> Dilutions,

    [property: Required()]
    [property: Description("The cocktail ingredient filters for searching against beers, wines and champagnes")]
    IEnumerable<IngredientFilterModel> BeerWineChampagne,

    [property: Required()]
    [property: Description("The cocktail ingredient filters for searching against eras when cocktails were established")]
    IEnumerable<IngredientFilterModel> Eras
);
