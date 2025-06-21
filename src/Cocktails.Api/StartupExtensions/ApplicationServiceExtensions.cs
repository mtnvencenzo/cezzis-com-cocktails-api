namespace Cocktails.Api.StartupExtensions;

using Azure.Identity;
using Cezzi.Security.Recaptcha;
using Cocktails.Api.Application.Behaviors.MediatRPipelines;
using Cocktails.Api.Application.Concerns.Accounts.Queries;
using Cocktails.Api.Application.Concerns.Cocktails.Queries;
using Cocktails.Api.Application.Concerns.Health.Queries;
using Cocktails.Api.Application.Concerns.LegalDocuments.Queries;
using Cocktails.Api.Application.Concerns.LocalImages.Queries;
using Cocktails.Api.Domain.Aggregates.AccountAggregate;
using Cocktails.Api.Domain.Aggregates.CocktailAggregate;
using Cocktails.Api.Domain.Aggregates.HealthAggregate;
using Cocktails.Api.Domain.Aggregates.IngredientAggregate;
using Cocktails.Api.Domain.Aggregates.LegalDocumentAggregate;
using Cocktails.Api.Domain.Config;
using Cocktails.Api.Infrastructure;
using Cocktails.Api.Infrastructure.Repositories;
using Cocktails.Api.Infrastructure.Services;
using FluentValidation;
using Microsoft.Extensions.Options;
using Microsoft.Graph;

internal static class ApplicationServiceExtensions
{
    internal static void AddApplicationServices(this WebApplicationBuilder builder)
    {
        var env = builder.Environment.EnvironmentName;

        builder.Configuration
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{env}.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"appsettings.{env}.json.user", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables();

        builder.Services.Configure<CocktailsApiConfig>(builder.Configuration.GetSection(CocktailsApiConfig.SectionName));
        builder.Services.Configure<CocktailsWebConfig>(builder.Configuration.GetSection(CocktailsWebConfig.SectionName));
        builder.Services.Configure<EmailHandlingConfig>(builder.Configuration.GetSection(EmailHandlingConfig.SectionName));
        builder.Services.Configure<LocalhostImagesConfig>(builder.Configuration.GetSection(LocalhostImagesConfig.SectionName));
        builder.Services.Configure<ZohoEmailConfig>(builder.Configuration.GetSection(ZohoEmailConfig.SectionName));
        builder.Services.Configure<BlobStorageConfig>(builder.Configuration.GetSection(BlobStorageConfig.SectionName));
        builder.Services.Configure<CosmosDbConfig>(builder.Configuration.GetSection(CosmosDbConfig.SectionName));
        builder.Services.Configure<MsGraphConfig>(builder.Configuration.GetSection(MsGraphConfig.SectionName));
        builder.Services.Configure<ScalarConfig>(builder.Configuration.GetSection(ScalarConfig.SectionName));
        builder.Services.Configure<SearchConfig>(builder.Configuration.GetSection(SearchConfig.SectionName));

        builder.Services.AddCosomsContexts();

        builder.Services.AddScoped<IMsGraphClient, MsGraphClient>();
        builder.Services.AddScoped<GraphServiceClient>((sp) =>
        {
            var options = sp.GetRequiredService<IOptions<MsGraphConfig>>().Value;

            var clientSecretCredential = new Azure.Identity.ClientSecretCredential(
                tenantId: options.TenantId,
                clientId: options.ClientId,
                clientSecret: options.ClientSecret);

            return new GraphServiceClient(
            tokenCredential: clientSecretCredential,
            scopes: ["https://graph.microsoft.com/.default"]);

            // NOTE: Use Default scope for client credentials
            // ---------------------------------------------------------------------------------------------------------------
            // The error "AADSTS1002012: The provided value for scope User.ReadWrite.All is not valid. Client credential flows
            // must have a scope value with /" indicates an issue with the scope used in a client credential flow for Azure
            // Active Directory authentication.You need to use the.default scope, not individual permissions, for client
            // credentials. 
            // ---------------------------------------------------------------------------------------------------------------
        });

        builder.Services.AddZohoEmail();

        // add the authentication and authorization services to DI
        builder.Services.AddDefaultAuthentication(builder.Configuration);
        builder.Services.AddDefaultAuthorization();

        // Register the dapr client
        builder.Services.AddDaprClient();

        // Add dapr serice bus messaging to DI
        builder.Services.AddEventBus(builder.Configuration);

        // Add dapr blob storage to DI
        builder.Services.AddStorageBus();

        // Add search client to DI
        builder.Services.AddSearchClient();

        // Add mediator and commands to DI
        builder.Services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblyContaining<Program>();
            cfg.AddOpenBehavior(typeof(ValidatorBehavior<,>));
        });

        // Add queries to DI
        builder.Services.AddScoped<ICocktailQueries, CocktailQueries>();
        builder.Services.AddScoped<IHealthQueries, HealthQueries>();
        builder.Services.AddScoped<ILegalDocumentQueries, LegalDocumentQueries>();
        builder.Services.AddScoped<ILocalImagesQueries, LocalImagesQueries>();
        builder.Services.AddScoped<IAccountsQueries, AccountsQueries>();

        // Add validators for the MediatR validation pipeline behavior (validators based on FluentValidation library)
        builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

        // Add repositories to DI
        builder.Services.AddScoped<CocktailDataStore>();
        builder.Services.AddScoped<IngredientsDataStore>();
        builder.Services.AddScoped<ICocktailRepository, CocktailRepository>();
        builder.Services.AddScoped<IIngredientRepository, IngredientRepository>();
        builder.Services.AddScoped<IAccountRepository, AccountRepository>();
        builder.Services.AddScoped<IAccountCocktailRatingsRepository, AccountCocktailRatingsRepository>();
        builder.Services.AddScoped<LegalDataStore>();
        builder.Services.AddScoped<ILegalDocumentRepository, LegalDocumentRepository>();
        builder.Services.AddScoped<IHealthRepository, HealthRepository>();

        // add in recaptcha validation to DI
        builder.Services.UseRecaptcha(builder.Configuration);

        // add in infrastructure services
        builder.Services.AddTransient<IRequestHeaderAccessor, RequestHeaderAccessor>();
    }
}