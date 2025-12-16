namespace Cocktails.Api.Infrastructure.Services;

using Cezzi.Applications;
using Cocktails.Api.Domain;
using Cocktails.Api.Domain.Services;
using Dapr.Client;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public class DaprEventBus(
    DaprClient daprClient,
    ILogger<DaprEventBus> logger) : IEventBus
{
    private readonly DaprClient daprClient = daprClient ?? throw new ArgumentNullException(nameof(daprClient));
    private readonly ILogger logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public async Task PublishAsync<T>(
        T @event,
        string messageLabel,
        string configName,
        string topicName,
        string contentType = null,
        string correlationId = null,
        CancellationToken cancellationToken = default) where T : class
    {
        Guard.NotDefault(@event, nameof(@event));
        Guard.NotNullOrWhiteSpace(messageLabel, nameof(messageLabel));
        Guard.NotNullOrWhiteSpace(configName, nameof(configName));
        Guard.NotNullOrWhiteSpace(topicName, nameof(topicName));

        var useableCorrelationId = correlationId ?? Guid.NewGuid().ToString();

        using var logScope = this.logger.BeginScope(new Dictionary<string, object>
        {
            { Monikers.ServiceBus.MsgId, messageLabel },
            { Monikers.ServiceBus.MsgSubject, messageLabel },
            { Monikers.ServiceBus.MsgCorrelationId, useableCorrelationId },
            { Monikers.ServiceBus.Topic, topicName }
        });

        await this.daprClient.PublishEventAsync(
            pubsubName: configName,
            topicName: topicName,
            data: @event,
            cancellationToken: cancellationToken,
            metadata: new Dictionary<string, string>
            {
                { "CorrelationId", useableCorrelationId },
                { "ContentType", contentType },
                { "Label", messageLabel }
            });

        this.logger.LogInformation("Message published {Label}", messageLabel);
    }
}
