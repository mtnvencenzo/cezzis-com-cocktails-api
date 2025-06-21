namespace Cocktails.Api.Domain.Common;

using MediatR;

public interface IEventEmitter<IEvent>
{
    void AddDomainEvent(IEvent eventItem);

    void RemoveDomainEvent(IEvent eventItem);

    void ClearDomainEvents();

    IReadOnlyCollection<IEvent> DomainEvents { get; }
}
