namespace Cocktails.Api.Domain.Aggregates.CocktailAggregate;

using Cocktails.Api.Domain.Common;
using Cocktails.Api.Domain.Exceptions;
using System.Collections.Generic;
using System.Text.Json.Serialization;

public class CocktailInstructionStep : ValueObject
{
    [JsonInclude]
    public string DisplayValue { get; private set; }

    [JsonInclude]
    public int Order { get; private set; }

    [JsonConstructor]
    private CocktailInstructionStep() { }

    public CocktailInstructionStep(string displayValue, int order)
    {
        this.DisplayValue = !string.IsNullOrWhiteSpace(displayValue)
            ? displayValue
            : throw new CocktailsApiDomainException($"{nameof(displayValue)} cannot be null or empty");

        this.Order = order > 0
            ? order
            : throw new CocktailsApiDomainException($"{nameof(order)} must be greater than or equal to zero");
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return this.DisplayValue;
        yield return this.Order;
    }
}
