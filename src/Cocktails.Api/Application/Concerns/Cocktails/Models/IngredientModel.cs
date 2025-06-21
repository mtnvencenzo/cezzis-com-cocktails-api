namespace Cocktails.Api.Application.Concerns.Cocktails.Models;

using global::Cocktails.Api.Application.Behaviors;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

#pragma warning disable format

/// <summary>The cocktail ingredient model</summary>
[type: Description("The cocktail ingredient model")]
public record IngredientModel
(
    [property: Required()]
    [property: Description("The name of the ingredient")]
    [property: OpenApiExampleDoc<string>("Gin")]
    string Name,

    [property: Required()]
    [property: Description("The unit of measure when using this ingredient in a cocktail recipe")]
    [property: OpenApiExampleDoc<UofMTypeModel>(UofMTypeModel.Ounces)]
    UofMTypeModel UoM,

    [property: Required()]
    [property: Description("Whether or not this ingredient is required ('Required' or 'Optional')")]
    [property: OpenApiExampleDoc<IngredientRequirementTypeModel>(IngredientRequirementTypeModel.Required)]
    IngredientRequirementTypeModel Requirement,

    [property: Required()]
    [property: Description("Gets the complete display value for the ingredient including units and measurments")]
    [property: OpenApiExampleDoc<string>("1 1/2 ounces gin")]
    string Display,

    [property: Required()]
    [property: Description("The number of units to use in relation to the UoM (unit of measure) in the cocktail recipe")]
    [property: OpenApiExampleDoc<float>(1.5F)]
    float Units,

    [property: Required()]
    [property: Description("Any preparation that should be made with this ingredient")]
    [property: OpenApiExampleDoc<PreparationTypeModel>(PreparationTypeModel.None)]
    PreparationTypeModel Preparation,

    [property: Required()]
    [property: Description("Suggestion when using this ingredient")]
    [property: OpenApiExampleDoc<string>("Preferably Plymouth")]
    string Suggestions,

    [property: Required()]
    [property: Description("The ingredient types that this ingredient is in relation to the cocktail recipe")]
    [property: OpenApiExampleDoc<IngredientTypeModel>(IngredientTypeModel.Bitters, IngredientTypeModel.Spirit)]
    IEnumerable<IngredientTypeModel> Types,

    [property: Required()]
    [property: Description("The ingredient applications that this ingredient is in relation to the cocktail recipe")]
    [property: OpenApiExampleDoc<IngredientApplicationModel>(IngredientApplicationModel.Base, IngredientApplicationModel.Garnishment)]
    IEnumerable<IngredientApplicationModel> Applications
);

