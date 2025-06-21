namespace Cocktails.Api.Infrastructure.Unit.Tests.Mocks;

using Microsoft.AspNetCore.Http;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

public class MockConnectionInfo : ConnectionInfo
{
    private X509Certificate2 certificate;
    private string id;
    private IPAddress localIpAddress;
    private IPAddress remoteIpAddress;
    private int localPort;
    private int remotePort;

    public override X509Certificate2 ClientCertificate { get => this.certificate; set => this.certificate = value; }

    public override string Id { get => this.id; set => this.id = value; }

    public override IPAddress LocalIpAddress { get => this.localIpAddress; set => this.localIpAddress = value; }

    public override int LocalPort { get => this.localPort; set => this.localPort = value; }

    public override IPAddress RemoteIpAddress { get => this.remoteIpAddress; set => this.remoteIpAddress = value; }

    public override int RemotePort { get => this.remotePort; set => this.remotePort = value; }

    public override Task<X509Certificate2> GetClientCertificateAsync(CancellationToken cancellationToken = default) => Task.FromResult(this.certificate);
}
