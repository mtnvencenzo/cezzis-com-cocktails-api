namespace Cocktails.Api.Application.Concerns.Health.Queries;

using global::Cocktails.Api.Application.Concerns.Health.Models;

public interface IHealthQueries
{
    PingRs GetPing();

    VersionRs GetVersion();
}
