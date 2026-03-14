namespace Cocktails.Api.Application.Concerns.Health.Models;

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

/// <summary>The health check response</summary>
[type: Description("The health check response")]
public record HealthCheckRs
(
    [property: Required()]
    [property: Description("The health status")]
    string Status,

    [property: Description("The service version")]
    string Version = null,

    [property: Description("Human-readable output message")]
    string Output = null,

    [property: Description("Per-dependency health details")]
    Dictionary<string, string> Details = null
);
