namespace Cocktails.Api.Domain.Config;

public class PubSubConfig
{
    public const string SectionName = "PubSub";

    public DaprServiceBusPublisherConfig EmailPublisher { get; set; }

    public DaprServiceBusSubscriberConfig EmailSubscriber { get; set; }

    public DaprServiceBusPublisherConfig AccountPublisher { get; set; }

    public DaprServiceBusSubscriberConfig AccountSubscriber { get; set; }

    public DaprServiceBusSubscriberConfig AccountEmailSubscriber { get; set; }

    public DaprServiceBusPublisherConfig AccountEmailPublisher { get; set; }

    public DaprServiceBusSubscriberConfig AccountPasswordSubscriber { get; set; }

    public DaprServiceBusPublisherConfig AccountPasswordPublisher { get; set; }

    public DaprServiceBusPublisherConfig CocktailRatingPublisher { get; set; }

    public DaprServiceBusSubscriberConfig CocktailRatingSubscriber { get; set; }
}
