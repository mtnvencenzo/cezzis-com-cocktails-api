namespace Cocktails.Api.Apis.App;

using Cocktails.Api.Application.Concerns.App.Commands;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Net;

/// <summary>
/// 
/// </summary>
public static class AppApi
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public static RouteGroupBuilder MapInitApiV1(this IEndpointRouteBuilder app)
    {
        var groupBuilder = app.MapGroup("/app")
            .AllowAnonymous();

        groupBuilder.MapGet("/initialize", PutInitialize)
            .WithName(nameof(PutInitialize))
            .WithDisplayName(nameof(PutInitialize));

        return groupBuilder;
    }

    /// <summary>Initializes the app</summary>
    /// <returns></returns>
    [ProducesResponseType((int)HttpStatusCode.NoContent, Type = typeof(void))]
    [ProducesDefaultResponseType(typeof(ProblemDetails))]
    public static NoContent PutInitialize([AsParameters] AppServices appServices)
    {
        var ping = appServices.Mediator.Send(new InitializeAppCommand());

        return TypedResults.NoContent();
    }
}