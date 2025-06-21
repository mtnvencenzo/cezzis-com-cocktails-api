namespace Cocktails.Api.Unit.Tests.Mocks;

using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.IO.Pipelines;
using System.Threading.Tasks;

/// <summary>
/// 
/// </summary>
public class MockHttpResponse(HttpContext context) : HttpResponse, IDisposable
{
    private readonly HeaderDictionary headers = [];
    private readonly HttpContext httpContext = context;
    private readonly IResponseCookies cookies = new MockResponseCookieCollection();
    private Stream stream = new MemoryStream();

    public override IHeaderDictionary Headers => this.headers;

    public override IResponseCookies Cookies => this.cookies;

    public override Stream Body { get => this.stream; set => this.stream = value; }

    public override long? ContentLength { get; set; }

    public override string ContentType { get; set; }

    public override int StatusCode { get; set; }

    public override bool HasStarted => false;

    public override HttpContext HttpContext => this.httpContext;

    public override PipeWriter BodyWriter => PipeWriter.Create(this.stream);

    public void Dispose() => this.stream?.Dispose();

    public override void OnCompleted(Func<object, Task> callback, object state)
    {
    }

    public override void OnStarting(Func<object, Task> callback, object state)
    {
    }

    public override void Redirect(string location, bool permanent)
    {
    }
}
