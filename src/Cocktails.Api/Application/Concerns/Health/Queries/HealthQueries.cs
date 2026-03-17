namespace Cocktails.Api.Application.Concerns.Health.Queries;

using global::Cocktails.Api.Application.Concerns.Health;
using global::Cocktails.Api.Application.Concerns.Health.Models;
using global::Cocktails.Api.Domain.Aggregates.HealthAggregate;
using global::Cocktails.Api.Domain.Config;
using global::Cocktails.Api.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Reflection;

public class HealthQueries(
    IHealthRepository healthRepository,
    IHttpClientFactory httpClientFactory,
    IOptions<DaprConfig> daprConfig,
    CocktailDbContext dbContext,
    ILogger<HealthQueries> logger) : IHealthQueries
{
    private static readonly long StartTimestamp = Stopwatch.GetTimestamp();
    internal static TimeSpan DaprGracePeriod = TimeSpan.FromSeconds(120); // 2 minutes

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

    public HealthCheckRs GetLiveness()
    {
        var version = Assembly.GetExecutingAssembly()
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
            .InformationalVersion;

        return new HealthCheckRs(
            Status: "healthy",
            Version: version ?? "0.0.0",
            Output: "API is running");
    }

    public async Task<HealthCheckRs> GetReadinessAsync()
    {
        var details = new Dictionary<string, string>();
        var overallHealthy = true;

        // Check Dapr outbound component health
        var daprHealthy = false;
        try
        {
            var opts = daprConfig.Value;
            using var client = httpClientFactory.CreateClient(HealthCheckConstants.DaprHealthCheckClientName);

            if (!string.IsNullOrWhiteSpace(opts.DaprAppToken))
            {
                client.DefaultRequestHeaders.Add("dapr-api-token", opts.DaprAppToken);
            }

            var response = await client.GetAsync(
                $"{opts.HttpEndpoint}/v1.0/healthz/outbound",
                new CancellationTokenSource(TimeSpan.FromSeconds(5)).Token);

            if ((int)response.StatusCode == 204)
            {
                details["dapr"] = "healthy";
                daprHealthy = true;
            }
            else
            {
                details["dapr"] = $"unhealthy (status {(int)response.StatusCode})";
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Dapr health check failed");
            details["dapr"] = "unhealthy";
        }

        // During the startup grace period, treat Dapr failures as degraded (not unhealthy)
        // to allow the pod to become Ready while the Dapr sidecar is still initializing
        if (!daprHealthy)
        {
            var elapsed = Stopwatch.GetElapsedTime(StartTimestamp);
            if (elapsed < DaprGracePeriod)
            {
                logger.LogWarning("Dapr is not ready but within startup grace period ({Elapsed:F0}s / {Grace:F0}s). Reporting as degraded.",
                    elapsed.TotalSeconds, DaprGracePeriod.TotalSeconds);
                details["dapr"] = $"degraded (starting, {elapsed.TotalSeconds:F0}s elapsed)";
            }
            else
            {
                overallHealthy = false;
            }
        }

        // Check CosmosDB connectivity
        // Note: The Cosmos EF Core provider does not support CanConnectAsync(),
        // so we run a lightweight query instead.
        try
        {
            _ = await dbContext.Cocktails.Select(c => c.Id).FirstOrDefaultAsync();
            details["cosmosdb"] = "healthy";
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "CosmosDB health check failed");
            details["cosmosdb"] = "unhealthy";
            overallHealthy = false;
        }

        var version = Assembly.GetExecutingAssembly()
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
            .InformationalVersion;

        return new HealthCheckRs(
            Status: overallHealthy ? "healthy" : "unhealthy",
            Version: version ?? "0.0.0",
            Output: overallHealthy ? "All dependencies are reachable" : "One or more dependencies are unreachable",
            Details: details);
    }
}
