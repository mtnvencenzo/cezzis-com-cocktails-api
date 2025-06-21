namespace Cocktails.Api.Application.Concerns.Cocktails.Models;

using global::Cocktails.Api.Application.Behaviors;
using global::Cocktails.Api.Domain.Aggregates.CocktailAggregate;
using System.ComponentModel;

#pragma warning disable format

public class UofMTypeModelConverter : EnumTypeConverter<UofM, UofMTypeModel> { }

/// <summary>
/// 
/// </summary>
[TypeConverter(typeof(UofMTypeModelConverter))]
public enum UofMTypeModel
{
    /// <summary>The none</summary>
    None = 0,

    /// <summary>The ounces</summary>
    Ounces = 1,

    /// <summary>The dashes</summary>
    Dashes = 2,

    /// <summary>The tablespoon</summary>
    Tablespoon = 3,

    /// <summary>The topoff</summary>
    Topoff = 4,

    /// <summary>The item</summary>
    Item = 5,

    /// <summary>The teaspoon</summary>
    Teaspoon = 6,

    /// <summary>Converts to taste.</summary>
    ToTaste = 7,

    /// <summary>The barspoon</summary>
    Barspoon = 8,

    /// <summary>The cups</summary>
    Cups = 9,

    /// <summary>The splash</summary>
    Splash = 10,

    /// <summary>The discretion</summary>
    Discretion = 11
}
