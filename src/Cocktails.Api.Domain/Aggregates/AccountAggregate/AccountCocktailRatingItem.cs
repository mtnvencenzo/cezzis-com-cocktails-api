namespace Cocktails.Api.Domain.Aggregates.AccountAggregate;

using Cocktails.Api.Domain.Common;
using System.Text.Json.Serialization;

public class AccountCocktailRatingItem : ValueObject
{
    [JsonInclude]
    public string Id { get; private set; }

    [JsonInclude]
    public string CocktailId { get; private set; }

    [JsonInclude]
    public int Stars { get; private set; }

    [JsonInclude]
    public DateTimeOffset CreatedAt { get; private set; }

    [JsonInclude]
    public DateTimeOffset UpdatedAt { get; private set; }

    [JsonConstructor]
    public AccountCocktailRatingItem() { }

    public AccountCocktailRatingItem(string cocktailId, int stars)
    {
        this.CocktailId = cocktailId;
        this.Stars = stars;
        this.CreatedAt = DateTimeOffset.UtcNow;
        this.UpdatedAt = DateTimeOffset.UtcNow;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return this.CocktailId;
        yield return this.Stars;
        yield return this.CreatedAt;
        yield return this.UpdatedAt;
    }
}
