namespace Cocktails.Api.Application.Utilities;

public static class RequestExtensions
{
    public static string GetIPAddress(this HttpContext httpContext)
    {
        var ipAddress = httpContext.GetServerVariable("HTTP_X_FORWARDED_FOR")?.Trim();

        if (!string.IsNullOrWhiteSpace(ipAddress))
        {
            var addresses = ipAddress.Split(',');

            if (addresses.Length != 0)
            {
                return addresses[0].Trim();
            }
        }

        return httpContext.GetServerVariable("REMOTE_ADDR")?.Trim();
    }
}
