﻿namespace Cocktails.Api.Application.Concerns.Health.Queries;

using global::Cocktails.Api.Application.Concerns.Health.Models;
using global::Cocktails.Api.Domain.Aggregates.HealthAggregate;
using System.Reflection;

public class HealthQueries(IHealthRepository healthRepository) : IHealthQueries
{
    public PingRs GetPing()
    {
        var item = healthRepository.GetServerInfo();

        return new PingRs
        (
            Is64BitOperatingSystem: item.Is64BitOperatingSystem,
            Is64BitProcess: item.Is64BitProcess,
            MachineName: item.MachineName,
            OSVersion: item.OSVersion,
            WorkingSet: item.WorkingSet,
            ProcessorCount: item.ProcessorCount,
            Version: item.Version
        );
    }

    public VersionRs GetVersion()
    {
        var version = Assembly.GetExecutingAssembly()
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
            .InformationalVersion;

        return new VersionRs(version ?? "0.0.0");
    }
}
