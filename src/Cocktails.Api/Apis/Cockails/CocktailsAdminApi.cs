namespace Cocktails.Api.Apis.Cockails;

using Cocktails.Api.Application.Exceptions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Cocktails.Api.Application.Concerns.Cocktails.Commands;
using Cocktails.Api.Application.Behaviors.Authorization;
using Cocktails.Api.StartupExtensions;

/// <summary>Provides administrative endpoints for cocktail management, including batch publishing operations.</summary>
public static class CocktailsAdminApi
{
    /// <summary>
    /// Cocktails api v1 routes
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public static RouteGroupBuilder MapCocktailsAdminApiV1(this IEndpointRouteBuilder app)
    {
        var groupBuilder = app.MapGroup("/cocktails/admin")
            .RequireAuthorization()
            .WithTags("Cocktails Admin");

        groupBuilder.MapPut("/pub", PublishCocktails)
            .WithName(nameof(PublishCocktails))
            .WithDisplayName(nameof(PublishCocktails))
            .WithDescription("Publishes the cocktails to an event stream")
            .RequireScope(AuthScopes.AdminCezziCocktails);

        return groupBuilder;
    }

    /// <summary>Publishes the cocktails to an event stream</summary>
    /// <param name="cocktailsServices">The required services</param>
    /// <returns></returns>
    [ProducesDefaultResponseType(typeof(ProblemDetails))]
    public async static Task<Results<NoContent, JsonHttpResult<ProblemDetails>>> PublishCocktails([AsParameters] CocktailsServices cocktailsServices)
    {
        var commandResult = await cocktailsServices.Mediator.Send(new PublishCocktailsCommand(BatchItemCount: 1), cocktailsServices.HttpContextAccessor.HttpContext.RequestAborted);

        if (!commandResult)
        {
            return TypedResults.Json<ProblemDetails>(ProblemDetailsExtensions.CreateValidationProblemDetails("Failed to publish cocktails", StatusCodes.Status500InternalServerError), statusCode: StatusCodes.Status500InternalServerError);
        }

        return TypedResults.NoContent();
    }
}