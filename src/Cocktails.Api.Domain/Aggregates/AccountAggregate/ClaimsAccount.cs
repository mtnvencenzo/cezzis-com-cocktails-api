namespace Cocktails.Api.Domain.Aggregates.AccountAggregate;

using Cezzi.Applications;
using Cocktails.Api.Domain.Common;
using Cocktails.Api.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Security.Claims;

public class ClaimsAccount : ValueObject
{
    public string SubjectId { get; private set; }

    public string Email { get; private set; }

    public string GivenName { get; private set; }

    public string FamilyName { get; private set; }

    public ClaimsIdentity ClaimsIdentity { get; }

    public ClaimsAccount(ClaimsIdentity claimsIdentity)
    {
        Guard.NotNull(claimsIdentity, nameof(claimsIdentity));

        this.SubjectId = claimsIdentity.Claims.FirstOrDefault(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
        Guard.NotNullOrWhiteSpace(this.SubjectId, () => new CocktailsApiDomainException($"{nameof(this.SubjectId)} cannot be null or empty"));

        this.GivenName = claimsIdentity.Claims.FirstOrDefault(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname")?.Value;
        Guard.NotNullOrWhiteSpace(this.GivenName, () => new CocktailsApiDomainException($"{nameof(this.GivenName)} cannot be null or empty"));

        this.FamilyName = claimsIdentity.Claims.FirstOrDefault(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname")?.Value;
        Guard.NotNullOrWhiteSpace(this.FamilyName, () => new CocktailsApiDomainException($"{nameof(this.FamilyName)} cannot be null or empty"));

        this.Email = claimsIdentity.Claims.FirstOrDefault(x => x.Type == "emails")?.Value;
        Guard.NotNullOrWhiteSpace(this.Email, () => new CocktailsApiDomainException($"{nameof(this.FamilyName)} cannot be null or empty"));

        this.ClaimsIdentity = claimsIdentity;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return this.SubjectId;
    }
}
