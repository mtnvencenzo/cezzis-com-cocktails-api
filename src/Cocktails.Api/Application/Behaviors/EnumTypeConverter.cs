namespace Cocktails.Api.Application.Behaviors;

using Cocktails.Api.Domain.Aggregates.CocktailAggregate;
using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

public class EnumTypeConverter<TFrom, TTo> : TypeConverter where TFrom : Enum where TTo : Enum
{
    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) => sourceType == typeof(TFrom) || base.CanConvertFrom(context, sourceType);

    public override bool CanConvertTo(ITypeDescriptorContext context, [NotNullWhen(true)] Type destinationType) => destinationType == typeof(TFrom) || base.CanConvertTo(context, destinationType);

    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
    {
        var castedSource = (TTo)value;

        return (TFrom)Enum.Parse(typeof(TTo), castedSource.ToString());
    }

    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
        var castedSource = (TFrom)value;

        return (TTo)Enum.Parse(typeof(TFrom), castedSource.ToString());
    }
}