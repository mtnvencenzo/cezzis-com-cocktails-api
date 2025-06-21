namespace Cocktails.Api.Domain.Aggregates.AccountAggregate;

using Cezzi.Applications;
using Cocktails.Api.Domain.Common;
using System.Text.Json.Serialization;

public class AccountCocktailRatings : Entity
{
    [JsonInclude]
    public string SubjectId { get; private set; }

    [JsonInclude]
    public List<AccountCocktailRatingItem> Ratings { get; private set; } = [];

    [JsonInclude]
    public string ETag { get; private set; }

    [JsonInclude]
    public string Discriminator { get; private set; }

    [JsonConstructor]
    protected AccountCocktailRatings() { }

    public AccountCocktailRatings(string id, string subjectId)
    {
        this.Id = id;
        this.SubjectId = subjectId;
        this.CreatedOn = DateTimeOffset.UtcNow;
        this.UpdatedOn = DateTimeOffset.UtcNow;
    }

    public AccountCocktailRatings AddRating(string cocktailId, int stars)
    {
        Guard.NotNullOrWhiteSpace(cocktailId);
        Guard.Positive(stars);

        if (stars > 5)
        {
            stars = 5;
        }

        this.Ratings.Add(new AccountCocktailRatingItem(cocktailId, stars));
        return this;
    }
}
