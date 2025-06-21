namespace Cocktails.Api.Domain.Config;

public class MsGraphConfig
{
    public const string SectionName = "MsGraph";

    public string TenantId { get; set; }

    public string ClientId { get; set; }

    public string ClientSecret { get; set; }
}