namespace Cocktails.Api.Application.Concerns.Cocktails.Models;

using global::Cocktails.Api.Application.Behaviors;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Microsoft.Graph.Models;

#pragma warning disable format

/// <summary>The cocktail recipe shortend list model</summary>
[type: Description("The cocktail recipe shortend list model")]
public record CocktailsListModel
(
    // <example>clover-club</example>
    [property: Required()]
    [property: Description("The cocktail recipe unique identifier")]
    [property: OpenApiExampleDoc<string>("clover-club")]
    string Id,

    [property: Required()]
    [property: Description("The name of the cocktail recipe")]
    [property: OpenApiExampleDoc<string>("Clover Club")]
    string Title,

    [property: Description("A more descriptive title for the cocktail recipe, generally used as an editorial title")]
    [property: OpenApiExampleDoc<string>("Clover Club: A Pink-Hued Classic")]
    string DescriptiveTitle,

    [property: Required()]
    [property: Description("The overral rating for the recipe")]
    [property: OpenApiExampleDoc<string>("3.5")]
    decimal Rating,

    [property: Required()]
    [property: Description("The list of ingredients that make up the cocktail recipe")]
    IEnumerable<IngredientModel> Ingredients,

    [property: Required()]
    [property: Description("Whether or not the cocktail represented by this recipe is recognized by the International Bartenders Association")]
    [property: OpenApiExampleDoc<bool>(true)]
    bool IsIba,

    [property: Required()]
    [property: Description("The number of people the cocktail recipe serves")]
    [property: OpenApiExampleDoc<int>(1)]
    int Serves,

    [property: Required()]
    [property: Description("The average number of minutes to build the cocktail using this recipe")]
    [property: OpenApiExampleDoc<int>(5)]
    int PrepTimeMinutes,

    [property: Description("A list of primary image uris for the cocktail recipe")]
    [property: OpenApiExampleDoc<string>(["https://cdn.cezzis.com/cocktails/traditional-clover-club-cocktail-main.webp"])]
    IEnumerable<string> MainImages,

    [property: Description("A list of secondary, smaller sized image uris for the cocktail recipe")]
    [property: OpenApiExampleDoc<string>(["https://cdn.cezzis.com/cocktails/traditional-clover-club-cocktail-300x300.webp"])]
    IEnumerable<string> SearchTiles,

    [property: Required()]
    [property: Description("The recommended glassware to use when serving the cocktail")]
    [property: OpenApiExampleDoc<GlasswareTypeModel>(GlasswareTypeModel.Coupe, GlasswareTypeModel.CocktailGlass)]
    IEnumerable<GlasswareTypeModel> Glassware
);
