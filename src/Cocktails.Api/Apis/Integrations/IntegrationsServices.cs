namespace Cocktails.Api.Apis.Integrations;

using Cocktails.Api.Infrastructure.Services;
using MediatR;

/// <summary></summary>
/// <param name="mediator"></param>
/// <param name="httpContextAccessor"></param>
/// <param name="auth0ManagementClient"></param>
/// <param name="logger"></param>
public class IntegrationsServices(
    IMediator mediator,
    IHttpContextAccessor httpContextAccessor,
    IAuth0ManagementClient auth0ManagementClient,
    ILogger<IntegrationsServices> logger)
{
    public IMediator Mediator { get; } = mediator ?? throw new ArgumentNullException(nameof(mediator));

    public ILogger<IntegrationsServices> Logger { get; } = logger ?? throw new ArgumentNullException(nameof(logger));

    public IHttpContextAccessor HttpContextAccessor { get; } = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));

    public IAuth0ManagementClient Auth0ManagementClient { get; } = auth0ManagementClient ?? throw new ArgumentNullException(nameof(auth0ManagementClient));
}