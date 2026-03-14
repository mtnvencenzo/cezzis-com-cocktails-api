namespace Cocktails.Api.Application.Behaviors.ProbeTelemetry;

using Microsoft.AspNetCore.Http;

/// <summary>
/// Provides a request-scoped check for whether the current request is a health probe.
/// Uses IHttpContextAccessor so the check works for all logs including hosting diagnostics
/// that fire before/after the middleware pipeline.
/// </summary>
public static class ProbeRequestContext
{
    private static readonly HashSet<string> ProbePaths = new(StringComparer.OrdinalIgnoreCase)
    {
        "/api/v1/health/liveness",
        "/api/v1/health/readiness"
    };

    private static IHttpContextAccessor _httpContextAccessor;

    public static void Initialize(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public static bool IsProbeRequest
    {
        get
        {
            var path = _httpContextAccessor?.HttpContext?.Request?.Path.Value;
            return path != null && ProbePaths.Contains(path);
        }
    }
}
