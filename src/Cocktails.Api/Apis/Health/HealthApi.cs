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
            .ExcludeFromDescription()
            .AllowAnonymous();

        groupBuilder.MapGet("/ping", GetPing)
            .WithName(nameof(GetPing))
            .WithDisplayName(nameof(GetPing));

        groupBuilder.MapGet("/version", GetVersion)
            .WithName(nameof(GetVersion))
            .WithDisplayName(nameof(GetVersion));

        groupBuilder.MapGet("/liveness", GetLiveness)
            .WithName(nameof(GetLiveness))
            .WithDisplayName(nameof(GetLiveness));

        groupBuilder.MapGet("/readiness", GetReadiness)
            .WithName(nameof(GetReadiness))
            .WithDisplayName(nameof(GetReadiness));

        return groupBuilder;
    }

    /// <summary>Pings the cocktails api</summary>
    /// <returns></returns>
    [ApiExplorerSettings(IgnoreApi = true)]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(PingRs))]
    [ProducesDefaultResponseType(typeof(ProblemDetails))]
    public static Ok<PingRs> GetPing([AsParameters] HealthServices healthServices)
    {
        var ping = healthServices.Queries.GetPing();

        return TypedResults.Ok(ping);
    }

    /// <summary>Gets the current cocktails api version</summary>
    /// <returns></returns>
    [ApiExplorerSettings(IgnoreApi = true)]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(VersionRs))]
    [ProducesDefaultResponseType(typeof(ProblemDetails))]
    public static Ok<VersionRs> GetVersion([AsParameters] HealthServices healthServices)
    {
        var version = healthServices.Queries.GetVersion();

        return TypedResults.Ok(version);
    }

    /// <summary>Performs a liveness check of the API</summary>
    /// <returns></returns>
    [ApiExplorerSettings(IgnoreApi = true)]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(HealthCheckRs))]
    [ProducesDefaultResponseType(typeof(ProblemDetails))]
    public static Ok<HealthCheckRs> GetLiveness([AsParameters] HealthServices healthServices)
    {
        var result = healthServices.Queries.GetLiveness();

        return TypedResults.Ok(result);
    }

    /// <summary>Performs a readiness check verifying connectivity to Dapr components and CosmosDB</summary>
    /// <returns></returns>
    [ApiExplorerSettings(IgnoreApi = true)]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(HealthCheckRs))]
    [ProducesResponseType((int)HttpStatusCode.ServiceUnavailable, Type = typeof(HealthCheckRs))]
    [ProducesDefaultResponseType(typeof(ProblemDetails))]
    public static async Task<Results<Ok<HealthCheckRs>, JsonHttpResult<HealthCheckRs>>> GetReadiness([AsParameters] HealthServices healthServices)
    {
        var result = await healthServices.Queries.GetReadinessAsync();

        if (result.Status != "healthy")
        {
            return TypedResults.Json(result, statusCode: StatusCodes.Status503ServiceUnavailable);
        }

        return TypedResults.Ok(result);
    }
}