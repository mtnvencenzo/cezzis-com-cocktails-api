namespace Cocktails.Api.Domain.Exceptions;

/// <summary>Exception type for domain exceptions
/// </summary>
public class CocktailsApiDomainException : Exception
{
    public CocktailsApiDomainException() { }

    public CocktailsApiDomainException(string message)
        : base(message) { }

    public CocktailsApiDomainException(string message, Exception innerException)
        : base(message, innerException) { }
}
