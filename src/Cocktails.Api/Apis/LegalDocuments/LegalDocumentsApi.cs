namespace Cocktails.Api.Apis.LegalDocuments;
using Cocktails.Api.Application.Concerns.LegalDocuments;
using Cocktails.Api.Application.Concerns.LegalDocuments.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// 
/// </summary>
public static class LegalDocumentsApi
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public static RouteGroupBuilder MapLegalDocumentsApiV1(this IEndpointRouteBuilder app)
    {
        var groupBuilder = app.MapGroup("/legal/documents")
            .WithTags("Legal")
            .AllowAnonymous();

        groupBuilder.MapGet("/privacy-policy", GetPrivacyPolicy)
            .WithDisplayName(nameof(GetPrivacyPolicy))
            .WithName(nameof(GetPrivacyPolicy))
            .WithDescription("Gets the Cezzi's.com privacy policy");

        groupBuilder.MapGet("/terms-of-service", GetTermsOfService)
            .WithDisplayName(nameof(GetTermsOfService))
            .WithName(nameof(GetTermsOfService))
            .WithDescription("Gets the Cezzi's.com terms of service");

        return groupBuilder;
    }

    /// <summary>Get the privacy policy</summary>
    /// <param name="legalDocumentServices"></param>
    /// <returns></returns>
    [ProducesDefaultResponseType(typeof(ProblemDetails))]
    public static Ok<LegalDocumentRs> GetPrivacyPolicy([AsParameters] LegalDocumentsServices legalDocumentServices)
    {
        var document = legalDocumentServices.Queries.GetPrivacyPolicy();

        return TypedResults.Ok(document);
    }

    /// <summary>Get the terms of service</summary>
    /// <param name="legalDocumentServices"></param>
    /// <returns></returns>
    [ProducesDefaultResponseType(typeof(ProblemDetails))]
    public static Ok<LegalDocumentRs> GetTermsOfService([AsParameters] LegalDocumentsServices legalDocumentServices)
    {
        var document = legalDocumentServices.Queries.GetTermsOfService();

        return TypedResults.Ok(document);
    }
}