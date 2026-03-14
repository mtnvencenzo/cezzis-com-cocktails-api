namespace Cocktails.Api.Application.Behaviors.ProbeTelemetry;

/// <summary>
/// Provides an async-local flag indicating whether the current request is a health probe.
/// </summary>
public static class ProbeRequestContext
{
    public static readonly AsyncLocal<bool> IsProbeRequest = new();
}
