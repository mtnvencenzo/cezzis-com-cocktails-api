namespace Cocktails.Api.Domain.Config;

public class TestAccountConfig
{
    public const string SectionName = "TestAccount";

    public string GivenName { get; set; }

    public string FamilyName { get; set; }

    public string DisplayName { get; set; }

    public string SubjectId { get; set; }

    public string LoginEmail { get; set; }

    public string AddressLine1 { get; set; }

    public string City { get; set; }

    public string Region { get; set; }

    public string PostalCode { get; set; }

    public string Country { get; set; }
}