namespace Cocktails.Api.Domain.Aggregates.AccountAggregate;

using Cocktails.Api.Domain.Common;
using Cocktails.Api.Domain.Exceptions;
using System;
using System.Text.Json.Serialization;

public class AccountCocktailRecommendationItem : Entity
{
    [JsonInclude]
    public string Name { get; private set; }

    [JsonInclude]
    public string Ingredients { get; private set; }

    [JsonInclude]
    public string Directions { get; private set; }

    [JsonInclude]
    public string VerificationCode { get; private set; }

    [JsonConstructor]
    protected AccountCocktailRecommendationItem() { }

    public AccountCocktailRecommendationItem(string name, string ingredients, string directions, string verificationCode) : this()
    {
        this.Name = !string.IsNullOrWhiteSpace(name) ? name : throw new CocktailsApiDomainException($"{nameof(name)} cannot be null or empty");
        this.Ingredients = !string.IsNullOrWhiteSpace(ingredients) ? ingredients : throw new CocktailsApiDomainException($"{nameof(ingredients)} cannot be null or empty");
        this.Directions = !string.IsNullOrWhiteSpace(directions) ? directions : throw new CocktailsApiDomainException($"{nameof(directions)} cannot be null or empty");
        this.VerificationCode = !string.IsNullOrWhiteSpace(verificationCode) ? verificationCode : throw new CocktailsApiDomainException($"{nameof(verificationCode)} cannot be null or empty");
    }

    public string GetRecommendation() => $"Try the '{this.Name}' cocktail!<br/><br/><b>Ingredients:</b><br/> {this.Ingredients}<br/><br/><b>Directions:</b><br/>{this.Directions}<br/><br/>";
}
