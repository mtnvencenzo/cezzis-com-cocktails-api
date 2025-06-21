namespace Cocktails.Api.Infrastructure;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Cosmos;
using Cocktails.Api.Domain.Config;

public class DatabaseInitializer(
    IOptions<CosmosDbConfig> config,
    AccountDbContext accountDbContext,
    CocktailDbContext cocktailDbContext,
    ILogger<DatabaseInitializer> logger)
{
    public async Task InitializeAsync()
    {
        try
        {
            logger.LogInformation("Starting database initialization for database: {DatabaseName}", config.Value.DatabaseName);

            var cocktailCosmosClient = cocktailDbContext.Database.GetCosmosClient();
            var accountCosmosClient = accountDbContext.Database.GetCosmosClient();

            // Create database if it doesn't exist
            try
            {
                logger.LogInformation("Creating database if it doesn't exist...");
                var cocktailDatabase = await cocktailCosmosClient.CreateDatabaseIfNotExistsAsync(config.Value.DatabaseName);
                logger.LogInformation("Database created/verified: {DatabaseId}", cocktailDatabase.Database.Id);

                logger.LogInformation("Creating database if it doesn't exist...");
                var accountDatabase = await accountCosmosClient.CreateDatabaseIfNotExistsAsync(config.Value.DatabaseName);
                logger.LogInformation("Database created/verified: {DatabaseId}", accountDatabase.Database.Id);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error creating database: {Message}", ex.Message);
                throw;
            }

            // Create containers if they don't exist
            logger.LogInformation("Creating containers if they don't exist...");

            try
            {
                var accountContainer = await accountCosmosClient
                    .GetDatabase(config.Value.DatabaseName)
                    .CreateContainerIfNotExistsAsync(new ContainerProperties
                    {
                        Id = "accounts-account",
                        PartitionKeyPath = "/subjectId",
                        PartitionKeyDefinitionVersion = PartitionKeyDefinitionVersion.V2
                    });
                logger.LogInformation("Account container created/verified: {ContainerId}", accountContainer.Container.Id);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error creating account container: {Message}", ex.Message);
                throw;
            }

            try
            {
                var cocktailContainer = await cocktailCosmosClient
                    .GetDatabase(config.Value.DatabaseName)
                    .CreateContainerIfNotExistsAsync(new ContainerProperties
                    {
                        Id = "cocktails-cocktail",
                        PartitionKeyPath = "/id",
                        PartitionKeyDefinitionVersion = PartitionKeyDefinitionVersion.V2
                    });
                logger.LogInformation("Cocktail container created/verified: {ContainerId}", cocktailContainer.Container.Id);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error creating cocktail container: {Message}", ex.Message);
                throw;
            }

            try
            {
                var ingredientContainer = await cocktailCosmosClient
                    .GetDatabase(config.Value.DatabaseName)
                    .CreateContainerIfNotExistsAsync(new ContainerProperties
                    {
                        Id = "cocktails-ingredient",
                        PartitionKeyPath = "/id",
                        PartitionKeyDefinitionVersion = PartitionKeyDefinitionVersion.V2
                    });
                logger.LogInformation("Ingredient container created/verified: {ContainerId}", ingredientContainer.Container.Id);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error creating ingredient container: {Message}", ex.Message);
                throw;
            }

            logger.LogInformation("Database initialization completed successfully.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during database initialization");
            throw;
        }
    }
}