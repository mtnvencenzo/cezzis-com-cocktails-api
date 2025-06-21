namespace Cocktails.Api.Application.Concerns.Cocktails.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

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
    IEnumerable<IngredientFilterModel2> Spirits,

    [property: Required()]
    [property: Description("The cocktail ingredient filters for searching against liqueurs")]
    IEnumerable<IngredientFilterModel3> Liqueurs,

    [property: Required()]
    [property: Description("The cocktail ingredient filters for searching against friuts")]
    IEnumerable<IngredientFilterModel4> Fruits,

    [property: Required()]
    [property: Description("The cocktail ingredient filters for searching against vegetables")]
    IEnumerable<IngredientFilterModel5> Vegetables,

    [property: Required()]
    [property: Description("The cocktail ingredient filters for searching against herbs and flowers")]
    IEnumerable<IngredientFilterModel6> HerbsAndFlowers,

    [property: Required()]
    [property: Description("The cocktail ingredient filters for searching against syrups and sauces")]
    IEnumerable<IngredientFilterModel7> SyrupsAndSauces,

    [property: Required()]
    [property: Description("The cocktail ingredient filters for searching against bitters")]
    IEnumerable<IngredientFilterModel8> Bitters,

    [property: Required()]
    [property: Description("The cocktail ingredient filters for searching against proteins")]
    IEnumerable<IngredientFilterModel9> Proteins,

    [property: Required()]
    [property: Description("The cocktail ingredient filters for searching against juices")]
    IEnumerable<IngredientFilterModel10> Juices,

    [property: Required()]
    [property: Description("The cocktail ingredient filters for searching against dilutions")]
    IEnumerable<IngredientFilterModel11> Dilutions,

    [property: Required()]
    [property: Description("The cocktail ingredient filters for searching against beers, wines and champagnes")]
    IEnumerable<IngredientFilterModel12> BeerWineChampagne,

    [property: Required()]
    [property: Description("The cocktail ingredient filters for searching against eras when cocktails were established")]
    IEnumerable<IngredientFilterModel13> Eras
);
