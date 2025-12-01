namespace Cocktails.Api.Apis.Health;

using Cocktails.Api.Application.Concerns.Health.Queries;

/// <summary>
/// 
/// </summary>
/// <param name="queries"></param>
public class HealthServices(IHealthQueries queries, ILogger<HealthServices> logger)
{
    /// <summary></summary>
    public IHealthQueries Queries { get; } = queries ?? throw new ArgumentNullException(nameof(queries));

    public ILogger<HealthServices> Logger { get; } = logger;
}