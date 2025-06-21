namespace Cocktails.Api.Domain.Config;

public class DaprServiceBusPublisherConfig
{
    public bool SkipPublish { get; set; }

    public string DaprBuildingBlock { get; set; }

    public string TopicName { get; set; }

    public string Subject { get; set; }
}
