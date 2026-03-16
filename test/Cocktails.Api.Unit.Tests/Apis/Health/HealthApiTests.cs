namespace Cocktails.Api.Unit.Tests.Apis.Health;

using FluentAssertions;
using global::Cocktails.Api.Apis.Health;
using global::Cocktails.Api.Application.Concerns.Health;
using global::Cocktails.Api.Application.Concerns.Health.Models;
using global::Cocktails.Api.Domain.Config;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using System.Net;
using Xunit;

public class HealthApiTests : ServiceTestBase
{
    [Fact]
    public void GetPing_ReturnsOkResult()
    {
        // arrange
        var sp = this.SetupEnvironment();
        var services = GetAsParameterServices<HealthServices>(sp);

        // act
        var response = HealthApi.GetPing(services) as Ok<PingRs>;
        var result = response?.Value;

        // assert
        result.Should().NotBeNull();
        result.MachineName.Should().Be(Environment.MachineName);
        result.Is64BitOperatingSystem.Should().Be(Environment.Is64BitOperatingSystem);
        result.Is64BitProcess.Should().Be(Environment.Is64BitProcess);
        result.OSVersion.Should().Be(Environment.OSVersion.ToString());
        result.ProcessorCount.Should().Be(Environment.ProcessorCount);
        result.Version.Should().Be(Environment.Version.ToString());
        result.WorkingSet.Should().BeGreaterThan(0);
    }

    [Fact]
    public void GetLiveness_ReturnsHealthy()
    {
        // arrange
        var sp = this.SetupEnvironment();
        var services = GetAsParameterServices<HealthServices>(sp);

        // act
        var response = HealthApi.GetLiveness(services) as Ok<HealthCheckRs>;
        var result = response?.Value;

        // assert
        result.Should().NotBeNull();
        result.Status.Should().Be("healthy");
        result.Output.Should().Be("API is running");
        result.Version.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task GetReadiness_WhenDaprAndCosmosHealthy_ReturnsOk()
    {
        // arrange
        var mockHandler = new MockHttpMessageHandler(new HttpResponseMessage(HttpStatusCode.NoContent));
        var httpClient = new HttpClient(mockHandler);

        this.httpClientFactoryMock
            .Setup(f => f.CreateClient(HealthCheckConstants.DaprHealthCheckClientName))
            .Returns(httpClient);

        var sp = this.SetupEnvironment(services =>
        {
            services.Configure<DaprConfig>(c => c.HttpEndpoint = "http://localhost:3500");
        });
        var services = GetAsParameterServices<HealthServices>(sp);

        // act
        var response = await HealthApi.GetReadiness(services);
        var okResult = response.Result as Ok<HealthCheckRs>;
        var jsonResult = response.Result as JsonHttpResult<HealthCheckRs>;

        // assert - should be Ok, not 503
        jsonResult.Should().BeNull("expected healthy 200 response but got 503: {0}", jsonResult?.StatusCode);
        okResult.Should().NotBeNull();
        var value = okResult.Value;
        value.Status.Should().Be("healthy");
        value.Details.Should().ContainKey("dapr").WhoseValue.Should().Be("healthy");
        value.Details.Should().ContainKey("cosmosdb").WhoseValue.Should().Be("healthy");
    }

    [Fact]
    public async Task GetReadiness_WhenDaprUnhealthy_Returns503()
    {
        // arrange
        var mockHandler = new MockHttpMessageHandler(new HttpResponseMessage(HttpStatusCode.InternalServerError));
        var httpClient = new HttpClient(mockHandler);

        this.httpClientFactoryMock
            .Setup(f => f.CreateClient(HealthCheckConstants.DaprHealthCheckClientName))
            .Returns(httpClient);

        var sp = this.SetupEnvironment(services =>
        {
            services.Configure<DaprConfig>(c => c.HttpEndpoint = "http://localhost:3500");
        });
        var services = GetAsParameterServices<HealthServices>(sp);

        // act
        var response = await HealthApi.GetReadiness(services);
        var result = response.Result as JsonHttpResult<HealthCheckRs>;

        // assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(StatusCodes.Status503ServiceUnavailable);
    }

    [Fact]
    public async Task GetReadiness_WhenDaprTimesOut_Returns503()
    {
        // arrange
        var mockHandler = new MockHttpMessageHandler(new TaskCanceledException("Simulated timeout"));
        var httpClient = new HttpClient(mockHandler);

        this.httpClientFactoryMock
            .Setup(f => f.CreateClient(HealthCheckConstants.DaprHealthCheckClientName))
            .Returns(httpClient);

        var sp = this.SetupEnvironment(services =>
        {
            services.Configure<DaprConfig>(c => c.HttpEndpoint = "http://localhost:3500");
        });
        var services = GetAsParameterServices<HealthServices>(sp);

        // act
        var response = await HealthApi.GetReadiness(services);
        var result = response.Result as JsonHttpResult<HealthCheckRs>;

        // assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(StatusCodes.Status503ServiceUnavailable);
    }

    private class MockHttpMessageHandler : HttpMessageHandler
    {
        private readonly HttpResponseMessage response;
        private readonly Exception exception;

        public MockHttpMessageHandler(HttpResponseMessage response)
        {
            this.response = response;
        }

        public MockHttpMessageHandler(Exception exception)
        {
            this.exception = exception;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (this.exception != null)
            {
                throw this.exception;
            }

            return Task.FromResult(this.response);
        }
    }
}
