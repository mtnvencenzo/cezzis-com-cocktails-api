namespace Cocktails.Api.Infrastructure.Unit.Tests.Mocks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

public class MockHttpRequest(HttpContext context) : HttpRequest, IDisposable
{
    private readonly HeaderDictionary headers = [];
    private readonly HttpContext httpContext = context;
    private IRequestCookieCollection cookies = new MockRequestCookieCollection();
    private IFormCollection form = new FormCollection([]);
    private Stream stream = new MemoryStream();

    public override HttpContext HttpContext => this.httpContext;

    public override IHeaderDictionary Headers => this.headers;

    public override Stream Body { get => this.stream; set => this.stream = value; }

    public override long? ContentLength { get; set; }

    public override string ContentType { get; set; }

    public override HostString Host { get; set; }

    public override bool IsHttps { get; set; }

    public override string Method { get; set; }

    public override PathString Path { get; set; }

    public override PathString PathBase { get; set; }

    public override string Protocol { get; set; }

    public override IQueryCollection Query { get; set; }

    public override string Scheme { get; set; }

    public override QueryString QueryString { get; set; }

    public override IRequestCookieCollection Cookies { get => this.cookies; set => this.cookies = value; }

    public override IFormCollection Form { get => this.form; set => this.form = value; }

    public override bool HasFormContentType => false;

    public void Dispose() => this.stream?.Dispose();

    public static MockHttpRequest FromHttpRequestString(HttpContext context, string http)
    {
        var httpLines = http.Split(Environment.NewLine);
        var method = null as string;
        var protocol = null as string;
        var rawuri = null as string;
        var headers = new List<(string name, string value)>();

        // Pull main header line
        foreach (var httpLine in httpLines)
        {
            var line = httpLine.Trim();

            if (!string.IsNullOrWhiteSpace(line))
            {
                method = line[..line.IndexOf(' ')];
                protocol = line[line.LastIndexOf(' ')..];

                rawuri = line[method.Length..];
                rawuri = rawuri[..rawuri.LastIndexOf(protocol)];
                break;
            }
        }

        var inHeaders = false;
        var inBody = false;
        var bodyText = string.Empty;

        for (var i = 0; i < httpLines.Length; i++)
        {
            var httpLine = httpLines[i];
            var line = httpLine.Trim();

            if (!inBody && !inHeaders && !string.IsNullOrWhiteSpace(line))
            {
                inHeaders = true;
                continue;
            }

            if (inHeaders && !inBody)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    inHeaders = false;
                    inBody = true;
                    continue;
                }

                var headerName = line[..line.IndexOf(':')];
                var headerValue = line[(headerName.Length + 1)..];
                headers.Add((headerName.Trim(), headerValue.Trim()));
                continue;
            }

            if (inHeaders && string.IsNullOrWhiteSpace(line))
            {
                inHeaders = false;
                inBody = true;
                continue;
            }

            if (inBody)
            {
                bodyText += httpLine;

                if (i < httpLines.Length - 1)
                {
                    bodyText += Environment.NewLine;
                }
            }
        }

        var uri = new Uri(rawuri.Trim());
        var host = new HostString(uri.Host, uri.Port);
        var path = new PathString(uri.AbsolutePath);
        var pathBase = new PathString(uri.AbsolutePath);
        var queryString = new QueryString(uri.Query);

        var queryStringDict = new Dictionary<string, StringValues>();
        if (!string.IsNullOrWhiteSpace(uri.Query))
        {
            var queryCollection = HttpUtility.ParseQueryString(uri.Query);
            foreach (var key in queryCollection.Keys)
            {
                queryStringDict.Add(key.ToString(), new StringValues(queryCollection.GetValues(key.ToString())));
            }
        }

        var request = new MockHttpRequest(context)
        {
            Host = host,
            IsHttps = rawuri.ToLower().Trim().StartsWith("https://"),
            Method = method.Trim().ToUpper(),
            Protocol = protocol.Trim().ToUpper(),
            Path = path,
            PathBase = pathBase,
            QueryString = queryString,
            Scheme = uri.Scheme,
            Form = new FormCollection([]),
            Cookies = new MockRequestCookieCollection(),
            Query = new QueryCollection(queryStringDict)
        };

        if (!string.IsNullOrWhiteSpace(bodyText))
        {
            request.Body.Seek(0, SeekOrigin.Begin);

            var writer = new StreamWriter(request.Body);
            writer.Write(bodyText);
            writer.Flush();
            request.Body.Position = 0;
            request.Body.Seek(0, SeekOrigin.Begin);

            if (!headers.Any(h => h.name == "Content-Length"))
            {
                headers.Add(("Content-Length", $"{request.Body.Length}"));
            }
        }
        else if (method.ToLower() is "post" or "put" or "patch")
        {
            if (!headers.Any(h => h.name == "Content-Length"))
            {
                headers.Add(("Content-Length", "0"));
            }
        }

        if (headers.Count > 0)
        {
            headers.ForEach(h => request.headers.Add(new KeyValuePair<string, StringValues>(h.name, new StringValues(h.value))));
        }

        if (request.headers.ContainsKey("Content-Type"))
        {
            request.ContentType = request.headers["Content-Type"];
        }

        if (request.headers.ContainsKey("Content-Length"))
        {
            request.ContentLength = int.Parse(request.headers["Content-Length"]);
        }

        return request;
    }

    public override Task<IFormCollection> ReadFormAsync(CancellationToken cancellationToken = default) => throw new NotImplementedException();
}
