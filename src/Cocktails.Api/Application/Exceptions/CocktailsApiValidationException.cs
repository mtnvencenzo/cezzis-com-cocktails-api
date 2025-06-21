namespace Cocktails.Api.Application.Exceptions;

using Cezzi.Applications.Extensions;
using FluentValidation.Results;

public class CocktailsApiValidationException : Exception
{
    public CocktailsApiValidationException() { }

    public CocktailsApiValidationException(string message)
        : base(message) { }

    public CocktailsApiValidationException(string message, Exception innerException)
        : base(message, innerException) { }

    public CocktailsApiValidationException(List<ValidationFailure> errors)
    {
        this.Errors = errors;
    }

    public List<ValidationFailure> Errors { get; init; }

    public int GetSuggestedHttpStatusCode()
    {
        if (this.Errors == null || this.Errors.Count == 0)
        {
            return StatusCodes.Status400BadRequest;
        }

        var statusCodes = new List<int>();

        this.Errors
            .Where(x => !string.IsNullOrWhiteSpace(x.ErrorCode))
            .Select(x => x.ErrorCode)
            .Distinct()
            .ForEach(x =>
            {
                if (int.TryParse(x, out var statusCode))
                {
                    statusCodes.Add(statusCode);
                }
            });

        if (statusCodes.Count > 0)
        {
            return statusCodes.OrderByDescending(x => x).First();
        }

        return StatusCodes.Status400BadRequest;
    }
}
