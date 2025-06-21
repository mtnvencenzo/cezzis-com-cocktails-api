namespace Cocktails.Api.Application.Concerns.Cocktails.Models;

using global::Cocktails.Api.Application.Behaviors;
using global::Cocktails.Api.Domain.Aggregates.CocktailAggregate;
using System.ComponentModel;

#pragma warning disable format

public class IngredientRequirementTypeModelConverter : EnumTypeConverter<IngredientRequirement, IngredientRequirementTypeModel> { }

/// <summary>
/// 
/// </summary>
[TypeConverter(typeof(IngredientRequirementTypeModelConverter))]
public enum IngredientRequirementTypeModel
{
    /// <summary>The none</summary>
    None = 0,

    /// <summary>The optional</summary>
    Optional = 1,

    /// <summary>The required</summary>
    Required = 2
}