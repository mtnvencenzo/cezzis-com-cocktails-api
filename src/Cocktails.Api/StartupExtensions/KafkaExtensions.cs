namespace Cocktails.Api.StartupExtensions;

using Cocktails.Api.Domain.Config;
using Cocktails.Api.Infrastructure.Services;
using Confluent.Kafka;
using Confluent.Kafka.Extensions.Diagnostics;
using Microsoft.Extensions.Options;

internal static class KafkaExtensions
{
    internal static IServiceCollection AddKafka(this IServiceCollection services)
    {
        services.AddTransient<ICocktailPublisher, KafkaCocktailPublisher>();
        services.AddScoped<KafkaInitializer>();

        services.AddSingleton((sp) =>
        {
            var kafkaConfig = sp.GetRequiredService<IOptions<KafkaConfig>>().Value;

            var producerConfig = new ProducerConfig
            {
                BootstrapServers = kafkaConfig.BootstrapServers,
                SslCaLocation = !string.IsNullOrWhiteSpace(kafkaConfig.SslCaLocation)
                    ? kafkaConfig.SslCaLocation
                    : null,
                SecurityProtocol = !string.IsNullOrWhiteSpace(kafkaConfig.SslCaLocation)
                    ? SecurityProtocol.Ssl
                    : SecurityProtocol.Plaintext,
                SslEndpointIdentificationAlgorithm = SslEndpointIdentificationAlgorithm.None,
                Acks = Acks.All,
                EnableIdempotence = true,
                MessageSendMaxRetries = 3,
                RetryBackoffMs = 1000,
                LingerMs = 5,
                BatchSize = 32 * 1024, // 32 KB
            };

            // https://github.com/confluentinc/confluent-kafka-dotnet/issues/1482

            // https://docs.azure.cn/en-us/event-hubs/event-hubs-for-kafka-ecosystem-overview

            // var config = new ProducerConfig
            // {
            //     BootstrapServers = "ehNamespace.servicebus.windows.net:9093",
            //     SslCaLocation = !string.IsNullOrWhiteSpace(kafkaConfig.SslCaLocation)
            //         ? kafkaConfig.SslCaLocation
            //         : null,
            //     SecurityProtocol = !string.IsNullOrWhiteSpace(kafkaConfig.SslCaLocation)
            //         ? SecurityProtocol.SaslSsl
            //         : SecurityProtocol.Plaintext,
            //     SaslMechanism = SaslMechanism.Plain,
            //     SaslUsername = "$ConnectionString",
            //     SaslPassword = "Endpoint=sb://ehNamespace.servicebus.windows.net/;SharedAccessKeyName=accessKeyName;SharedAccessKey=accessKey",
            //     Debug = "security,broker,protocol"
            // };
            var logger = sp.GetRequiredService<ILoggerFactory>().CreateLogger("KafkaProducer");

            return new ProducerBuilder<Null, string>(producerConfig)
                .SetErrorHandler((_, e) => logger.LogError("Kafka producer error: {Reason} (IsFatal={IsFatal})", e.Reason, e.IsFatal))
                .BuildWithInstrumentation();
        });

        return services;
    }
}
