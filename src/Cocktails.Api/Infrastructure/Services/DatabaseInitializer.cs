namespace Cocktails.Api.Infrastructure.Services;

using Cocktails.Api.Application.Concerns.Accounts.Commands;
using Cocktails.Api.Application.Concerns.Cocktails.Commands;
using Cocktails.Api.Domain.Config;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

public class DatabaseInitializer(
    IOptions<CosmosDbConfig> config,
    IMediator mediator,
    ILogger<DatabaseInitializer> logger)
{
    public async Task InitializeAsync(bool seedDataOnlyIfEmpty, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Seeding data into database {DatabaseName}", config.Value.DatabaseName);

            await mediator.Send(new SeedIngredientsCommand(OnlyIfEmpty: seedDataOnlyIfEmpty), cancellationToken);
            await mediator.Send(new SeedCocktailsCommand(OnlyIfEmpty: seedDataOnlyIfEmpty), cancellationToken);
            await mediator.Send(new SeedTestAccountCommand(), cancellationToken);

            logger.LogInformation("Database initialization completed successfully.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during database initialization");
            throw;
        }
    }
}