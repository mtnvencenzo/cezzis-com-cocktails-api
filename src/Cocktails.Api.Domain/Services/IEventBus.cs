namespace Cocktails.Api.Domain.Services;
using System.Threading;
using System.Threading.Tasks;

public interface IEventBus
{
    Task PublishAsync<T>(
        T @event,
        string messageLabel,
        string configName,
        string topicName,
        string contentType = null,
        CancellationToken cancellationToken = default) where T : IIntegrationEvent;
}
