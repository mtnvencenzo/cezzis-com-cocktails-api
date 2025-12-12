namespace Cocktails.Api.Application.Concerns.App.Commands;

using global::Cocktails.Api.Infrastructure.Services;
using MediatR;

public record InitializeAppCommand(bool SeedDataOnlyIfEmpty = false) : IRequest<bool>;

public class InitializeAppCommandHandler(
    DatabaseInitializer databaseInitializer,
    ILogger<InitializeAppCommandHandler> logger) : IRequestHandler<InitializeAppCommand, bool>
{
    public async Task<bool> Handle(InitializeAppCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("Initializing application");

        logger.LogInformation("Initializing Database");
        await databaseInitializer.InitializeAsync(command.SeedDataOnlyIfEmpty, cancellationToken);

        return true;
    }
}