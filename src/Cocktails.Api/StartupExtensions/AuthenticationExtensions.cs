namespace Cocktails.Api.StartupExtensions;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;

internal static class AuthenticationExtensions
{
    internal static IServiceCollection AddDefaultAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        // Adds Microsoft Identity platform (Azure Entra Ext Id) support to protect this Api
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddMicrosoftIdentityWebApi(
                (jwtOptions) =>
                {
                    configuration.Bind("EntraCIAM", jwtOptions);
                    jwtOptions.TokenValidationParameters.NameClaimType = "name";
                },
                (identityOptions) =>
                {
                    configuration.Bind("EntraCIAM", identityOptions);
                    identityOptions.WithSpaAuthCode = true;
                });

        return services;
    }
}
