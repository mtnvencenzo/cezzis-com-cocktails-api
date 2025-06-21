namespace Cocktails.Api.Application.Behaviors.Telemetry;

using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;

public class SyntheticRequestTelemetryFilter(ITelemetryProcessor next) : ITelemetryProcessor
{
    private readonly ITelemetryProcessor next = next;

    public void Process(ITelemetry item)
    {
        if (!string.IsNullOrEmpty(item.Context.Operation.SyntheticSource))
        {
            return;
        }

        if (item is RequestTelemetry request && request?.Url?.LocalPath != null)
        {
            if (ExcludePaths.Contains(request.Url.LocalPath.ToLower()))
            {
                return;
            }
        }

        // Send everything else:
        this.next.Process(item);
    }

    public static IList<string> ExcludePaths => ["", "/", "/diagnostics/server"];

    ///// <summary>Gets the log level.</summary>
    ///// <param name="ctx">The CTX.</param>
    ///// <param name="_">The .</param>
    ///// <param name="ex">The ex.</param>
    ///// <returns></returns>
    //public static LogEventLevel GetLogLevel(HttpContext ctx, double _, Exception ex) =>
    //    ex != null
    //        ? LogEventLevel.Error
    //        : ctx.Response.StatusCode > 499
    //            ? LogEventLevel.Error
    //            : ExcludePaths.Contains(ctx.Request?.Path.ToString().ToLower())
    //                ? LogEventLevel.Verbose
    //                : LogEventLevel.Information;
}