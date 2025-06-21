namespace Cocktails.Api.Unit.Tests.Apis.Health;

using FluentAssertions;
using global::Cocktails.Api.Apis.Health;
using global::Cocktails.Api.Application.Concerns.Health;
using global::Cocktails.Api.Application.Concerns.Health.Models;
using Microsoft.AspNetCore.Http.HttpResults;
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
}
