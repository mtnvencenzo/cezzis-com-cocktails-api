﻿namespace Cocktails.Api.StartupExtensions;

using Cocktails.Api.Domain.Config;
using Microsoft.Extensions.Options;
using Confluent.Kafka;
using Cocktails.Api.Infrastructure.Services;

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
            };

            // https://github.com/confluentinc/confluent-kafka-dotnet/issues/1482
            //
            // https://docs.azure.cn/en-us/event-hubs/event-hubs-for-kafka-ecosystem-overview
            //
            // var config = new ProducerConfig
            // {
            //     BootstrapServers = "ehNamespace.servicebus.windows.net:9093",
            //     SecurityProtocol = SecurityProtocol.SaslSsl,
            //     SaslMechanism = SaslMechanism.Plain,
            //     SaslUsername = "$ConnectionString",
            //     SaslPassword = "Endpoint=sb://ehNamespace.servicebus.windows.net/;SharedAccessKeyName=accessKeyName;SharedAccessKey=accessKey",
            //     SslCaLocation = "cacert.pem",
            //     Debug = "security,broker,protocol"
            // };
            var logger = sp.GetRequiredService<ILoggerFactory>().CreateLogger("KafkaProducer");

            return new ProducerBuilder<Null, string>(producerConfig)
                .SetErrorHandler((_, e) => logger.LogError("Kafka producer error: {Reason} (IsFatal={IsFatal})", e.Reason, e.IsFatal))
                .Build();
        });

        return services;
    }
}
