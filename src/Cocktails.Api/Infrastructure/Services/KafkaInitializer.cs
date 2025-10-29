using Cocktails.Api.Domain.Config;
using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Microsoft.Extensions.Options;

public class KafkaInitializer(
    IOptions<KafkaConfig> kafkaConfig,
    ILogger<KafkaInitializer> logger)
{
    public async Task InitializeAsync()
    {
        try
        {
            logger.LogInformation("Starting kafka initialization");

            try
            {
                using var adminClient = new AdminClientBuilder(new AdminClientConfig { BootstrapServers = kafkaConfig.Value.BootstrapServers }).Build();

                var metadata = adminClient.GetMetadata(TimeSpan.FromSeconds(10));
                logger.LogInformation("Connected to Kafka cluster: {ClusterId}", metadata.OriginatingBrokerId);

                if (!metadata.Topics.Any(t => t.Topic == kafkaConfig.Value.CocktailsTopic))
                {
                    logger.LogInformation("Topic {Topic} does not exist and will be created.", kafkaConfig.Value.CocktailsTopic);
                    await adminClient.CreateTopicsAsync(
                    [
                        new() { Name = kafkaConfig.Value.CocktailsTopic, ReplicationFactor = -1, NumPartitions = 4 }
                    ]);

                    logger.LogInformation("Topic {Topic} created successfully.", kafkaConfig.Value.CocktailsTopic);
                }
                else
                {
                    logger.LogInformation("Topic {Topic} already exists.", kafkaConfig.Value.CocktailsTopic);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error creating topic: {Message}", ex.Message);
                throw;
            }

            logger.LogInformation("Kafka initialization completed successfully.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during Kafka initialization");
            throw;
        }
    }
}