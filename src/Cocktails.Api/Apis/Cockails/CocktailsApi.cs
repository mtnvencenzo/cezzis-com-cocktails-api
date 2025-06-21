namespace Cocktails.Api.Apis.Cockails;

using Cocktails.Api.Application.Exceptions;
//using Cocktails.Api.Application.Filters;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Cocktails.Api.Application.Concerns.Cocktails.Commands;
using Cocktails.Api.Application.Concerns.Cocktails.Models;

/// <summary>
/// 
/// </summary>
public static class CocktailsApi
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public static RouteGroupBuilder MapCocktailsApiV1(this IEndpointRouteBuilder app)
    {
        var groupBuilder = app.MapGroup("/cocktails")
            .WithTags("Cocktails")
            .AllowAnonymous();

        groupBuilder.MapGet("/", GetCocktailsList)
            .WithName(nameof(GetCocktailsList))
            .WithDisplayName(nameof(GetCocktailsList))
            .WithDescription("Search the cocktail recipes");

        groupBuilder.MapPut("/", SeedCocktails)
            .WithName(nameof(SeedCocktails))
            .WithDisplayName(nameof(SeedCocktails))
            .WithDescription("Seeds the cocktails in the database");

        groupBuilder.MapGet("/{id}", GetCocktail)
            .WithName(nameof(GetCocktail))
            .WithDisplayName(nameof(GetCocktail))
            .WithDescription("Get a specific cocktail recipe");

        groupBuilder.MapGet("/ingredients/filters", GetCocktailIngredientFilters)
            .WithName(nameof(GetCocktailIngredientFilters))
            .WithDisplayName(nameof(GetCocktailIngredientFilters))
            .WithDescription("Get the available cocktails filters to be used while performing a search");

        groupBuilder.MapPut("/ingredients", SeedIngredients)
            .WithName(nameof(SeedIngredients))
            .WithDisplayName(nameof(SeedIngredients))
            .WithDescription("Seeds the ingredients in the database");

        groupBuilder.MapGet("/seo/sitemap", GetCocktailsSiteMap)
            .WithName(nameof(GetCocktailsSiteMap))
            .WithDisplayName(nameof(GetCocktailsSiteMap))
            .ExcludeFromDescription()
            .WithDescription("Gets the cocktails list site map");

        groupBuilder.MapGet("/seo/indexnow", GetCocktailsIndexNow)
            .WithName(nameof(GetCocktailsIndexNow))
            .WithDisplayName(nameof(GetCocktailsIndexNow))
            .ExcludeFromDescription()
            .WithDescription("Gets the cocktails indexnow result");

        return groupBuilder;
    }

    /// <summary>Get a specific cocktail recipe</summary>
    /// <param name="id">The cocktail recipe unique identifier</param>
    /// <param name="cocktailsServices"></param>
    /// <returns></returns>
    [ProducesDefaultResponseType(typeof(ProblemDetails))]
    public async static Task<Results<Ok<CocktailRs>, JsonHttpResult<ProblemDetails>>> GetCocktail(
        [FromRoute, Description("The cocktail recipe unique identifier")] string id,
        [AsParameters] CocktailsServices cocktailsServices)
    {
        var item = await cocktailsServices.Queries.GetCocktail(id, cocktailsServices.HttpContextAccessor.HttpContext.RequestAborted);

        if (item is not null)
        {
            return TypedResults.Ok(item);
        }

        return TypedResults.Json<ProblemDetails>(ProblemDetailsExtensions.CreateValidationProblemDetails("Cocktail receipe not found", StatusCodes.Status404NotFound), statusCode: StatusCodes.Status404NotFound);
    }

    /// <summary>Searches the cocktail recipes</summary>
    /// <param name="freeText">The free text search term to match against</param>
    /// <param name="skip">The number of cocktail recipes to skip from the paged response</param>
    /// <param name="take">The number of cocktail recipes to return</param>
    /// <param name="cocktailsServices">The required services</param>
    /// <param name="matches">A list of cocktail ids that can be included in the list</param>
    /// <param name="matchExclusive">Whether or not the supplied matches must be exclusively returned</param>
    /// <param name="include">The list of extension objects to include for each cocktail recipe</param>
    /// <param name="filters">An optional list of filters to use when quering the cocktail recipes</param>
    /// <returns></returns>
    [ProducesDefaultResponseType(typeof(ProblemDetails))]
    public async static Task<Results<Ok<CocktailsListRs>, JsonHttpResult<ProblemDetails>>> GetCocktailsList(
        [FromQuery, Description("The free text search term to match against")] string freeText,
        [FromQuery, Description("The number of cocktail recipes to skip from the paged response")] int? skip,
        [FromQuery, Description("The number of cocktail recipes to return")] int? take,
        [AsParameters] CocktailsServices cocktailsServices,
        [FromQuery(Name = "m"), Description("A list of cocktails that can be included in the list")] string[] matches = null,
        [FromQuery(Name = "match-exclusive"), Description("Whether or not the supplied matches must be exclusively returned")] bool? matchExclusive = false,
        [FromQuery(Name = "inc"), Description("The list of extension objects to include for each cocktail recipe")] CocktailDataIncludeModel[] include = null,
        [FromQuery(Name = "sf"), Description("An optional list of filters to use when quering the cocktail recipes")] string[] filters = null)
    {
        var rs = await cocktailsServices.Queries.GetCocktailsList(
            freeText: freeText ?? string.Empty,
            skip: skip ?? 0,
            take: take ?? 20,
            false,
            filters: filters?.ToList() ?? [],
            include: include,
            matches: matches,
            matchExclusive: matchExclusive ?? false,
            useSearchIndex: cocktailsServices.SearchConfig.Value.UseSearchIndex,
            cancellationToken: cocktailsServices.HttpContextAccessor.HttpContext.RequestAborted);

        return TypedResults.Ok(rs);
    }

    /// <summary>Get the available cocktail ingredient filters to be used while performing a search</summary>
    /// <param name="cocktailsServices">The required services</param>
    /// <returns></returns>
    [ProducesDefaultResponseType(typeof(ProblemDetails))]
    public async static Task<Results<Ok<CocktailIngredientFiltersRs>, JsonHttpResult<ProblemDetails>>> GetCocktailIngredientFilters([AsParameters] CocktailsServices cocktailsServices)
    {
        var rs = await cocktailsServices.Queries.GetCocktailIngredientFilters(cocktailsServices.HttpContextAccessor.HttpContext.RequestAborted);

        return TypedResults.Ok(rs);
    }

    /// <summary>Seeds the ingredients in the database</summary>
    /// <param name="request">The request.</param>
    /// <param name="cocktailsServices">The required services</param>
    /// <returns></returns>
    [ProducesDefaultResponseType(typeof(ProblemDetails))]
    public async static Task<Results<NoContent, JsonHttpResult<ProblemDetails>>> SeedIngredients([AsParameters] CocktailsServices cocktailsServices)
    {
        var commandResult = await cocktailsServices.Mediator.Send(new SeedIngredientsCommand(), cocktailsServices.HttpContextAccessor.HttpContext.RequestAborted);

        if (!commandResult)
        {
            return TypedResults.Json<ProblemDetails>(ProblemDetailsExtensions.CreateValidationProblemDetails("Failed to seed ingredients", StatusCodes.Status502BadGateway), statusCode: StatusCodes.Status502BadGateway);
        }

        return TypedResults.NoContent();
    }

    /// <summary>Gets the cocktails list site map</summary>
    /// <param name="cocktailsServices">The required services</param>
    /// <returns></returns>
    [Produces("text/xml")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async static Task<ContentHttpResult> GetCocktailsSiteMap([AsParameters] CocktailsServices cocktailsServices)
    {
        var sitemap = await cocktailsServices.Queries.GetCocktailsSiteMap(cocktailsServices.HttpContextAccessor.HttpContext.RequestAborted);

        return TypedResults.Content(
            content: sitemap,
            contentType: "text/xml",
            statusCode: 200);
    }

    /// <summary>Gets the cocktails indexnow url listing</summary>
    /// <param name="cocktailsServices">The required services</param>
    /// <returns></returns>
    [Produces("application/json")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async static Task<Results<Ok<CocktailIndexNowRs>, JsonHttpResult<ProblemDetails>>> GetCocktailsIndexNow([AsParameters] CocktailsServices cocktailsServices)
    {
        var indexNow = await cocktailsServices.Queries.GetCocktailsIndexNowResult(cocktailsServices.HttpContextAccessor.HttpContext.RequestAborted);

        return TypedResults.Ok(indexNow);
    }

    /// <summary>Seeds the cocktails in the database</summary>
    /// <param name="request">The request.</param>
    /// <param name="cocktailsServices">The required services</param>
    /// <returns></returns>
    [ProducesDefaultResponseType(typeof(ProblemDetails))]
    public async static Task<Results<NoContent, JsonHttpResult<ProblemDetails>>> SeedCocktails([AsParameters] CocktailsServices cocktailsServices)
    {
        var commandResult = await cocktailsServices.Mediator.Send(new SeedCocktailsCommand(), cocktailsServices.HttpContextAccessor.HttpContext.RequestAborted);

        if (!commandResult)
        {
            return TypedResults.Json<ProblemDetails>(ProblemDetailsExtensions.CreateValidationProblemDetails("Failed to seed cocktails", StatusCodes.Status502BadGateway), statusCode: StatusCodes.Status502BadGateway);
        }

        return TypedResults.NoContent();
    }
}
