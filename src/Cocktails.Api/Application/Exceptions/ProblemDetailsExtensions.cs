namespace Cocktails.Api.Application.Exceptions;

using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;

public static class ProblemDetailsExtensions
{
    public static ProblemDetails CreateValidationProblemDetails(string error, int statusCode = 400)
    {
        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Type = "ValidationFailure",
            Title = "Validation error",
            Detail = "One or more validation errors has occurred"
        };

        if (!string.IsNullOrWhiteSpace(error))
        {
            problemDetails.Extensions["errors"] = new List<ValidationFailure>
            {
                new(propertyName: null, errorMessage: error)
            };
        }

        return problemDetails;
    }
}
