namespace Cocktails.Api.Infrastructure.Unit.Tests.Mocks;

using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Net;

public class MockResponseCookieCollection : IResponseCookies
{
    private readonly Dictionary<string, Cookie> cookies;

    public MockResponseCookieCollection()
    {
        this.cookies = [];
    }

    public void Append(string key, string value) => this.cookies.Add(key, new Cookie(key, value));

    public void Append(string key, string value, CookieOptions options) => this.cookies.Add(key, new Cookie(key, value, options.Path, options.Domain));

    public void Delete(string key) => this.cookies.Remove(key);

    public void Delete(string key, CookieOptions options) => this.cookies.Remove(key);
}
