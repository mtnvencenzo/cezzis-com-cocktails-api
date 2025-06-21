namespace Cocktails.Api.Application.Concerns.Cocktails.Models;

using global::Cocktails.Api.Application.Behaviors;
using global::Cocktails.Api.Domain.Aggregates.IngredientAggregate;
using System.ComponentModel;

#pragma warning disable format

public class IngredientTypeModelConverter : EnumTypeConverter<IngredientType, IngredientTypeModel> { }

/// <summary>
/// 
/// </summary>
[TypeConverter(typeof(IngredientTypeModelConverter))]
public enum IngredientTypeModel
{
    /// <summary>The herb</summary>
    Herb = 1,

    /// <summary>The fruit</summary>
    Fruit = 2,

    /// <summary>The juice</summary>
    Juice = 3,

    /// <summary>The bitters</summary>
    Bitters = 4,

    /// <summary>The syrup</summary>
    Syrup = 5,

    /// <summary>The protein</summary>
    Protein = 6,

    /// <summary>The flowers</summary>
    Flowers = 7,

    /// <summary>The sauce</summary>
    Sauce = 8,

    /// <summary>The vegetables</summary>
    Vegetable = 9,

    /// <summary>The dilutions</summary>
    Dilution = 10,

    /// <summary>The beer</summary>
    Beer = 11,

    /// <summary>The spirit</summary>
    Spirit = 12,

    /// <summary>The liqueur</summary>
    Liqueur = 13,

    /// <summary>The wine</summary>
    Wine = 14,

    /// <summary>The champagne</summary>
    Champagne = 15
}
