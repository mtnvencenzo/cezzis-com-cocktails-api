namespace Cocktails.Api.Apis.Cockails;

using Cocktails.Api.Application.Exceptions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Cocktails.Api.Application.Concerns.Cocktails.Commands;
using Cocktails.Api.Application.Behaviors.Authorization;
using Cocktails.Api.StartupExtensions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Cocktails.Api.Application.Concerns.Cocktails.Models;

/// <summary>Provides administrative endpoints for cocktail management, including batch publishing operations.</summary>
public static class CocktailsAdminApi
{
    /// <summary>Cocktails admin api v1 routes</summary>
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
    public async static Task<Results<NoContent, JsonHttpResult<ProblemDetails>>> PublishCocktails(
        [FromBody, Required, Description("The request to publish cocktails to external systems")] PublishCocktailsRq request,
        [AsParameters] CocktailsServices cocktailsServices)
    {
        cocktailsServices.Logger.LogInformation("Initiating cocktails batch publish with {BatchItemCount} items", 1);

        var commandResult = await cocktailsServices.Mediator.Send(new PublishCocktailsCommand(
            BatchItemCount: 1,
            CocktailIds: [.. request.CocktailIds]
        ), cocktailsServices.HttpContextAccessor.HttpContext.RequestAborted);

        if (!commandResult)
        {
            cocktailsServices.Logger.LogError("Failed to publish cocktails batch");
            return TypedResults.Json(ProblemDetailsExtensions.CreateValidationProblemDetails("Failed to publish cocktails", StatusCodes.Status500InternalServerError), statusCode: StatusCodes.Status500InternalServerError);
        }

        cocktailsServices.Logger.LogInformation("Successfully published cocktails batch");
        return TypedResults.NoContent();
    }
}