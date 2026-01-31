namespace Cocktails.Api.StartupExtensions;

using Cezzi.Security.Recaptcha;
using Cocktails.Api.Application.Behaviors.MediatRPipelines;
using Cocktails.Api.Application.Concerns.Cocktails.Queries;
using Cocktails.Api.Application.Concerns.Cocktails.Services;
using Cocktails.Api.Application.Concerns.Health.Queries;
using Cocktails.Api.Application.Concerns.LegalDocuments.Queries;
using Cocktails.Api.Application.Concerns.LocalImages.Queries;
using Cocktails.Api.Domain.Aggregates.CocktailAggregate;
using Cocktails.Api.Domain.Aggregates.HealthAggregate;
using Cocktails.Api.Domain.Aggregates.IngredientAggregate;
using Cocktails.Api.Domain.Aggregates.LegalDocumentAggregate;
using Cocktails.Api.Domain.Config;
using Cocktails.Api.Infrastructure;
using Cocktails.Api.Infrastructure.Repositories;
using Cocktails.Api.Infrastructure.Services;
using FluentValidation;

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
        builder.Services.Configure<LocalhostImagesConfig>(builder.Configuration.GetSection(LocalhostImagesConfig.SectionName));
        builder.Services.Configure<CosmosDbConfig>(builder.Configuration.GetSection(CosmosDbConfig.SectionName));
        builder.Services.Configure<Auth0Config>(builder.Configuration.GetSection(Auth0Config.SectionName));
        builder.Services.Configure<KafkaConfig>(builder.Configuration.GetSection(KafkaConfig.SectionName));
        builder.Services.Configure<DaprConfig>(builder.Configuration.GetSection(DaprConfig.SectionName));

        builder.Services.AddCosomsContexts();

        // add the authentication and authorization services to DI
        builder.Services.AddDefaultAuthentication(builder.Configuration);
        builder.Services.AddDefaultAuthorization();

        // Add dapr client to DI
        builder.Services.AddDaprClient();

        // Add dapr serice bus messaging to DI
        builder.Services.AddEventBus(builder.Configuration);

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
        builder.Services.AddScoped<ICocktailModelConverter, CocktailModelConverter>();

        // Add validators for the MediatR validation pipeline behavior (validators based on FluentValidation library)
        builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

        // Add repositories to DI
        builder.Services.AddScoped<CocktailDataStore>();
        builder.Services.AddScoped<IngredientsDataStore>();
        builder.Services.AddScoped<ICocktailRepository, CocktailRepository>();
        builder.Services.AddScoped<IIngredientRepository, IngredientRepository>();
        builder.Services.AddScoped<LegalDataStore>();
        builder.Services.AddScoped<ILegalDocumentRepository, LegalDocumentRepository>();
        builder.Services.AddScoped<IHealthRepository, HealthRepository>();

        // add in recaptcha validation to DI
        builder.Services.UseRecaptcha(builder.Configuration);

        // add in infrastructure services to DI
        builder.Services.AddTransient<IRequestHeaderAccessor, RequestHeaderAccessor>();
    }
}