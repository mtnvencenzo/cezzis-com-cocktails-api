namespace Cocktails.Api.Domain.Aggregates.IngredientAggregate;

using Cocktails.Api.Domain.Common;
using Cocktails.Api.Domain.Exceptions;

public class IngredientFilter : Entity
{
    public IngredientFilter(string id, IngredientType type, string name)
    {
        this.Id = !string.IsNullOrWhiteSpace(id)
            ? id
            : throw new CocktailsApiDomainException($"{nameof(id)} cannot be null or empty");

        this.Type = type != IngredientType.None
            ? type
            : throw new CocktailsApiDomainException($"{nameof(type)} must be specified");

        this.Name = !string.IsNullOrWhiteSpace(name)
            ? name
            : throw new CocktailsApiDomainException($"{nameof(name)} cannot be null or empty");
    }

    public string Name { get; }

    public IngredientType Type { get; }
}
