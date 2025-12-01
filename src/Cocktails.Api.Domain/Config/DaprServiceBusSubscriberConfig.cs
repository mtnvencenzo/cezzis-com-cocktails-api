namespace Cocktails.Api.Domain.Config;

public class DaprServiceBusSubscriberConfig
{
    public required string DaprBuildingBlock { get; set; }

    public required string QueueName { get; set; }
}
