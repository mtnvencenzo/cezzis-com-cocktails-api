namespace Cocktails.Api.Application.Concerns.Cocktails.Models;

using global::Cocktails.Api.Application.Behaviors;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

#pragma warning disable format

/// <summary>The cocktail ingredient filter model</summary>
[type: Description("The cocktail ingredient filter model")]
public record IngredientFilterModel
(
    [property: Required()]
    [property: Description("The filter identifier")]
    [property: OpenApiExampleDoc<string>("classification-name")]
    string Id,

    [property: Required()]
    [property: Description("The display name of the filter")]
    [property: OpenApiExampleDoc<string>("Name Of Filter")]
    string Name
);
