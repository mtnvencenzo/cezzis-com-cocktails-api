namespace Cocktails.Api.Domain.Config;

public class DaprServiceBusPublisherConfig
{
    public bool SkipPublish { get; set; }

    public required string DaprBuildingBlock { get; set; }

    public required string TopicName { get; set; }

    public required string Subject { get; set; }
}
