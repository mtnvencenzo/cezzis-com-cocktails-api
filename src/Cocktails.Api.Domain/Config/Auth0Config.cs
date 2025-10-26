namespace Cocktails.Api.Domain.Config;

public class Auth0Config
{
    public const string SectionName = "Auth0";

    public required string Domain { get; set; }
    public required string ClientId { get; set; }
    public required string Audience { get; set; }
    public required string DatabaseConnectionName { get; set; }
    public required string ManagementDomain { get; set; }
    public required string ManagementM2MClientId { get; set; }
    public required string ManagementM2MClientSecret { get; set; }
}