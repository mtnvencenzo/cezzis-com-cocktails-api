namespace Cocktails.Api.Application.Behaviors.Telemetry;

/// <summary>
/// Options for open telemetry
/// </summary>
public class OTelOptions
{
    /// <summary>
    /// The app settings section name used for open telemetry configurations
    /// </summary>
    public const string SectionName = "OTel";

    /// <summary>
    /// The name of the service
    /// </summary>
    /// <value></value>
    public string ServiceName { get; set; }

    /// <summary>
    /// The namespace that the service belongs to
    /// </summary>
    /// <value></value>
    public string ServiceNamespace { get; set; }

    /// <summary>
    /// Options for open telemetry tracing
    /// </summary>
    /// <value></value>
    public OTelTracesOptions Traces { get; set; }

    /// <summary>
    /// Options for open telemetry logging
    /// </summary>
    /// <value></value>
    public OTelLogsOptions Logs { get; set; }

    /// <summary>
    /// Options for open telemetry metrics
    /// </summary>
    /// <value></value>
    public OTelMetricsOptions Metrics { get; set; }
}