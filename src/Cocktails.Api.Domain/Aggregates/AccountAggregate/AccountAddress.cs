namespace Cocktails.Api.Domain.Aggregates.AccountAggregate;

using Cocktails.Api.Domain.Common;
using System.Collections.Generic;
using System.Text.Json.Serialization;

public class AccountAddress : ValueObject
{
    [JsonInclude]
    public string AddressLine1 { get; private set; }

    [JsonInclude]
    public string AddressLine2 { get; private set; }

    [JsonInclude]
    public string City { get; private set; }

    [JsonInclude]
    public string Region { get; private set; }

    [JsonInclude]
    public string SubRegion { get; private set; }

    [JsonInclude]
    public string PostalCode { get; private set; }

    [JsonInclude]
    public string Country { get; private set; }

    [JsonConstructor]
    protected AccountAddress() { }

    public AccountAddress(
        string addressLine1,
        string addressLine2,
        string city,
        string region,
        string subRegion,
        string postalCode,
        string country)
    {
        this.AddressLine1 = addressLine1;
        this.AddressLine2 = addressLine2;
        this.City = city;
        this.Region = region;
        this.SubRegion = subRegion;
        this.PostalCode = postalCode;
        this.Country = country;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return this.AddressLine1;
        yield return this.AddressLine2;
        yield return this.City;
        yield return this.Region;
        yield return this.SubRegion;
        yield return this.PostalCode;
        yield return this.Country;
    }
}
