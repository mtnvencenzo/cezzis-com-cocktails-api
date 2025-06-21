namespace Cocktails.Api.Infrastructure.Unit.Tests.Mocks;

using Microsoft.AspNetCore.Http;
using System.Collections;
using System.Collections.Generic;

public class MockRequestCookieCollection : IRequestCookieCollection
{
    private readonly Dictionary<string, string> cookies;

    public MockRequestCookieCollection()
    {
        this.cookies = [];
    }

    public string this[string key] => this.cookies[key];

    public int Count => this.cookies.Count;

    public ICollection<string> Keys => this.cookies.Keys;

    public bool ContainsKey(string key) => this.cookies.ContainsKey(key);

    public IEnumerator<KeyValuePair<string, string>> GetEnumerator() => this.cookies.GetEnumerator();

    public bool TryGetValue(string key, out string value) => this.cookies.TryGetValue(key, out value);

    IEnumerator IEnumerable.GetEnumerator() => this.cookies.GetEnumerator();
}
