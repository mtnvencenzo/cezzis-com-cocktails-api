namespace Cocktails.Api.StartupExtensions;

using Cocktails.Api.Application.Behaviors.ApimHostKeyAuthorization;
using Microsoft.AspNetCore.Authorization;

internal static class AuthorizationExtensions
{
    internal static IServiceCollection AddDefaultAuthorization(this IServiceCollection services)
    {
        // Adding apim host key authorization.  
        services.AddTransient<ApimHostKeyRequirementHandler>();
        services.AddTransient<IAuthorizationHandler, ApimHostKeyRequirementHandler>();

        services.AddAuthorizationBuilder()
            .AddPolicy(ApimHostKeyRequirement.PolicyName, (o) =>
            {
                o.AddRequirements(new ApimHostKeyRequirement());
            });

        return services;
    }
}
