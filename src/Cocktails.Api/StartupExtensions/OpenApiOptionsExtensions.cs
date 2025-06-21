namespace Cocktails.Api.StartupExtensions;

using Asp.Versioning.ApiExplorer;
using Cocktails.Api.Application.Behaviors;
using Cocktails.Api.Domain.Config;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Microsoft.Identity.Web;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;

internal static class OpenApiOptionsExtensions
{
    internal static OpenApiOptions ApplyApiVersionInfo(this OpenApiOptions options, string title, string description, string logoUri, string logoAlt)
    {
        options.AddDocumentTransformer((document, context, cancellationToken) =>
        {
            var versionedDescriptionProvider = context.ApplicationServices.GetService<IApiVersionDescriptionProvider>();
            var apiDescription = versionedDescriptionProvider?.ApiVersionDescriptions
                .SingleOrDefault(description => description.GroupName == context.DocumentName);

            if (apiDescription is null)
            {
                return Task.CompletedTask;
            }

            document.Info.Version = apiDescription.ApiVersion.ToString();
            document.Info.Title = title;
            document.Info.Description = BuildDescription(apiDescription, description);
            document.Info.Extensions = new Dictionary<string, IOpenApiExtension>
            {
                { "x-logo", new OpenApiObject
                    {
                        { "url", new OpenApiString(logoUri)},
                        { "altText", new OpenApiString(logoAlt)}
                    }
                }
            };

            return Task.CompletedTask;
        });
        return options;
    }

    internal static OpenApiOptions ApplySecuritySchemeDefinitions(this OpenApiOptions options)
    {
        options.AddDocumentTransformer<SecuritySchemeDefinitionsTransformer>();
        return options;
    }

    internal static OpenApiOptions ApplyAuthorizationChecks(this OpenApiOptions options)
    {
        options.AddOperationTransformer((operation, context, cancellationToken) =>
        {
            var scalarConfig = context.ApplicationServices.GetRequiredService<IOptions<ScalarConfig>>();

            var metadata = context.Description.ActionDescriptor.EndpointMetadata;

            var requiredScopesMetadatas = metadata.OfType<IAuthRequiredScopeMetadata>();

            if (!requiredScopesMetadatas.Any())
            {
                operation.Security = [];
                return Task.CompletedTask;
            }

            var oAuthScheme = new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "oauth2" }
            };

            var availableScopes = scalarConfig.Value.AuthorizationCodeFlow.Scopes;

            var scopes = requiredScopesMetadatas
                .SelectMany(x => x.AcceptedScope)
                .Where(x => availableScopes.FirstOrDefault(av => av.EndsWith($"/{x}")) != null)
                .ToList();

            operation.Security =
            [
                new()
                {
                    [oAuthScheme] = [.. scopes]
                }
            ];

            return Task.CompletedTask;
        });

        return options;
    }

    internal static OpenApiOptions ApplyOperationDeprecatedStatus(this OpenApiOptions options)
    {
        options.AddOperationTransformer((operation, context, cancellationToken) =>
        {
            var apiDescription = context.Description;
            operation.Deprecated |= apiDescription.IsDeprecated();
            return Task.CompletedTask;
        });
        return options;
    }

    internal static OpenApiOptions AddOperationSubscriptionKeyHeader(this OpenApiOptions options)
    {
        options.AddOperationTransformer((operation, context, cancellationToken) =>
        {
            operation.Parameters ??= [];

            operation.Parameters.Add(new OpenApiParameter
            {
                Description = "Subscription key",
                In = ParameterLocation.Header,
                Required = false,
                Name = "X-Key",
            });

            return Task.CompletedTask;
        });

        return options;
    }

    internal static OpenApiOptions ApplyApiVersionDescription(this OpenApiOptions options)
    {
        options.AddOperationTransformer((operation, context, cancellationToken) =>
        {
            // Find parameter named "api-version" and add a description to it
            var apiVersionParameter = operation.Parameters?.FirstOrDefault(p => p.Name == "api-version");

            if (apiVersionParameter is not null)
            {
                apiVersionParameter.Description = "The API version, in the format 'major.minor'.";
                apiVersionParameter.Schema.Example = new OpenApiString("1.0");
            }

            return Task.CompletedTask;
        });
        return options;
    }

    internal static OpenApiOptions ApplySchemaNullableFalse(this OpenApiOptions options)
    {
        options.AddSchemaTransformer((schema, context, cancellationToken) =>
        {
            if (schema.Properties is not null)
            {
                foreach (var property in schema.Properties)
                {
                    if (schema.Required != null && schema.Required.Contains(property.Key))
                    {
                        property.Value.Nullable = false;
                    }
                    else
                    {
                        property.Value.Nullable = true;
                    }
                }
            }

            return Task.CompletedTask;
        });
        return options;
    }

    internal static OpenApiOptions ApplySchemaPropertyExamples(this OpenApiOptions options)
    {
        options.AddSchemaTransformer((schema, context, cancellationToken) =>
        {
            if (schema.Properties is not null)
            {
                var typeInfo = context.JsonTypeInfo.Type;
                var typeProps = typeInfo.GetProperties();

                foreach (var property in schema.Properties)
                {
                    var propInfo = typeProps.FirstOrDefault(x => x.Name.Equals(property.Key, StringComparison.OrdinalIgnoreCase));

                    if (propInfo != null)
                    {
                        var exampleAttribute = propInfo.GetCustomAttribute<OpenApiUntypedExampleDocAttribute>(inherit: true);

                        if (exampleAttribute != null)
                        {
                            property.Value.Example = exampleAttribute.GetExampleOpenApi();
                        }
                    }
                }
            }

            return Task.CompletedTask;
        });

        return options;
    }

    private static string BuildDescription(ApiVersionDescription api, string description)
    {
        var text = new StringBuilder(description);

        if (api.IsDeprecated)
        {
            if (text.Length > 0)
            {
                if (text[^1] != '.')
                {
                    text.Append('.');
                }

                text.Append(' ');
            }

            text.Append("This API version has been deprecated.");
        }

        if (api.SunsetPolicy is { } policy)
        {
            if (policy.Date is { } when)
            {
                if (text.Length > 0)
                {
                    text.Append(' ');
                }

                text.Append("The API will be sunset on ")
                    .Append(when.Date.ToShortDateString())
                    .Append('.');
            }

            if (policy.HasLinks)
            {
                text.AppendLine();

                var rendered = false;

                foreach (var link in policy.Links.Where(l => l.Type == "text/html"))
                {
                    if (!rendered)
                    {
                        text.Append("<h4>Links</h4><ul>");
                        rendered = true;
                    }

                    text.Append("<li><a href=\"");
                    text.Append(link.LinkTarget.OriginalString);
                    text.Append("\">");
                    text.Append(
                        StringSegment.IsNullOrEmpty(link.Title)
                        ? link.LinkTarget.OriginalString
                        : link.Title.ToString());
                    text.Append("</a></li>");
                }

                if (rendered)
                {
                    text.Append("</ul>");
                }
            }
        }

        return text.ToString();
    }

    private class SecuritySchemeDefinitionsTransformer(IConfiguration configuration) : IOpenApiDocumentTransformer
    {
        public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
        {
            var azureAdB2cConfig = new AzureAdB2cConfig();
            configuration.Bind(AzureAdB2cConfig.SectionName, azureAdB2cConfig);

            var scalarConfig = new ScalarConfig();
            configuration.Bind(ScalarConfig.SectionName, scalarConfig);

            var scopes = new Dictionary<string, string>();
            scalarConfig.AuthorizationCodeFlow.Scopes.ForEach(x =>
            {
                scopes.Add(x, x[(x.LastIndexOf("/") + 1)..]);
            });

            var securityScheme = new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows
                {
                    AuthorizationCode = new OpenApiOAuthFlow
                    {
                        AuthorizationUrl = new Uri($"{azureAdB2cConfig.Instance}/{azureAdB2cConfig.Domain}/{azureAdB2cConfig.SignUpSignInPolicyId}/oauth2/v2.0/authorize"),
                        TokenUrl = new Uri($"{azureAdB2cConfig.Instance}/{azureAdB2cConfig.Domain}/{azureAdB2cConfig.SignUpSignInPolicyId}/oauth2/v2.0/token"),
                        Scopes = scopes,
                        Extensions = new Dictionary<string, IOpenApiExtension>
                        {
                            { "x-defaultClientId", new OpenApiString(scalarConfig.AuthorizationCodeFlow.ClientId) },
                            { "x-usePkce", new OpenApiString("SHA-256") }
                        }
                    }
                },
            };

            document.Components ??= new();
            document.Components.SecuritySchemes.Add("oauth2", securityScheme);

            return Task.CompletedTask;
        }
    }
}
