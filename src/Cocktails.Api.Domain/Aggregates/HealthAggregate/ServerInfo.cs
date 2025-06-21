namespace Cocktails.Api.Domain.Aggregates.HealthAggregate;

using Cocktails.Api.Domain.Common;
using System;
using System.Collections.Generic;

public class ServerInfo : ValueObject
{
    public string MachineName { get; } = Environment.MachineName;

    public string Version { get; } = Environment.Version.ToString();

    public bool Is64BitOperatingSystem { get; } = Environment.Is64BitOperatingSystem;

    public bool Is64BitProcess { get; } = Environment.Is64BitProcess;

    public int ProcessorCount { get; } = Environment.ProcessorCount;

    public string OSVersion { get; } = Environment.OSVersion.ToString();

    public long WorkingSet { get; } = Environment.WorkingSet;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return this.MachineName;
        yield return this.Version;
        yield return this.Is64BitOperatingSystem;
        yield return this.Is64BitProcess;
        yield return this.ProcessorCount;
        yield return this.OSVersion;
        yield return this.WorkingSet;
    }
}
