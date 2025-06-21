namespace Cocktails.Api.Unit.Tests.Application.Queries.Health.HealthViewModels;

using FluentAssertions;
using global::Cocktails.Api.Application.Concerns.Health;
using global::Cocktails.Api.Application.Concerns.Health.Models;
using Xunit;

public class PingRsTests : ServiceTestBase
{
    [Fact]
    public void MachineName_Should_Return_EnvironmentMachineName()
    {
        // arrange
        var expectedMachineName = Environment.MachineName;

        // act
        var pingRs = new PingRs(MachineName: expectedMachineName);
        var machineName = pingRs.MachineName;

        // assert
        machineName.Should().Be(expectedMachineName);
    }

    [Fact]
    public void Version_Should_Return_EnvironmentVersion()
    {
        // arrange
        var expectedVersion = Environment.Version.ToString();

        // act
        var pingRs = new PingRs(Version: expectedVersion);
        var version = pingRs.Version;

        // assert
        version.Should().Be(expectedVersion);
    }

    [Fact]
    public void Is64BitOperatingSystem_Should_Return_EnvironmentIs64BitOperatingSystem()
    {
        // arrange
        var expectedIs64BitOperatingSystem = Environment.Is64BitOperatingSystem;

        // act
        var pingRs = new PingRs(Is64BitOperatingSystem: expectedIs64BitOperatingSystem);
        var is64BitOperatingSystem = pingRs.Is64BitOperatingSystem;

        // assert
        is64BitOperatingSystem.Should().Be(expectedIs64BitOperatingSystem);
    }

    [Fact]
    public void Is64BitProcess_Should_Return_EnvironmentIs64BitProcess()
    {
        // arrange
        var expectedIs64BitProcess = Environment.Is64BitProcess;

        // act
        var pingRs = new PingRs(Is64BitProcess: expectedIs64BitProcess);
        var is64BitProcess = pingRs.Is64BitProcess;

        // assert
        is64BitProcess.Should().Be(expectedIs64BitProcess);
    }

    [Fact]
    public void ProcessorCount_Should_Return_EnvironmentProcessorCount()
    {
        // arrange
        var expectedProcessorCount = Environment.ProcessorCount;

        // act
        var pingRs = new PingRs(ProcessorCount: expectedProcessorCount);
        var processorCount = pingRs.ProcessorCount;

        // assert
        processorCount.Should().Be(expectedProcessorCount);
    }

    [Fact]
    public void OSVersion_Should_Return_EnvironmentOSVersion()
    {
        // arrange
        var expectedOSVersion = Environment.OSVersion.ToString();

        // act
        var pingRs = new PingRs(OSVersion: expectedOSVersion);
        var osVersion = pingRs.OSVersion;

        // assert
        osVersion.Should().Be(expectedOSVersion);
    }

    [Fact]
    public void WorkingSet_Should_Return_EnvironmentWorkingSet()
    {
        // arrange
        var expectedWorkingSet = Environment.WorkingSet;

        // act
        var pingRs = new PingRs(WorkingSet: expectedWorkingSet);
        var osVersion = pingRs.WorkingSet;

        // assert
        osVersion.Should().Be(expectedWorkingSet);
    }
}
