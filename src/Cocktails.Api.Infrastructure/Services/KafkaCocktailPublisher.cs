namespace Cocktails.Api.Infrastructure.Services;

using Cocktails.Api.Domain.Aggregates.CocktailAggregate;
using Cocktails.Api.Domain.Config;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

/// <summary>Kafka publisher for cocktails.</summary>
public class KafkaCocktailPublisher(
    IProducer<Null, List<Cocktail>> kafkaProducer,
    IOptions<KafkaConfig> kafkaConfig,
    ILogger<KafkaCocktailPublisher> logger) : ICocktailPublisher
{
    /// <summary>Publishes the next batch of cocktails.</summary>
    /// <param name="cocktails">The cocktails to publish.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    public async Task PublishNextBatchAsync(
        List<Cocktail> cocktails,
        CancellationToken cancellationToken = default)
    {
        if (cocktails.Count == 0)
        {
            logger.LogInformation("No cocktails to publish.");
            return;
        }

        var message = new Message<Null, List<Cocktail>>
        {
            Key = null,
            Value = cocktails
        };

        await kafkaProducer.ProduceAsync(kafkaConfig.Value.CocktailsTopic, message, cancellationToken);

        logger.LogInformation("Published {CocktailCount} cocktails to Kafka topic", cocktails.Count);
    }
}