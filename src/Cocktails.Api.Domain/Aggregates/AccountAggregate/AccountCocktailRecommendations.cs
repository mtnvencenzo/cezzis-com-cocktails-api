namespace Cocktails.Api.Domain.Aggregates.AccountAggregate;

using Cezzi.Applications;
using Cocktails.Api.Domain.Common;
using Cocktails.Api.Domain.Exceptions;
using System;
using System.Text.Json.Serialization;

public class AccountCocktailRecommendations : Entity
{
    [JsonInclude]
    public string SubjectId { get; private set; }

    [JsonInclude]
    public List<AccountCocktailRecommendationItem> Recommendations { get; private set; } = [];

    [JsonConstructor]
    protected AccountCocktailRecommendations() { }

    public AccountCocktailRecommendations(string id, string subjectId)
    {
        this.Id = id;
        this.SubjectId = subjectId;
        this.CreatedOn = DateTimeOffset.UtcNow;
        this.UpdatedOn = DateTimeOffset.UtcNow;
    }

    public AccountCocktailRecommendations AddRecommendation(string name, string ingredients, string directions, string verificationCode)
    {
        this.Recommendations.Add(new AccountCocktailRecommendationItem(name, ingredients, directions, verificationCode));
        return this;
    }
}
