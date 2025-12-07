namespace Cocktails.Api.Apis.Jobs;

using MediatR;

/// <summary></summary>
/// <param name="mediator"></param>
/// <param name="logger"></param>
public class JobsServices(
    IMediator mediator,
    ILogger<JobsServices> logger)
{
    public IMediator Mediator { get; } = mediator ?? throw new ArgumentNullException(nameof(mediator));

    public ILogger<JobsServices> Logger { get; } = logger ?? throw new ArgumentNullException(nameof(logger));
}