namespace Cocktails.Api.Application.Behaviors.Telemetry;

/// <summary>
/// Options for open telemetry logging
/// </summary>
public class OTelLogsOptions
{
    /// <summary>
    /// The open telemetry exporter options for logging
    /// </summary>
    /// <value></value>
    public OTelExporterOptions OtlpExporter { get; set; }

    /// <summary>
    /// Whether or not to include formatted messages with the open telemetry logging
    /// </summary>
    /// <value></value>
    public bool IncludeFormattedMessage { get; set; } = false;

    /// <summary>
    /// Where or not to include scopes with the logs
    /// </summary>
    /// <value></value>
    public bool IncludeScopes { get; set; } = false;

    /// <summary>
    /// Whether or not to include the console exporter with open telemetry logging
    /// </summary>
    /// <value></value>
    public bool AddConsoleExporter { get; set; } = false;
}