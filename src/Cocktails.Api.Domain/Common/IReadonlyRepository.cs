namespace Cocktails.Api.Domain.Common;
using System.Linq;

public interface IReadonlyRepository<T> where T : Entity
{
    IQueryable<T> Items { get; }
}
