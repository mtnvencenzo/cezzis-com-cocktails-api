namespace Cocktails.Api.Apis.Jobs;

using Cocktails.Api.Domain.Config;
using MediatR;
using Microsoft.Extensions.Options;

/// <summary></summary>
/// <param name="mediator"></param>
/// <param name="cocktailsApiConfig"></param>
/// <param name="logger"></param>
public class JobsServices(
    IMediator mediator,
    IOptions<CocktailsApiConfig> cocktailsApiConfig,
    ILogger<JobsServices> logger)
{
    public IMediator Mediator { get; } = mediator ?? throw new ArgumentNullException(nameof(mediator));

    public IOptions<CocktailsApiConfig> CocktailsApiConfig { get; } = cocktailsApiConfig ?? throw new ArgumentNullException(nameof(cocktailsApiConfig));

    public ILogger<JobsServices> Logger { get; } = logger ?? throw new ArgumentNullException(nameof(logger));
}