namespace Cocktails.Api.Application.Concerns.Cocktails.Models;

using global::Cocktails.Api.Application.Behaviors;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

#pragma warning disable format

/// <summary>The cocktail image model</summary>
[type: Description("The cocktail image model")]
public record CocktailImageModel
(
    [property: Required()]
    [property: Description("The uri of the image")]
    [property: OpenApiExampleDoc<string>("https://cdn.cezzis.com/cocktails/traditional-clover-club-cocktail-main.webp")]
    string Uri,

    [property: Description("The width of the image")]
    [property: OpenApiExampleDoc<int>(800)]
    int Width,

    [property: Description("The height of the image")]
    [property: OpenApiExampleDoc<int>(533)]
    int Height
);

/// <summary>The cocktail image model</summary>
[type: Description("The cocktail image model")]
public record CocktailImageModel2
(
    [property: Required()]
    [property: Description("The uri of the image")]
    [property: OpenApiExampleDoc<string>("https://cdn.cezzis.com/cocktails/traditional-clover-club-cocktail-main.webp")]
    string Uri,

    [property: Description("The width of the image")]
    [property: OpenApiExampleDoc<int>(800)]
    int Width,

    [property: Description("The height of the image")]
    [property: OpenApiExampleDoc<int>(533)]
    int Height
);