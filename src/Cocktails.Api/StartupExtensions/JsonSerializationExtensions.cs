namespace Cocktails.Api.StartupExtensions;

using System.Text.Json;
using System.Text.Json.Serialization;

internal static class JsonSerializationExtensions
{
    internal static IServiceCollection ConfigureJsonSerialization(this IServiceCollection services)
    {
        // ---------------------------------------------------------------------
        // Why I'm doing this twice???
        // https://github.com/domaindrivendev/Swashbuckle.AspNetCore/issues/2293
        // ---------------------------------------------------------------------
        services.ConfigureHttpJsonOptions((o) =>
        {
            o.SerializerOptions.AllowTrailingCommas = true;
            o.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            o.SerializerOptions.PropertyNameCaseInsensitive = true;
            o.SerializerOptions.Converters.Add(new JsonStringEnumConverter(namingPolicy: JsonNamingPolicy.CamelCase, allowIntegerValues: true));
            o.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            o.SerializerOptions.ReadCommentHandling = JsonCommentHandling.Skip;
        });

        services.Configure<Microsoft.AspNetCore.Mvc.JsonOptions>((o) =>
        {
            o.JsonSerializerOptions.AllowTrailingCommas = true;
            o.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            o.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
            o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(namingPolicy: JsonNamingPolicy.CamelCase, allowIntegerValues: true));
            o.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            o.JsonSerializerOptions.ReadCommentHandling = JsonCommentHandling.Skip;
        });

        return services;
    }
}
