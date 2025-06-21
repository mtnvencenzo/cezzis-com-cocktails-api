namespace Cocktails.Api.StartupExtensions;

internal static class CorsExtensions
{
    internal static IServiceCollection AddHttpCors(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("origin-policy", corsBuilder =>
            {
                corsBuilder.WithOrigins(configuration["AllowedOrigins"].Split(","))
                    .AllowCredentials()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .WithExposedHeaders("Authorization")
                    .SetPreflightMaxAge(TimeSpan.FromSeconds(600));
            });
        });

        return services;
    }
}
