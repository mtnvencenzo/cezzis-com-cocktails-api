namespace Cocktails.Api.Infrastructure.Services;

using Cezzi.Applications;
using Cocktails.Api.Domain;
using Cocktails.Api.Domain.Config;
using Cocktails.Api.Domain.Services;
using Dapr.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public class DaprEventBus(
    IOptions<PubSubConfig> pubSubConfig,
    DaprClient daprClient,
    ILogger<DaprEventBus> logger) : IEventBus
{
    private readonly PubSubConfig pubSubConfig = pubSubConfig?.Value ?? throw new ArgumentNullException(nameof(pubSubConfig));
    private readonly DaprClient daprClient = daprClient ?? throw new ArgumentNullException(nameof(daprClient));
    private readonly ILogger logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public async Task PublishAsync<T>(
        T @event,
        string messageLabel,
        string configName,
        string topicName,
        string contentType = null,
        CancellationToken cancellationToken = default) where T : IIntegrationEvent
    {
        Guard.NotDefault(@event, nameof(@event));
        Guard.NotNullOrWhiteSpace(messageLabel, nameof(messageLabel));
        Guard.NotNullOrWhiteSpace(configName, nameof(configName));
        Guard.NotNullOrWhiteSpace(topicName, nameof(topicName));

        using var logScope = this.logger.BeginScope(new Dictionary<string, object>
        {
            { Monikers.ServiceBus.MsgId, messageLabel },
            { Monikers.ServiceBus.MsgSubject, messageLabel },
            { Monikers.ServiceBus.MsgCorrelationId, @event.CorrelationId },
            { Monikers.ServiceBus.Topic, topicName }
        });

        await this.daprClient.PublishEventAsync(
            pubsubName: configName,
            topicName: topicName,
            data: @event,
            cancellationToken: cancellationToken,
            metadata: new Dictionary<string, string>
            {
                { "CorrelationId", @event.CorrelationId },
                { "ContentType", contentType },
                { "Label", messageLabel }
            });

        this.logger.LogInformation("Message published {Label}", messageLabel);
    }
}
