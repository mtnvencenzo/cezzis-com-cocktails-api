namespace Cocktails.Api.Infrastructure.EntityConfigurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;

public static class EntityConfigurationExtensions
{
    private readonly static JsonNamingPolicy namingPolicy = JsonNamingPolicy.CamelCase;

    public static EntityTypeBuilder<T> ApplyCamelCasingNamingStrategry<T>(this EntityTypeBuilder<T> builder) where T : class
    {
        foreach (var property in builder.Metadata.GetDeclaredProperties())
        {
            property.SetJsonPropertyName(namingPolicy.ConvertName(property.Name));
        }

        return builder;
    }

    public static OwnedNavigationBuilder<TParent, TChild> ApplyCamelCasingNamingStrategry<TParent, TChild>(this OwnedNavigationBuilder<TParent, TChild> builder) where TParent : class where TChild : class
    {
        var containingPropertyName = builder.OwnedEntityType.GetContainingPropertyName();

        builder.OwnedEntityType.SetContainingPropertyName(namingPolicy.ConvertName(containingPropertyName));

        foreach (var property in builder.OwnedEntityType.GetDeclaredProperties())
        {
            property.SetJsonPropertyName(namingPolicy.ConvertName(property.Name));
        }

        return builder;
    }
}
