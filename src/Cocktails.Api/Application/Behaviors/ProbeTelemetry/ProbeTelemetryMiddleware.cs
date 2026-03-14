namespace Cocktails.Api.Application.Behaviors.ProbeTelemetry;

/// <summary>
/// Middleware that sets a context flag for health probe request paths
/// so the logging filter can suppress log output during those requests.
/// </summary>
public class ProbeTelemetryMiddleware(RequestDelegate next)
{
    private static readonly HashSet<string> ProbePaths = new(StringComparer.OrdinalIgnoreCase)
    {
        "/api/v1/health/liveness",
        "/api/v1/health/readiness"
    };

    public async Task InvokeAsync(HttpContext context)
    {
        if (ProbePaths.Contains(context.Request.Path.Value ?? string.Empty))
        {
            ProbeRequestContext.IsProbeRequest.Value = true;
            try
            {
                await next(context);
            }
            finally
            {
                ProbeRequestContext.IsProbeRequest.Value = false;
            }
        }
        else
        {
            await next(context);
        }
    }
}
