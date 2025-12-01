namespace Cocktails.Api.Application.Concerns.App.Commands;

using global::Cocktails.Api.Infrastructure.Services;
using MediatR;

public record InitializeAppCommand(bool OnlyIfEmpty = false) : IRequest<bool>;

public class InitializeAppCommandHandler(
    StorageInitializer storageInitializer,
    DatabaseInitializer databaseInitializer,
    ILogger<InitializeAppCommandHandler> logger) : IRequestHandler<InitializeAppCommand, bool>
{
    public async Task<bool> Handle(InitializeAppCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("Initializing application");

        logger.LogInformation("Initializing Storage");
        await storageInitializer.InitializeAsync();

        // logger.LogInformation("Initializing Kafka");
        // await kafkaInitializer.InitializeAsync();

        logger.LogInformation("Initializing Database");
        await databaseInitializer.InitializeAsync();

        return true;
    }
}