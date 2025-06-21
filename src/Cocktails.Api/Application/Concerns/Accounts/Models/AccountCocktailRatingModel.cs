namespace Cocktails.Api.Application.Concerns.Accounts.Models;

using global::Cocktails.Api.Application.Behaviors;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

#pragma warning disable format

/// <summary>The cocktail recommendation model</summary>
[type: Description("The cocktail user ratings")]
public record AccountCocktailRatingModel
(
    [property: Required()]
    [property: Description("The number of one star ratings")]
    [property: OpenApiExampleDoc<string>("5")]
    int OneStars,

    [property: Required()]
    [property: Description("The number of two star ratings")]
    [property: OpenApiExampleDoc<string>("1")]
    int TwoStars,

    [property: Required()]
    [property: Description("The number of three star ratings")]
    [property: OpenApiExampleDoc<string>("1")]
    int ThreeStars,

    [property: Required()]
    [property: Description("The number of four star ratings")]
    [property: OpenApiExampleDoc<string>("1")]
    int FourStars,

    [property: Required()]
    [property: Description("The number of five star ratings")]
    [property: OpenApiExampleDoc<string>("1")]
    int FiveStars,

    [property: Required()]
    [property: Description("The total number of stars given")]
    [property: OpenApiExampleDoc<string>("1")]
    int TotalStars,

    [property: Required()]
    [property: Description("The actual overal rating")]
    [property: OpenApiExampleDoc<string>("3.5")]
    decimal Rating,

    [property: Required()]
    [property: Description("The total number of ratings given")]
    [property: OpenApiExampleDoc<string>("1")]
    int RatingCount
);