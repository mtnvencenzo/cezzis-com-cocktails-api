namespace Cocktails.Api.Infrastructure.Services;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Cosmos;
using Cocktails.Api.Domain.Config;
using MediatR;
using Cocktails.Api.Application.Concerns.Cocktails.Commands;
using Cocktails.Api.Application.Concerns.Accounts.Commands;

public class DatabaseInitializer(
    IOptions<CosmosDbConfig> config,
    AccountDbContext accountDbContext,
    CocktailDbContext cocktailDbContext,
    IMediator mediator,
    ILogger<DatabaseInitializer> logger)
{
    public async Task InitializeAsync()
    {
        try
        {
            logger.LogInformation("Starting database initialization for database: {DatabaseName}", config.Value.DatabaseName);

            var cocktailCosmosClient = cocktailDbContext.Database.GetCosmosClient();
            var accountCosmosClient = accountDbContext.Database.GetCosmosClient();

            try
            {
                var cocktailDatabase = await cocktailCosmosClient.CreateDatabaseIfNotExistsAsync(config.Value.DatabaseName);
                logger.LogInformation("Database created/verified: {DatabaseId}", cocktailDatabase.Database.Id);

                var accountDatabase = await accountCosmosClient.CreateDatabaseIfNotExistsAsync(config.Value.DatabaseName);
                logger.LogInformation("Database created/verified: {DatabaseId}", accountDatabase.Database.Id);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error creating database: {Message}", ex.Message);
                throw;
            }

            logger.LogInformation("Creating containers if they don't exist");
            await this.CreateContainer(accountCosmosClient, config.Value.DatabaseName, "accounts-account", "/subjectId");
            await this.CreateContainer(cocktailCosmosClient, config.Value.DatabaseName, "cocktails-cocktail", "/id");
            await this.CreateContainer(cocktailCosmosClient, config.Value.DatabaseName, "cocktails-ingredient", "/id");

            logger.LogInformation("Seeding data");
            await mediator.Send(new SeedIngredientsCommand(OnlyIfEmpty: true));
            await mediator.Send(new SeedCocktailsCommand(OnlyIfEmpty: true));
            await mediator.Send(new SeedTestAccountCommand());

            logger.LogInformation("Database initialization completed successfully.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during database initialization");
            throw;
        }
    }

    private async Task CreateContainer(CosmosClient cosmosClient, string databaseName, string containerId, string partitionKeyPath)
    {
        try
        {
            var container = await cosmosClient
                .GetDatabase(databaseName)
                .CreateContainerIfNotExistsAsync(new ContainerProperties
                {
                    Id = containerId,
                    PartitionKeyPath = partitionKeyPath,
                    PartitionKeyDefinitionVersion = PartitionKeyDefinitionVersion.V2
                });
            logger.LogInformation("Container created/verified: {ContainerId}", container.Container.Id);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating container: {ContainerId} {Message}", containerId, ex.Message);
            throw;
        }
    }
}