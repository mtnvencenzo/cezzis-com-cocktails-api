namespace Cocktails.Api.Apis.Cockails;

using Cocktails.Api.Application.Concerns.Cocktails.Queries;
using MediatR;

public class CocktailsServices(
    IMediator mediator,
    ICocktailQueries queries,
    IHttpContextAccessor httpContextAccessor,
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
}