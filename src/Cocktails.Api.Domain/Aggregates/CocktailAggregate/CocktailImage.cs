namespace Cocktails.Api.Domain.Aggregates.CocktailAggregate;

using Cocktails.Api.Domain.Common;
using Cocktails.Api.Domain.Exceptions;
using System.Collections.Generic;
using System.Text.Json.Serialization;

public class CocktailImage : ValueObject
{
    [JsonInclude]
    public string Uri { get; private set; }

    [JsonInclude]
    public CocktailImageType Type { get; private set; }

    [JsonInclude]
    public int Width { get; private set; }

    [JsonInclude]
    public int Height { get; private set; }

    [JsonConstructor]
    private CocktailImage() { }

    public CocktailImage(string uri, CocktailImageType type, int width, int height)
    {
        this.SetUri(uri);

        this.Type = type != CocktailImageType.None
            ? type
            : throw new CocktailsApiDomainException($"{nameof(type)} must be specified");

        this.Width = width > 0
            ? width
            : throw new CocktailsApiDomainException($"{nameof(width)} must be greater than zero");

        this.Height = height > 0
            ? height
            : throw new CocktailsApiDomainException($"{nameof(height)} must be greater than zero");
    }

    public CocktailImage SetUri(string uri)
    {
        this.Uri = !string.IsNullOrWhiteSpace(uri)
            ? uri
            : throw new CocktailsApiDomainException($"{nameof(uri)} cannot be null or empty");

        return this;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return this.Uri;
        yield return this.Type;
        yield return this.Width;
        yield return this.Height;
    }
}
