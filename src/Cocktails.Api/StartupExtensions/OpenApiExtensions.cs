namespace Cocktails.Api.StartupExtensions;

using Asp.Versioning;
using Cocktails.Api.Apis.Cockails;
using Cocktails.Api.Domain.Config;
using Microsoft.Extensions.Options;
using Scalar.AspNetCore;

internal static class OpenApiExtensions
{
    internal static IHostApplicationBuilder AddDefaultOpenApi(
        this IHostApplicationBuilder builder,
        IApiVersioningBuilder apiVersioning = default)
    {
        if (apiVersioning is not null)
        {
            // the default format will just be ApiVersion.ToString(); for example, 1.0.
            // this will format the version as "'v'major[.minor][-status]"
            var versioned = apiVersioning.AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });

            string[] versions = ["v1"];

            foreach (var version in versions)
            {
                builder.Services.AddOpenApi(version, options =>
                {
                    options.ApplyApiVersionInfo("Cocktails Api", "Cocktails Api", "/api-docs-logo.png", "Api documentation for the cocktails api");
                    options.ApplyAuthorizationChecks();
                    options.ApplySecuritySchemeDefinitions();
                    options.ApplyOperationDeprecatedStatus();
                    options.ApplyApiVersionDescription();
                    options.ApplySchemaNullableFalse();
                    options.ApplySchemaPropertyExamples();
                    options.AddOperationSubscriptionKeyHeader();
                });
            }
        }

        return builder;
    }

    internal static IApplicationBuilder UseDefaultOpenApi(this WebApplication app)
    {
        var cocktailsApiOptions = app.Services.GetRequiredService<IOptions<CocktailsApiConfig>>();

        var openApiPattern = "/scalar/{documentName}/openapi.json";

        app.MapOpenApi(pattern: openApiPattern);

        app.MapScalarApiReference(options =>
        {
            // Disable default fonts to avoid download unnecessary fonts
            options.DefaultFonts = false;
            options.Favicon = "/favicon.svg";
            options.Title = "Cezzi's Cocktails Api";
            options.Theme = ScalarTheme.Purple;
            options.OpenApiRoutePattern = openApiPattern;
            options.Servers =
            [
                new ScalarServer(cocktailsApiOptions.Value.BaseOpenApiUri, "Default")
            ];
        });

        app.MapGet("/", () => Results.Redirect("/scalar/v1")).ExcludeFromDescription();

        return app;
    }
}
