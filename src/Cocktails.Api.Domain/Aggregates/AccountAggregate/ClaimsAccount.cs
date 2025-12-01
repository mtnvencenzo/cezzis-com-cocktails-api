namespace Cocktails.Api.Domain.Aggregates.AccountAggregate;

using Cezzi.Applications;
using Cocktails.Api.Domain.Common;
using Cocktails.Api.Domain.Exceptions;
using System.Collections.Generic;
using System.Security.Claims;

public class ClaimsAccount : ValueObject
{
    public const string ClaimNamespace = "https://www.cezzis.com";
    public const string ClaimType_GivenName = $"{ClaimNamespace}/given_name";
    public const string ClaimType_FamilyName = $"{ClaimNamespace}/family_name";
    public const string ClaimType_Email = $"{ClaimNamespace}/email";
    public const string ClaimType_DisplayName = $"{ClaimNamespace}/display_name";

    public string SubjectId { get; private set; }

    public string Email { get; private set; }

    public string GivenName { get; private set; }

    public string FamilyName { get; private set; }

    public ClaimsIdentity ClaimsIdentity { get; }

    public ClaimsAccount(ClaimsIdentity claimsIdentity)
    {
        Guard.NotNull(claimsIdentity, nameof(claimsIdentity));

        this.SubjectId = claimsIdentity.Name;
        Guard.NotNullOrWhiteSpace(this.SubjectId, () => new CocktailsApiDomainException($"{nameof(this.SubjectId)} cannot be null or empty"));

        this.GivenName = claimsIdentity.Claims.FirstOrDefault(x => x.Type == ClaimType_GivenName)?.Value;

        this.FamilyName = claimsIdentity.Claims.FirstOrDefault(x => x.Type == ClaimType_FamilyName)?.Value;

        this.Email = claimsIdentity.Claims.FirstOrDefault(x => x.Type == ClaimType_Email)?.Value;

        this.ClaimsIdentity = claimsIdentity;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return this.SubjectId;
    }
}
