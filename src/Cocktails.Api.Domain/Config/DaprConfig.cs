namespace Cocktails.Api.Domain.Config;

public class DaprConfig
{
    public const string SectionName = "Dapr";

    public string HttpEndpoint { get; set; }

    public string GrpcEndpoint { get; set; }

    /// <summary>The Dapr sidecar API token used to secure communication between the application and the Dapr sidecar.
    /// </summary>
    /// <remarks>Dapr sidecar token (sent from app to sidecar)</remarks>
    public string DaprAppToken { get; set; }

    /// <summary>The Dapr application API token used to secure communication between the application and the Dapr sidecar.
    /// </summary>
    /// <remarks>Application API token (sent from sidecar to app)</remarks>
    public string AppApiToken { get; set; }

    public bool InitJobEnabled { get; set; } = true;
}
