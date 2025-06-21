namespace Cocktails.Api.StartupExtensions;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;

internal static class AuthenticationExtensions
{
    internal static IServiceCollection AddDefaultAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        // Adds Microsoft Identity platform (Azure AD B2C) support to protect this Api
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddMicrosoftIdentityWebApi(
                (jwtOptions) =>
                {
                    configuration.Bind("AzureAdB2C", jwtOptions);
                    jwtOptions.TokenValidationParameters.NameClaimType = "name";
                },
                (identityOptions) =>
                {
                    configuration.Bind("AzureAdB2C", identityOptions);
                    identityOptions.WithSpaAuthCode = true;
                });

        return services;
    }
}
