//namespace Cocktails.Api.Application.Behaviors.Telemetry;

//using Cocktails.Domain;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Http.Features;
//using Microsoft.AspNetCore.Mvc.Controllers;

///// <summary>
///// 
///// </summary>
//public static class LogHelper
//{
//    /// <summary>Enriches from request.</summary>
//    /// <param name="diagnosticContext">The diagnostic context.</param>
//    /// <param name="httpContext">The HTTP context.</param>
//    public static void EnrichFromRequest(IDiagnosticContext diagnosticContext, HttpContext httpContext)
//    {
//        try
//        {
//            var endpointBeingHit = httpContext.Features.Get<IEndpointFeature>()?.Endpoint;
//            var actionDescriptor = endpointBeingHit?.Metadata?.GetMetadata<ControllerActionDescriptor>();

//            if (!string.IsNullOrWhiteSpace(actionDescriptor?.ActionName))
//            {
//                // setting route template moniker
//                diagnosticContext.Set(Monikers.Api.RouteTemplate.Replace("@", ""), actionDescriptor?.AttributeRouteInfo?.Template);
//            }
//        }
//        catch { }
//    }
//}
