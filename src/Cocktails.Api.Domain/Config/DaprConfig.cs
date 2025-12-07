namespace Cocktails.Api.Domain.Config;

public class DaprConfig
{
    public const string SectionName = "Dapr";

    public string HttpEndpoint { get; set; }

    public string GrpcEndpoint { get; set; }

    public string ApiToken { get; set; }

    public string AppToken { get; set; }

    public bool InitJobEnabled { get; set; } = true;
}
