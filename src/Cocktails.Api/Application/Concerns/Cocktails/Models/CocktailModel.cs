namespace Cocktails.Api.Application.Concerns.Cocktails.Models;

using global::Cocktails.Api.Application.Behaviors;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

#pragma warning disable format

/// <summary>The cocktail recipe model</summary>
[type: Description("The cocktail recipe model")]
public record CocktailModel
(
    [property: Required()]
    [property: Description("The cocktail recipe unique identifier")]
    [property: OpenApiExampleDoc<string>("clover-club")]
    string Id,

    [property: Required()]
    [property: Description("The name of the cocktail recipe")]
    [property: OpenApiExampleDoc<string>("Clover Club")]
    string Title,

    [property: Required()]
    [property: Description("A more descriptive title for the cocktail recipe, generally used as an editorial title")]
    [property: OpenApiExampleDoc<string>("Clover Club: A Pink-Hued Classic")]
    string DescriptiveTitle,

    [property: Required()]
    [property: Description("A brief editorial description for the cocktail recipe")]
    [property: OpenApiExampleDoc<string>("A classic pre-Prohibition cocktail made with gin, raspberry syrup, lemon juice, and egg white. Named after a Philadelphia men's club, it's a smooth, frothy, and slightly tart cocktail.")]
    string Description,

    [property: Required()]
    [property: Description("The complete descriptive cocktail recipe including ingredients, directions and historical information in markdown format")]
    [property: OpenApiExampleDoc<string>("--markdown content--")]
    string Content,

    [property: Required()]
    [property: Description("The date this cocktail recipe was published on Cezzis.Com")]
    [property: OpenApiExampleDoc<string>("2024-05-24T00:00:00-07:00")]
    DateTimeOffset PublishedOn,

    // <example>2024-05-24T00:00:00-07:00</example>
    [property: Required()]
    [property: Description("The date this cocktail recipe was last modified on Cezzis.Com")]
    [property: OpenApiExampleDoc<string>("2024-05-24T00:00:00-07:00")]
    DateTimeOffset ModifiedOn,

    [property: Required()]
    [property: Description("The number of people the cocktail recipe serves")]
    [property: OpenApiExampleDoc<int>(1)]
    int Serves,

    [property: Required()]
    [property: Description("The average number of minutes to build the cocktail using this recipe")]
    [property: OpenApiExampleDoc<int>(5)]
    int PrepTimeMinutes,

    [property: Required()]
    [property: Description("Whether or not the cocktail represented by this recipe is recognized by the International Bartenders Association")]
    [property: OpenApiExampleDoc<bool>(true)]
    bool IsIba,

    [property: Required()]
    [property: Description("A list of primary images for the cocktail recipe")]
    IEnumerable<CocktailImageModel> MainImages,

    [property: Required()]
    [property: Description("A list of secondary, smaller sized images for the cocktail recipe")]
    IEnumerable<CocktailImageModel2> SearchTiles,

    [property: Required()]
    [property: Description("The recommended glassware to use when serving the cocktail")]
    [property: OpenApiExampleDoc<GlasswareTypeModel>(GlasswareTypeModel.Coupe, GlasswareTypeModel.CocktailGlass)]
    IEnumerable<GlasswareTypeModel> Glassware,

    [property: Required()]
    [property: Description("A ratings for this cocktail")]
    CocktailRatingModel Rating,

    // <example>[&quot;Clover Club&quot;]</example>
    [property: Required()]
    [property: Description("A list of titles that are queried against when issuing cocktail recipe search queries")]
    [property: OpenApiExampleDoc<string>("Clover Club", "Clover Club: A Pink-Hued Classic")]
    IEnumerable<string> SearchableTitles,

    [property: Required()]
    [property: Description("A list of taxonomy tags defining the cocktail recipe")]
    [property: OpenApiExampleDoc<string>(["Traditional"])]
    IEnumerable<string> Tags,

    [property: Required()]
    [property: Description("The list of ingredients that make up the cocktail recipe")]
    IEnumerable<IngredientModel> Ingredients,

    [property: Required()]
    [property: Description("The list of instructions to make the cocktail recipe")]
    IEnumerable<InstructionStepModel> Instructions
);
