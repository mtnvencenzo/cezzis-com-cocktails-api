namespace Cocktails.Api.Domain.Config;

public class KafkaConfig
{
    public const string SectionName = "Kafka";

    public required string CocktailsTopic { get; set; }

    public required string BootstrapServers { get; set; }

    public string SslCaLocation { get; set; }
}
