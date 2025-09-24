namespace Cocktails.Api.Domain.Config;

public class EntraCiamConfig
{
    public const string SectionName = "EntraCIAM";

    public string Instance { get; set; }
    public string Domain { get; set; }
    public string ClientId { get; set; }
    public string SignUpSignInPolicyId { get; set; }
    public string Audience { get; set; }
}
