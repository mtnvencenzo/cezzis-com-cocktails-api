namespace Cocktails.Api.StartupExtensions;

using Cocktails.Api.Apis.Accounts;
using Cocktails.Api.Apis.Cockails;
using Cocktails.Api.Apis.Health;
using Cocktails.Api.Apis.Integrations;
using Cocktails.Api.Apis.Jobs;
using Cocktails.Api.Apis.LegalDocuments;
using Cocktails.Api.Apis.LocalImages;
using Cocktails.Api.Application.Behaviors.ApimHostKeyAuthorization;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.HttpLogging;

internal static class EndpointExtensions
{
    internal static WebApplication UseApplicationEndpoints(this WebApplication app)
    {
        var rootApi = app.UseDefaultEndpoints();
        rootApi.MapIntegrationsApi();
        rootApi.MapJobsApi();
        rootApi.MapSubscribeHandler();

        var cocktailsApi = app.NewVersionedApi("Cocktails")
            .MapGroup("api/v{version:apiVersion}")
            .HasApiVersion(1.0)
            .RequireAuthorization(ApimHostKeyRequirement.PolicyName);

        cocktailsApi.MapCocktailsApiV1();
        cocktailsApi.MapCocktailsAdminApiV1();
        cocktailsApi.MapHealthApiV1();
        cocktailsApi.MapLegalDocumentsApiV1();
        cocktailsApi.MapLocalImagesApiV1();
        cocktailsApi.MapAccountsApiV1();

        return app;
    }

    private static WebApplication UseDefaultEndpoints(this WebApplication app)
    {
        // Adding health checks endpoints to applications in non-development environments has security implications.
        // See https://aka.ms/dotnet/aspire/healthchecks for details before enabling these endpoints in non-development environments.
        if (app.Environment.IsEnvironment("local"))
        {
            // All health checks must pass for app to be considered ready to accept traffic after starting
            app.MapHealthChecks("/health")
                .ExcludeFromDescription()
                .WithHttpLogging(HttpLoggingFields.None);

            // Only health checks tagged with the "live" tag must pass for app to be considered alive
            app.MapHealthChecks("/alive", new HealthCheckOptions
            {
                Predicate = r => r.Tags.Contains("live")
            }).ExcludeFromDescription().WithHttpLogging(HttpLoggingFields.None);
        }

        return app;
    }
}
