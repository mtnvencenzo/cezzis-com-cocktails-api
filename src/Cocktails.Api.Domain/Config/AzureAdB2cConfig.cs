namespace Cocktails.Api.Domain.Config;

public class AzureAdB2cConfig
{
    public const string SectionName = "AzureAdB2C";

    public string Instance { get; set; }
    public string Domain { get; set; }
    public string ClientId { get; set; }
    public string SignUpSignInPolicyId { get; set; }
    public string Audience { get; set; }
}
