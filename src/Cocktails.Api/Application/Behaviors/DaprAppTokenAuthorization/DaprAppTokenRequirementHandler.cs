namespace Cocktails.Api.Application.Behaviors.DaprAppTokenAuthorization;

using Cocktails.Api.Domain.Config;
using Cocktails.Api.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;

public class DaprAppTokenRequirementHandler(
    IOptions<DaprConfig> daprConfig,
    IRequestHeaderAccessor requestHeaderAccessor,
    ILogger<DaprAppTokenRequirementHandler> logger) : AuthorizationHandler<DaprAppTokenRequirement>
{
    public const string DaprAppTokenHeaderName = "dapr-api-token";

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, DaprAppTokenRequirement requirement)
    {
        if (!string.IsNullOrWhiteSpace(daprConfig.Value.AppToken))
        {
            var headerValue = requestHeaderAccessor.GetHeaderValue(DaprAppTokenHeaderName);
            if (string.IsNullOrWhiteSpace(headerValue) || !CryptographicOperations.FixedTimeEquals(
                System.Text.Encoding.UTF8.GetBytes(headerValue),
                System.Text.Encoding.UTF8.GetBytes(daprConfig.Value.AppToken)))
            {
                logger.LogWarning("Dapr APP token authorization failed due to invalid supplied token");
                context.Fail();
                return Task.CompletedTask;
            }
        }
        else
        {
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") != "local")
            {
                logger.LogWarning("Dapr APP token authorization bypassed due to unconfigured APP token");
            }
        }

        context.Succeed(requirement);

        return Task.CompletedTask;
    }
}
