namespace Cocktails.Api.Domain.Config;

public class KafkaConfig
{
    public const string SectionName = "Kafka";

    public string CocktailsTopic { get; set; }

    public string BootstrapServers { get; set; }
}
