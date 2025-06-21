namespace Cocktails.Api.Apis.Accounts;

using Cocktails.Api.Application.Concerns.Accounts.Commands;
using Cocktails.Api.Application.Concerns.Accounts.Models;
using Cocktails.Api.Application.Concerns.Cocktails.Commands;
using Cocktails.Api.Application.Exceptions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using Microsoft.Identity.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Security.Claims;

/// <summary>
/// 
/// </summary>
public static class AccountsApi
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public static RouteGroupBuilder MapAccountsApiV1(this IEndpointRouteBuilder app)
    {
        var groupBuilder = app.MapGroup("/accounts")
            .RequireAuthorization()
            .WithTags("Accounts");

        groupBuilder.MapGet("/owned/profile", GetAccountOwnedProfile)
            .WithName(nameof(GetAccountOwnedProfile))
            .WithDisplayName(nameof(GetAccountOwnedProfile))
            .WithDescription("Gets the account profile for the user represented within the authenticated bearer token")
            .RequireScope("Account.Read");

        groupBuilder.MapPut("/owned/profile", UpdateAccountOwnedProfile)
            .WithName(nameof(UpdateAccountOwnedProfile))
            .WithDisplayName(nameof(UpdateAccountOwnedProfile))
            .WithDescription("Updates the account profile for the user represented within the authenticated bearer token")
            .RequireScope("Account.Read")
            .RequireScope("Account.Write");

        groupBuilder.MapPut("/owned/profile/email", UpdateAccountOwnedProfileEmail)
            .WithName(nameof(UpdateAccountOwnedProfileEmail))
            .WithDisplayName(nameof(UpdateAccountOwnedProfileEmail))
            .WithDescription("Updates the account profile email address for the user represented within the authenticated bearer token")
            .RequireScope("Account.Read")
            .RequireScope("Account.Write");

        groupBuilder.MapPut("/owned/profile/accessibility", UpdateAccountOwnedAccessibilitySettings)
            .WithName(nameof(UpdateAccountOwnedAccessibilitySettings))
            .WithDisplayName(nameof(UpdateAccountOwnedAccessibilitySettings))
            .WithDescription("Updates the account profile accessibility settings for the user represented within the authenticated bearer token")
            .RequireScope("Account.Read")
            .RequireScope("Account.Write");

        groupBuilder.MapPost("/owned/profile/image", UploadProfileImage)
            .WithName(nameof(UploadProfileImage))
            .WithDisplayName(nameof(UploadProfileImage))
            .WithDescription("Uploads an account profile image for to the user represented within the authenticated bearer token")
            .RequireScope("Account.Read")
            .RequireScope("Account.Write")
            .DisableAntiforgery();

        groupBuilder.MapPut("/owned/profile/cocktails/favorites", ManageFavoriteCocktails)
            .WithName(nameof(ManageFavoriteCocktails))
            .WithDisplayName(nameof(ManageFavoriteCocktails))
            .WithDescription("Manages the account profile favorite cocktails list for the user represented within the authenticated bearer token")
            .RequireScope("Account.Read")
            .RequireScope("Account.Write");

        groupBuilder.MapPost("/owned/profile/cocktails/ratings", RateCocktail)
            .WithName(nameof(RateCocktail))
            .WithDisplayName(nameof(RateCocktail))
            .WithDescription("Rates a cocktail for the account profile user represented within the authenticated bearer token")
            .RequireScope("Account.Read")
            .RequireScope("Account.Write");

        groupBuilder.MapGet("/owned/profile/cocktails/ratings", GetCocktailRatings)
            .WithName(nameof(GetCocktailRatings))
            .WithDisplayName(nameof(GetCocktailRatings))
            .WithDescription("Gets the account cocktail ratings for the account profile user represented within the authenticated bearer token")
            .RequireScope("Account.Read");

        groupBuilder.MapPost("/owned/profile/cocktails/recommendations", SendCocktailRecommendation)
            .WithName(nameof(SendCocktailRecommendation))
            .WithDisplayName(nameof(SendCocktailRecommendation))
            .WithDescription("Sends a cocktail recommendation for review")
            .RequireScope("Account.Read")
            .RequireScope("Account.Write");

        return groupBuilder;
    }

    /// <summary>Gets the account profile for the user represented within the authenticated bearer token</summary>
    /// <returns></returns>
    [ProducesDefaultResponseType(typeof(ProblemDetails))]
    public async static Task<Results<Ok<AccountOwnedProfileRs>, JsonHttpResult<ProblemDetails>>> GetAccountOwnedProfile([AsParameters] AccountsServices accountServices)
    {
        var rs = await accountServices.Queries.GetAccountOwnedProfile(accountServices.HttpContextAccessor.HttpContext);

        return TypedResults.Ok(rs);
    }

    /// <summary>Uploads an account profile image for to the user represented within the authenticated bearer token</summary>
    /// <returns></returns>
    [ProducesDefaultResponseType(typeof(ProblemDetails))]
    public async static Task<Results<Created<UploadProfileImageRs>, JsonHttpResult<ProblemDetails>>> UploadProfileImage(IFormFile file, [AsParameters] AccountsServices accountServices)
    {
        var command = new ProfileImageUploadCommand(file, accountServices.HttpContextAccessor.HttpContext?.User?.Identity as ClaimsIdentity);

        var result = await accountServices.Mediator.Send(command);

        return TypedResults.Created<UploadProfileImageRs>(result.ImageUri, result);
    }

    /// <summary>Updates the account profile for the user represented within the authenticated bearer token</summary>
    /// <param name="request">The account profile information to update</param>
    /// <returns></returns>
    [ProducesDefaultResponseType(typeof(ProblemDetails))]
    public async static Task<Results<Ok<AccountOwnedProfileRs>, JsonHttpResult<ProblemDetails>>> UpdateAccountOwnedProfile([FromBody] UpdateAccountOwnedProfileRq request, [AsParameters] AccountsServices accountServices)
    {
        var command = new UpdateAccountOwnedProfileCommand(request, accountServices.HttpContextAccessor.HttpContext?.User?.Identity as ClaimsIdentity);

        var result = await accountServices.Mediator.Send(command);

        return TypedResults.Ok<AccountOwnedProfileRs>(result);
    }

    /// <summary>Updates the account profile email address for the user represented within the authenticated bearer token</summary>
    /// <param name="request">The account profile email information to update</param>
    /// <returns></returns>
    [ProducesDefaultResponseType(typeof(ProblemDetails))]
    public async static Task<Results<Ok<AccountOwnedProfileRs>, JsonHttpResult<ProblemDetails>>> UpdateAccountOwnedProfileEmail([FromBody] UpdateAccountOwnedProfileEmailRq request, [AsParameters] AccountsServices accountServices)
    {
        var command = new UpdateAccountOwnedProfileEmailCommand(request, accountServices.HttpContextAccessor.HttpContext?.User?.Identity as ClaimsIdentity);

        var result = await accountServices.Mediator.Send(command);

        return TypedResults.Ok<AccountOwnedProfileRs>(result);
    }

    /// <summary>Updates the account profile accessibility settings for the user represented within the authenticated bearer token</summary>
    /// <param name="request">The account profile accessibility settings to update</param>
    /// <returns></returns>
    [ProducesDefaultResponseType(typeof(ProblemDetails))]
    public async static Task<Results<Ok<AccountOwnedProfileRs>, JsonHttpResult<ProblemDetails>>> UpdateAccountOwnedAccessibilitySettings([FromBody] UpdateAccountOwnedAccessibilitySettingsRq request, [AsParameters] AccountsServices accountServices)
    {
        var command = new UpdateAccountOwnedAccessibilitySettingsCommand(request, accountServices.HttpContextAccessor.HttpContext?.User?.Identity as ClaimsIdentity);

        var result = await accountServices.Mediator.Send(command);

        return TypedResults.Ok<AccountOwnedProfileRs>(result);
    }

    /// <summary>Manages the account profile favorite cocktails for the user represented within the authenticated bearer token</summary>
    /// <param name="request">The cocktails to add or remove from the faviorites list</param>
    /// <returns></returns>
    [ProducesDefaultResponseType(typeof(ProblemDetails))]
    public async static Task<Results<Ok<AccountOwnedProfileRs>, JsonHttpResult<ProblemDetails>>> ManageFavoriteCocktails([FromBody] ManageFavoriteCocktailsRq request, [AsParameters] AccountsServices accountServices)
    {
        var command = new ManageFavoriteCocktailsCommand(request, accountServices.HttpContextAccessor.HttpContext?.User?.Identity as ClaimsIdentity);

        var result = await accountServices.Mediator.Send(command);

        return TypedResults.Ok<AccountOwnedProfileRs>(result);
    }

    /// <summary>Rates a cocktail for the account profile user represented within the authenticated bearer token</summary>
    /// <param name="request">The cocktails and rating request</param>
    /// <returns></returns>
    [ProducesDefaultResponseType(typeof(ProblemDetails))]
    public async static Task<Results<Created<RateCocktailRs>, JsonHttpResult<ProblemDetails>, Conflict<ProblemDetails>>> RateCocktail([FromBody] RateCocktailRq request, [AsParameters] AccountsServices accountServices)
    {
        var command = new RateCocktailCommand(
            CocktailId: request.CocktailId,
            Stars: request.Stars,
            Identity: accountServices.HttpContextAccessor.HttpContext?.User?.Identity as ClaimsIdentity);

        var result = await accountServices.Mediator.Send(command);

        if (result == null)
        {
            return TypedResults.Conflict<ProblemDetails>(ProblemDetailsExtensions.CreateValidationProblemDetails("Cocktail already rated", 409));
        }

        return TypedResults.Created<RateCocktailRs>("", result);
    }

    /// <summary>Gets the account cocktail ratings for the account profile user represented within the authenticated bearer token</summary>
    /// <returns></returns>
    [ProducesDefaultResponseType(typeof(ProblemDetails))]
    public async static Task<Results<Ok<AccountCocktailRatingsRs>, JsonHttpResult<ProblemDetails>>> GetCocktailRatings([AsParameters] AccountsServices accountServices)
    {
        var rs = await accountServices.Queries.GetAccountOwnedCocktailRatings(accountServices.HttpContextAccessor.HttpContext);

        return TypedResults.Ok(rs);
    }

    /// <summary>Sends a cocktail recommendation for review</summary>
    /// <param name="request">The request.</param>
    /// <param name="accountServices">The required services</param>
    /// <returns></returns>`
    [ProducesDefaultResponseType(typeof(ProblemDetails))]
    public async static Task<Results<Accepted, JsonHttpResult<ProblemDetails>>> SendCocktailRecommendation(
        [FromBody, Required, Description("The cocktail recommendation request body")] CocktailRecommendationRq request,
        [AsParameters] AccountsServices accountServices)
    {
        var command = new CocktailRecommendationCommand(request.Recommendation, request.VerificationCode);
        var commandResult = await accountServices.Mediator.Send(command, accountServices.HttpContextAccessor.HttpContext.RequestAborted);

        if (!commandResult)
        {
            return TypedResults.Json<ProblemDetails>(ProblemDetailsExtensions.CreateValidationProblemDetails("Failed to send recommendation", StatusCodes.Status502BadGateway), statusCode: StatusCodes.Status502BadGateway);
        }

        return TypedResults.Accepted(string.Empty);
    }

}