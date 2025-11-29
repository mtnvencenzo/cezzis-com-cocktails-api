namespace Cocktails.Api.Domain.Config;

public class PubSubConfig
{
    public const string SectionName = "PubSub";

    public required DaprServiceBusPublisherConfig EmailPublisher { get; set; }

    public required DaprServiceBusSubscriberConfig EmailSubscriber { get; set; }

    public required DaprServiceBusPublisherConfig AccountPublisher { get; set; }

    public required DaprServiceBusSubscriberConfig AccountSubscriber { get; set; }

    public required DaprServiceBusSubscriberConfig AccountEmailSubscriber { get; set; }

    public required DaprServiceBusPublisherConfig AccountEmailPublisher { get; set; }

    public required DaprServiceBusSubscriberConfig AccountPasswordSubscriber { get; set; }

    public required DaprServiceBusPublisherConfig AccountPasswordPublisher { get; set; }

    public required DaprServiceBusPublisherConfig CocktailRatingPublisher { get; set; }

    public required DaprServiceBusSubscriberConfig CocktailRatingSubscriber { get; set; }
}
