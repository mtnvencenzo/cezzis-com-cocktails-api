namespace Cocktails.Api.Domain.Aggregates.HealthAggregate;

using Cocktails.Api.Domain.Common;

public interface IHealthRepository : IRepository<Health>
{
    ServerInfo GetServerInfo();
}
