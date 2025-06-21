namespace Cocktails.Api.StartupExtensions;

using Azure.Identity;
using Cocktails.Api.Domain.Config;
using Cocktails.Api.Infrastructure;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;

internal static class CosmosExtensions
{
    internal static IServiceCollection AddCosomsContexts(this IServiceCollection services)
    {
        services.AddDbContext<AccountDbContext>();
        services.AddDbContext<CocktailDbContext>();
        services.AddScoped<DatabaseInitializer>();

        return services;
    }

    private static IServiceCollection AddDbContext<TContext>(this IServiceCollection services)
        where TContext : DbContext
    {
        services.AddDbContext<TContext>((sp, optionsBuilder) =>
        {
            var options = sp.GetRequiredService<IOptions<CosmosDbConfig>>().Value;

            if (!string.IsNullOrWhiteSpace(options.ConnectionString))
            {
                // -------------------------------------------------------------------------------------
                // These options are for development purposes only. Since not planning 
                // on using connection strings in real environments, changing the options to work with local 
                // cosmos emulator.  (Note: disabling cert checks!)
                // -------------------------------------------------------------------------------------
                optionsBuilder.UseCosmos(
                    connectionString: options.ConnectionString,
                    databaseName: options.DatabaseName,
                    cosmosOptionsAction: options =>
                    {
                        options.ConnectionMode(Microsoft.Azure.Cosmos.ConnectionMode.Gateway);
                        options.LimitToEndpoint(true);
                        options.HttpClientFactory(() =>
                        {
                            HttpMessageHandler httpMessageHandler = new HttpClientHandler()
                            {
                                ServerCertificateCustomValidationCallback = (req, cert, chain, errors) => true
                            };

                            return new HttpClient(httpMessageHandler);
                        });
                    });
            }
            else
            {
                optionsBuilder.UseCosmos(
                    accountEndpoint: options.AccountEndpoint,
                    tokenCredential: new DefaultAzureCredential(),
                    databaseName: options.DatabaseName,
                    cosmosOptionsAction: options =>
                    {
                        options.ConnectionMode(Microsoft.Azure.Cosmos.ConnectionMode.Direct);
                        options.MaxRequestsPerTcpConnection(16);
                        options.MaxTcpConnectionsPerEndpoint(32);
                    });
            }
        },
        contextLifetime: ServiceLifetime.Scoped,
        optionsLifetime: ServiceLifetime.Singleton);

        return services;
    }
}
