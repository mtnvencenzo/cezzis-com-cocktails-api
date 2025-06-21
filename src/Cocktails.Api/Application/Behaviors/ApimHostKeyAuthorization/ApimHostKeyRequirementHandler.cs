namespace Cocktails.Api.Application.Behaviors.ApimHostKeyAuthorization;

using Cocktails.Api.Domain.Config;
using Cocktails.Api.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

public class ApimHostKeyRequirementHandler(
    IOptions<CocktailsApiConfig> cocktailsApiConfig,
    IRequestHeaderAccessor requestHeaderAccessor,
    ILogger<ApimHostKeyRequirementHandler> logger) : AuthorizationHandler<ApimHostKeyRequirement>
{
    public const string ApimHostKeyHeaderName = "X-Apim-Host-Key";

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ApimHostKeyRequirement requirement)
    {
        if (!string.IsNullOrWhiteSpace(cocktailsApiConfig.Value.ApimHostKey))
        {
            var headerValue = requestHeaderAccessor.GetHeaderValue(ApimHostKeyHeaderName);

            if (string.IsNullOrWhiteSpace(headerValue) || headerValue != cocktailsApiConfig.Value.ApimHostKey)
            {
                logger.LogWarning("Host key authorization failed due to invalid supplied host key");

                context.Fail();
            }
        }
        else
        {
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") != "local")
            {
                logger.LogWarning("Host key authorization bypassed due to unconfigured host key");
            }
        }

        context.Succeed(requirement);

        return Task.CompletedTask;
    }
}
