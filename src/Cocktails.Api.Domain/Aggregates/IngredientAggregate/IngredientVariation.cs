namespace Cocktails.Api.Domain.Aggregates.IngredientAggregate;

using Cezzi.Applications.Extensions;
using Cocktails.Api.Domain.Common;
using Cocktails.Api.Domain.Exceptions;
using System.Collections.Generic;
using System.Text.Json.Serialization;

public class IngredientVariation : Entity
{
    [JsonInclude]
    public string Name { get; private set; }

    [JsonInclude, JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
    public List<string> Applications { get; private set; }

    [JsonConstructor]
    protected IngredientVariation()
    {
        this.Applications = [];
    }

    public IngredientVariation(
        string id,
        string name) : this(id, name, []) { }

    public IngredientVariation(
        string id,
        string name,
        List<string> applications) : this()
    {
        this.Id = !string.IsNullOrWhiteSpace(id)
            ? id
            : throw new CocktailsApiDomainException($"{nameof(id)} cannot be null or empty");

        this.Name = !string.IsNullOrWhiteSpace(name)
            ? name
            : throw new CocktailsApiDomainException($"{nameof(name)} cannot be null or empty");

        this.Applications = applications != null && applications.Count > 0 && !applications.Contains(IngredientApplication.None.ToString(), StringComparer.OrdinalIgnoreCase)
            ? [.. applications.Distinct()]
            : throw new CocktailsApiDomainException($"{nameof(applications)} must contain at least one specified application");
    }
}
