namespace Cocktails.Api.Apis.App;

using MediatR;

/// <summary>
/// 
/// </summary>
/// <param name="mediator"></param>
/// <param name="logger"></param>
public class AppServices(IMediator mediator, ILogger<AppServices> logger)
{
    public ILogger<AppServices> Logger { get; } = logger;

    public IMediator Mediator { get; } = mediator;
}