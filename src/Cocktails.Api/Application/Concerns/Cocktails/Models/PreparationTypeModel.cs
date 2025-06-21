namespace Cocktails.Api.Application.Concerns.Cocktails.Models;

using global::Cocktails.Api.Application.Behaviors;
using global::Cocktails.Api.Domain.Aggregates.CocktailAggregate;
using System.ComponentModel;

#pragma warning disable format

public class PreparationTypeModelConverter : EnumTypeConverter<PreparationType, PreparationTypeModel> { }

/// <summary>
/// 
/// </summary>
[TypeConverter(typeof(PreparationTypeModelConverter))]
public enum PreparationTypeModel
{
    /// <summary>The none</summary>
    None = 0,

    /// <summary>The chilled</summary>
    Chilled = 1,

    /// <summary>The freshly squeezed</summary>
    FreshlySqueezed = 2,

    /// <summary>The peeled and juiced</summary>
    PeeledAndJuiced = 3,

    /// <summary>The freshly grated</summary>
    FreshlyGrated = 4,

    /// <summary>The quartered</summary>
    Quartered = 5,

    /// <summary>The fresh pressed</summary>
    FreshPressed = 6
}
