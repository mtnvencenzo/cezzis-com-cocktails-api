namespace Cocktails.Api.Apis.Jobs;

using Cocktails.Api.Application.Behaviors.DaprAppTokenAuthorization;
using Cocktails.Api.Application.Concerns.App.Commands;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Threading;

public static class JobsApi
{
    public static RouteGroupBuilder MapJobsApi(this IEndpointRouteBuilder app)
    {
        // The `/job` path is required by dapr scheduler
        // and must be used from the rooot of the application (no versioning)
        var groupBuilder = app.MapGroup("/job")
            .WithTags("Jobs")
            .RequireAuthorization(DaprAppTokenRequirement.PolicyName);

        groupBuilder.MapPost("/initialize-app", Init)
            .WithName(nameof(Init))
            .WithDisplayName(nameof(Init))
            .WithDescription("Initialize the application");

        return groupBuilder;
    }

    /// <summary>Initialize the application</summary>
    /// <param name="jobServices"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async static Task<Results<Ok, JsonHttpResult<ProblemDetails>>> Init(
        [AsParameters] JobsServices jobsServices,
        CancellationToken cancellationToken)
    {
        _ = await jobsServices.Mediator.Send(
            request: new InitializeAppCommand(SeedDataOnlyIfEmpty: true, CreateObjects: true),
            cancellationToken: cancellationToken);

        return TypedResults.Ok();
    }
}
