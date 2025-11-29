namespace Cocktails.Api.Infrastructure.Services;

using Cocktails.Api.Application.Concerns.Cocktails.Models;
using Cocktails.Api.Domain.Config;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;

/// <summary>Kafka publisher for cocktails.</summary>
public class KafkaCocktailPublisher(
    IProducer<Null, string> kafkaProducer,
    IOptions<KafkaConfig> kafkaConfig,
    IOptions<Microsoft.AspNetCore.Mvc.JsonOptions> jsonOptions,
    ILogger<KafkaCocktailPublisher> logger) : ICocktailPublisher
{
    /// <summary>Publishes the next batch of cocktails.</summary>
    /// <param name="cocktails">The cocktails to publish.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    public async Task PublishNextBatchAsync(
        List<CocktailModel> cocktails,
        CancellationToken cancellationToken = default)
    {
        if (cocktails is null || cocktails.Count == 0)
        {
            logger.LogInformation("No cocktails to publish.");
            return;
        }

        var payload = JsonSerializer.Serialize(cocktails, jsonOptions.Value.JsonSerializerOptions);
        var message = new Message<Null, string> { Key = null, Value = payload };

        await kafkaProducer.ProduceAsync(
            topic: kafkaConfig.Value.CocktailsTopic,
            message: message,
            cancellationToken: cancellationToken);

        logger.LogInformation("Published {CocktailCount} cocktails to Kafka topic", cocktails.Count);
    }
}