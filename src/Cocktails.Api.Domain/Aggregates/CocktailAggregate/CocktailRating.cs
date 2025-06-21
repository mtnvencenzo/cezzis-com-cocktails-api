namespace Cocktails.Api.Domain.Aggregates.CocktailAggregate;

using Cocktails.Api.Domain.Common;
using Cocktails.Api.Domain.Exceptions;
using System.Text.Json.Serialization;

public class CocktailRating : ValueObject
{
    [JsonInclude]
    public int OneStars { get; private set; }

    [JsonInclude]
    public int TwoStars { get; private set; }

    [JsonInclude]
    public int ThreeStars { get; private set; }

    [JsonInclude]
    public int FourStars { get; private set; }

    [JsonInclude]
    public int FiveStars { get; private set; }

    [JsonInclude]
    public int RatingCount { get; private set; }

    [JsonInclude]
    public int TotalStars => this.OneStars + (2 * this.TwoStars) + (3 * this.ThreeStars) + (4 * this.FourStars) + (5 * this.FiveStars);

    [JsonInclude]
    public decimal Rating => this.TotalStars == 0
        ? 0
        : Convert.ToDecimal(this.TotalStars) / Convert.ToDecimal(this.RatingCount == 0 ? 1 : this.RatingCount);

    [JsonConstructor]
    private CocktailRating() { }

    public CocktailRating(int oneStars, int twoStars, int threeStars, int fourStars, int fiveStars, int ratingCount)
    {
        this.OneStars = oneStars >= 0
            ? oneStars
            : throw new CocktailsApiDomainException($"{nameof(oneStars)} must be greater than or equal to zero");

        this.TwoStars = twoStars >= 0
            ? twoStars
            : throw new CocktailsApiDomainException($"{nameof(twoStars)} must be greater than or equal to zero");

        this.ThreeStars = threeStars >= 0
            ? threeStars
            : throw new CocktailsApiDomainException($"{nameof(threeStars)} must be greater than or equal to zero");

        this.FourStars = fourStars >= 0
            ? fourStars
            : throw new CocktailsApiDomainException($"{nameof(fourStars)} must be greater than or equal to zero");

        this.FiveStars = fiveStars >= 0
            ? fiveStars
            : throw new CocktailsApiDomainException($"{nameof(fiveStars)} must be greater than or equal to zero");

        this.RatingCount = ratingCount >= 0
            ? ratingCount
            : throw new CocktailsApiDomainException($"{nameof(ratingCount)} must be greater than or equal to zero");
    }

    public CocktailRating Increment(int stars)
    {
        if (stars is < 1 or > 5)
        {
            throw new CocktailsApiDomainException($"{nameof(stars)} must be between 1 and 5");
        }

        if (stars == 1)
        {
            this.OneStars++;
        }
        else if (stars == 2)
        {
            this.TwoStars++;
        }
        else if (stars == 3)
        {
            this.ThreeStars++;
        }
        else if (stars == 4)
        {
            this.FourStars++;
        }
        else if (stars == 5)
        {
            this.FiveStars++;
        }

        this.RatingCount++;

        return this;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return this.OneStars;
        yield return this.TwoStars;
        yield return this.ThreeStars;
        yield return this.FourStars;
        yield return this.FiveStars;
    }
}
