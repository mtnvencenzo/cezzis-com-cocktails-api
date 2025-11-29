namespace Cocktails.Api.Application.Concerns.Cocktails.Models;

using global::Cocktails.Api.Application.Behaviors;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

#pragma warning disable format

/// <summary>The publish cocktails request model</summary>
[type: Description("The publish cocktails request model")]
public record PublishCocktailsRq
(
    [property: Required()]
    [property: Description("A list of cocktail IDs to publish")]
    [property: OpenApiExampleDoc<string>(["pegu-club", "aperol-spritz", "margarita"])]
    IEnumerable<string> CocktailIds
);