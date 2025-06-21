namespace Cocktails.Api.StartupExtensions;

using Cezzi.Smtp;
using Cocktails.Api.Infrastructure.Services;
using System.Net.Http;
using System.Runtime.CompilerServices;

internal static class ZohoEmailExtensions
{
    internal static IServiceCollection AddZohoEmail(this IServiceCollection services)
    {
        services.AddTransient<IZohoEmailClient, ZohoEmailClient>();
        services.AddTransient<ISmtpClientFactory, SmtpClientFactory>();

        return services;
    }
}
