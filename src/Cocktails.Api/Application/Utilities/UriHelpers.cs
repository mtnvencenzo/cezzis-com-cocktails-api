namespace Cocktails.Api.Application.Utilities;

public static class UriHelpers
{
    public static Uri ReplaceHostName(this Uri uri, string hostName)
    {
        if (string.IsNullOrWhiteSpace(hostName))
        {
            return uri;
        }

        var builder = new UriBuilder(uri);
        builder.Host = hostName;
        return builder.Uri;
    }
}
