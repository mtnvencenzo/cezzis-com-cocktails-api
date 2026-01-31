namespace Cocktails.Api.Apis.Integrations;

using MediatR;

/// <summary></summary>
/// <param name="mediator"></param>
/// <param name="httpContextAccessor"></param>
/// <param name="logger"></param>
public class IntegrationsServices(
    IMediator mediator,
    IHttpContextAccessor httpContextAccessor,
    ILogger<IntegrationsServices> logger)
{
    public IMediator Mediator { get; } = mediator ?? throw new ArgumentNullException(nameof(mediator));

    public ILogger<IntegrationsServices> Logger { get; } = logger ?? throw new ArgumentNullException(nameof(logger));

    public IHttpContextAccessor HttpContextAccessor { get; } = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
}