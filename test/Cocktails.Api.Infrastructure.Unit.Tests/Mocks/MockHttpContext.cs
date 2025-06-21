namespace Cocktails.Api.Infrastructure.Unit.Tests.Mocks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Claims;
using System.Threading;

/// <summary>
/// 
/// </summary>
public class MockHttpContext : HttpContext, IDisposable
{
    private readonly MockHttpRequest httpRequest;
    private readonly MockHttpResponse httpResponse;
    private readonly ConnectionInfo connectionInfo;
    private readonly FeatureCollection features;

    public MockHttpContext(IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
    {
        this.RequestAborted = cancellationToken;
        this.TraceIdentifier = $"UT-{Guid.NewGuid()}";
        this.RequestServices = serviceProvider;
        this.Items = new Dictionary<object, object>();

        this.features = new FeatureCollection();
        this.httpRequest = new MockHttpRequest(this)
        {
            Host = new HostString("localhost", 1),
            IsHttps = true,
            Method = "POST",
            Protocol = "HTTPS",
            Path = "/payments",
            PathBase = new PathString("/payments"),
            QueryString = new QueryString(),
            Scheme = "HTTP/1.1",
            Form = new FormCollection([]),
            Cookies = new MockRequestCookieCollection(),
            Query = new QueryCollection()
        };

        this.connectionInfo = BuildConnectionInfo();
        this.httpResponse = new MockHttpResponse(this);
    }

    public MockHttpContext(
        IServiceProvider serviceProvider,
        string path,
        string method,
        CancellationToken cancellationToken = default)
    {
        this.RequestAborted = cancellationToken;
        this.TraceIdentifier = $"UT-{Guid.NewGuid()}";
        this.RequestServices = serviceProvider;
        this.Items = new Dictionary<object, object>();

        this.features = new FeatureCollection();
        this.httpRequest = new MockHttpRequest(this)
        {
            Host = new HostString("localhost", 5001),
            IsHttps = true,
            Method = method,
            Protocol = "HTTP/1.1",
            Path = path,
            RouteValues = [],
            PathBase = new PathString(path),
            QueryString = new QueryString(),
            Scheme = "https",
            Form = new FormCollection([]),
            Cookies = new MockRequestCookieCollection(),
            Query = new QueryCollection()
        };

        this.connectionInfo = BuildConnectionInfo();
        this.httpResponse = new MockHttpResponse(this);
    }

    public override CancellationToken RequestAborted { get; set; }

    public override HttpRequest Request => this.httpRequest;

    public override HttpResponse Response => this.httpResponse;

    public override IServiceProvider RequestServices { get; set; }

    public override string TraceIdentifier { get; set; }

    public override ClaimsPrincipal User { get; set; }

    public override IDictionary<object, object> Items { get; set; }

    public override ConnectionInfo Connection => this.connectionInfo;

    public override IFeatureCollection Features => this.features;

    public override ISession Session { get; set; }

    public override WebSocketManager WebSockets => throw new NotImplementedException();

    public override void Abort() => this.RequestAborted = new CancellationToken(true);

    private static MockConnectionInfo BuildConnectionInfo()
    {
        return new MockConnectionInfo()
        {
            LocalIpAddress = new IPAddress(0),
            RemoteIpAddress = new IPAddress(0),
            Id = Guid.NewGuid().ToString(),
            LocalPort = 0,
            RemotePort = 0
        };
    }

    public void Dispose()
    {
        this.httpRequest.Dispose();
        this.httpResponse.Dispose();
        GC.SuppressFinalize(this);
    }
}
