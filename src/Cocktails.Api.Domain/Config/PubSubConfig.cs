namespace Cocktails.Api.Domain.Config;

public class PubSubConfig
{
    public const string SectionName = "PubSub";

    public required DaprServiceBusSubscriberConfig CocktailRatingSubscriber { get; set; }

    public required DaprServiceBusPublisherConfig CocktailUpdatesPublisher { get; set; }
}
