namespace Cocktails.Api.Domain.Aggregates.AccountAggregate;

using Cezzi.Applications;
using Cezzi.Applications.Extensions;
using Cocktails.Api.Domain.Common;
using Cocktails.Api.Domain.Exceptions;
using System.Security.Claims;
using System.Text.Json.Serialization;

public class Account : Entity, IAggregateRoot
{
    [JsonInclude]
    public string SubjectId { get; private set; }

    [JsonInclude]
    public string RatingsId { get; private set; }

    [JsonInclude]
    public string LoginEmail { get; private set; }

    [JsonInclude]
    public string Email { get; private set; }

    [JsonInclude]
    public string GivenName { get; private set; }

    [JsonInclude]
    public string FamilyName { get; private set; }

    [JsonInclude]
    public string DisplayName { get; private set; }

    [JsonInclude]
    public string AvatarUri { get; private set; }

    [JsonInclude]
    public AccountAddress PrimaryAddress { get; private set; }

    [JsonInclude]
    public AccountAccessibilitySettings Accessibility { get; private set; }

    [JsonInclude]
    public string ETag { get; private set; }

    [JsonInclude]
    public string Discriminator { get; private set; }

    [JsonInclude]
    public List<string> FavoriteCocktails { get; private set; } = [];

    [JsonConstructor]
    protected Account() { }

    public Account(ClaimsAccount claimsAccount)
    {
        Guard.NotNull(claimsAccount, nameof(claimsAccount));

        this.Id = Guid.NewGuid().ToString();
        this.SubjectId = claimsAccount.SubjectId;
        this.LoginEmail = claimsAccount.Email;
        this.CreatedOn = DateTimeOffset.UtcNow;
        this.UpdatedOn = DateTimeOffset.UtcNow;

        this.SetName(givenName: claimsAccount.GivenName, claimsAccount.FamilyName);
        this.SetEmail(claimsAccount.Email);
    }

    public Account SetName(string givenName, string familyName)
    {
        if (string.IsNullOrWhiteSpace(givenName))
        {
            throw new CocktailsApiDomainException($"{nameof(givenName)} not specified");
        }

        if (string.IsNullOrWhiteSpace(familyName))
        {
            throw new CocktailsApiDomainException($"{familyName} not specified");
        }

        this.GivenName = givenName;
        this.FamilyName = familyName;

        return this;
    }

    public Account SetEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new CocktailsApiDomainException($"{nameof(email)} not specified");
        }

        this.Email = email;

        return this;
    }

    public Account SetLoginEmail(string loginEmail)
    {
        if (string.IsNullOrWhiteSpace(loginEmail))
        {
            throw new CocktailsApiDomainException($"{nameof(loginEmail)} not specified");
        }

        if (!string.IsNullOrWhiteSpace(this.LoginEmail))
        {
            throw new CocktailsApiDomainException($"Account login email has already been specified");
        }

        this.LoginEmail = loginEmail;

        return this;
    }

    public Account SetDisplayName(string displayName)
    {
        this.DisplayName = displayName;
        return this;
    }

    public Account SetAvatarUri(string avatarUri)
    {
        if (string.IsNullOrWhiteSpace(avatarUri))
        {
            throw new CocktailsApiDomainException("avatar uri cannot be null");
        }

        this.AvatarUri = avatarUri;
        return this;
    }

    public Account SetAccessibilitySettings(AccessibilityTheme theme)
    {
        Guard.NotEnumMember(theme, AccessibilityTheme.None, () => new CocktailsApiDomainException($"{nameof(AccessibilityTheme)} not specified"));

        var value = (int)theme;

        if (!Enum.GetValues<AccessibilityTheme>().Select(x => (int)x).Contains(value))
        {
            throw new CocktailsApiDomainException($"{nameof(AccessibilityTheme)} member '{theme}' not found");
        }

        this.Accessibility = new AccountAccessibilitySettings(theme);
        return this;
    }

    public Account SetRatingsId(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            throw new CocktailsApiDomainException($"{nameof(id)} not specified");
        }

        this.RatingsId = id;
        return this;
    }

    public Account SetPrimaryAddress(
        string addressLine1,
        string addressLine2,
        string city,
        string region,
        string subRegion,
        string postalCode,
        string country)
    {
        this.PrimaryAddress = new AccountAddress(
            addressLine1: addressLine1,
            addressLine2: addressLine2,
            city: city,
            region: region,
            subRegion: subRegion,
            postalCode: postalCode,
            country: country);

        return this;
    }

    public Account ClearPrimaryAddress()
    {
        this.PrimaryAddress = null;
        return this;
    }

    public Account ManageFavoriteCocktails(
        List<string> remove,
        List<string> add)
    {
        this.FavoriteCocktails ??= [];

        remove
            .Distinct()
            .Where(x => this.FavoriteCocktails.Contains(x, StringComparer.OrdinalIgnoreCase))
            .ToList()
            .ForEach(x => this.FavoriteCocktails.Remove(x));

        add
            .Distinct()
            .Where(x => !this.FavoriteCocktails.Contains(x, StringComparer.OrdinalIgnoreCase))
            .ToList()
            .ForEach(x => this.FavoriteCocktails.Add(x));

        return this;
    }
}
