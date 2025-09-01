namespace Cocktails.Api.Application.Behaviors.Telemetry;

/// <summary>
/// Options for open telemetry traces
/// </summary>
public class OTelTracesOptions
{
    /// <summary>
    /// The open telemetry exporter options for logging
    /// </summary>
    /// <value></value>
    public OTelExporterOptions OtlpExporter { get; set; }

    /// <summary>
    /// Excluded route paths for the aspnet open telemetry traces
    /// </summary>
    /// <value></value>
    public List<string> ExcludePaths { get; set; } = [];

    /// <summary>
    /// Additional sources to add to the opentelemetry tracing
    /// </summary>
    /// <value></value>
    public List<string> Sources { get; set; } = [];

    /// <summary>
    /// Whether or not to include the console exporter with open telemetry logging
    /// </summary>
    /// <value></value>
    public bool AddConsoleExporter { get; set; } = false;
}