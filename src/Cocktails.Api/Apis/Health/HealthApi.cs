namespace Cocktails.Api.Apis.Health;

using Cocktails.Api.Application.Concerns.Health.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Net;

/// <summary>
/// 
/// </summary>
public static class HealthApi
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public static RouteGroupBuilder MapHealthApiV1(this IEndpointRouteBuilder app)
    {
        var groupBuilder = app.MapGroup("/health")
            .WithName(nameof(GetPing))
            .ExcludeFromDescription()
            .AllowAnonymous();

        groupBuilder.MapGet("/ping", GetPing)
            .WithName(nameof(GetPing))
            .WithDisplayName(nameof(GetPing));

        return groupBuilder;
    }

    /// <summary>Gets the cocktails open API spec.</summary>
    /// <returns></returns>
    [ApiExplorerSettings(IgnoreApi = true)]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(PingRs))]
    [ProducesDefaultResponseType(typeof(ProblemDetails))]
    public static Ok<PingRs> GetPing([AsParameters] HealthServices healthServices)
    {
        var ping = healthServices.Queries.GetPing();

        return TypedResults.Ok(ping);
    }
}