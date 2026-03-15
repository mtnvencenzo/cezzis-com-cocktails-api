namespace Cocktails.Api.Application.Concerns.Health.Queries;

using global::Cocktails.Api.Application.Concerns.Health.Models;
using global::Cocktails.Api.Domain.Aggregates.HealthAggregate;
using global::Cocktails.Api.Domain.Config;
using global::Cocktails.Api.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Reflection;

public class HealthQueries(
    IHealthRepository healthRepository,
    IHttpClientFactory httpClientFactory,
    IOptions<DaprConfig> daprConfig,
    CocktailDbContext dbContext,
    ILogger<HealthQueries> logger) : IHealthQueries
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
        try
        {
            var opts = daprConfig.Value;
            using var client = httpClientFactory.CreateClient();

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
            }
            else
            {
                details["dapr"] = $"unhealthy (status {(int)response.StatusCode})";
                overallHealthy = false;
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Dapr health check failed");
            details["dapr"] = "unhealthy";
            overallHealthy = false;
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
