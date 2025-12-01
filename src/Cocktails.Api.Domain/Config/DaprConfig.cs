namespace Cocktails.Api.Domain.Config;

public class DaprConfig
{
    public const string SectionName = "Dapr";

    public string HttpEndpoint { get; set; }

    public string GrpcEndpoint { get; set; }
}
