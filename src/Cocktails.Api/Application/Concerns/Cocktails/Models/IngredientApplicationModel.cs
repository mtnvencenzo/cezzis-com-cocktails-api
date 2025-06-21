namespace Cocktails.Api.Application.Concerns.Cocktails.Models;

using global::Cocktails.Api.Application.Behaviors;
using global::Cocktails.Api.Domain.Aggregates.IngredientAggregate;
using System.ComponentModel;

#pragma warning disable format

public class IngredientApplicationModelConverter : EnumTypeConverter<IngredientApplication, IngredientApplicationModel> { }

/// <summary>
/// 
/// </summary>
[TypeConverter(typeof(IngredientApplicationModelConverter))]
public enum IngredientApplicationModel
{
    /// <summary>The base</summary>
    Base = 1,

    /// <summary>The additional</summary>
    Additional = 2,

    /// <summary>The garnishment</summary>
    Garnishment = 3,

    /// <summary>The muddle</summary>
    Muddle = 4
}
