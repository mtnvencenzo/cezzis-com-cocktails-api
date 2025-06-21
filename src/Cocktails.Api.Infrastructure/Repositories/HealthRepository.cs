namespace Cocktails.Api.Infrastructure.Repositories;

using Cocktails.Api.Domain.Aggregates.HealthAggregate;
using Cocktails.Api.Domain.Common;

public class HealthRepository() : IHealthRepository
{
    public IUnitOfWork UnitOfWork => null;

    public ServerInfo GetServerInfo() => new();
}
