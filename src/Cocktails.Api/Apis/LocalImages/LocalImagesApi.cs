namespace Cocktails.Api.Apis.LocalImages;

using Cocktails.Api.Application.Exceptions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Net;

/// <summary>
/// 
/// </summary>
public static class LocalImagesApi
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public static RouteGroupBuilder MapLocalImagesApiV1(this IEndpointRouteBuilder app)
    {
        var groupBuilder = app.MapGroup("/images")
            .WithTags("Images")
            .ExcludeFromDescription()
            .AllowAnonymous();

        groupBuilder.MapGet("/{name}", GetImage)
            .WithName(nameof(GetImage))
            .WithDisplayName(nameof(GetImage));

        return groupBuilder;
    }

    /// <summary>Returns images from a local host filesystem.  Not intended for production
    /// </summary>
    /// <param name="name"></param>
    /// <param name="localImageServices"></param>
    /// <returns></returns>
    [ProducesResponseType(typeof(PhysicalFileHttpResult), (int)HttpStatusCode.OK, "image/webp")]
    [ProducesDefaultResponseType(typeof(ProblemDetails))]
    public static Results<PhysicalFileHttpResult, JsonHttpResult<ProblemDetails>> GetImage(
        [FromRoute] string name,
        [AsParameters] LocalImagesServices localImageServices)
    {
        var filepath = localImageServices.Queries.GetImageFilePath(name);

        if (string.IsNullOrWhiteSpace(filepath))
        {
            return TypedResults.Json<ProblemDetails>(ProblemDetailsExtensions.CreateValidationProblemDetails("Image not found", StatusCodes.Status404NotFound), statusCode: StatusCodes.Status404NotFound);
        }

        return TypedResults.PhysicalFile(filepath, contentType: "image/webp");
    }
}