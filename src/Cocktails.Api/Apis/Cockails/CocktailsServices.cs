namespace Cocktails.Api.Apis.Cockails;

using Cocktails.Api.Application.Concerns.Cocktails.Queries;
using Cocktails.Api.Domain.Config;
using MediatR;
using Microsoft.Extensions.Options;

public class CocktailsServices(
    IMediator mediator,
    ICocktailQueries queries,
    IHttpContextAccessor httpContextAccessor,
    IOptions<SearchConfig> searchConfig,
    ILogger<CocktailsServices> logger)
{
    /// <summary></summary>
    public IMediator Mediator { get; } = mediator ?? throw new ArgumentNullException(nameof(mediator));

    /// <summary></summary>
    public ILogger<CocktailsServices> Logger { get; } = logger ?? throw new ArgumentNullException(nameof(logger));

    /// <summary></summary>
    public ICocktailQueries Queries { get; } = queries ?? throw new ArgumentNullException(nameof(queries));

    /// <summary></summary>
    public IHttpContextAccessor HttpContextAccessor { get; } = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));

    /// <summary></summary>
    public IOptions<SearchConfig> SearchConfig { get; } = searchConfig ?? throw new ArgumentNullException(nameof(searchConfig));
}