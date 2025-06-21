namespace Cocktails.Api.Infrastructure.Services;

using Cezzi.Applications;
using Microsoft.AspNetCore.Http;

public class RequestHeaderAccessor(IHttpContextAccessor httpContextAccessor) : IRequestHeaderAccessor
{
    public string GetHeaderValue(string headerName)
    {
        Guard.NotNullOrWhiteSpace(headerName, nameof(headerName));

        var headers = httpContextAccessor.HttpContext?.Request?.Headers;

        Guard.NotNull(headers, nameof(headers));

        return headers.TryGetValue(headerName, out var value)
            ? value.ToString()
            : string.Empty;
    }
}