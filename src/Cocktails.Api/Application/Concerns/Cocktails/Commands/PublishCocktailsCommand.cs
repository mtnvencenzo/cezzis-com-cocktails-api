namespace Cocktails.Api.Application.Concerns.Cocktails.Commands;

using global::Cocktails.Api.Application.Concerns.Cocktails.Services;
using global::Cocktails.Api.Domain;
using global::Cocktails.Api.Domain.Aggregates.CocktailAggregate;
using global::Cocktails.Api.Domain.Config;
using global::Cocktails.Api.Domain.Services;
using global::Cocktails.Common;
using MediatR;
using Microsoft.Extensions.Options;

public record PublishCocktailsCommand(int BatchItemCount = 1, string[] CocktailIds = null) : IRequest<bool>;

public class PublishCocktailsCommandHandler(
    ICocktailRepository cocktailRepository,
    IEventBus eventBus,
    IOptions<PubSubConfig> pubSubConfig,
    ICocktailModelConverter cocktailModelConverter,
    ILogger<PublishCocktailsCommandHandler> logger) : IRequestHandler<PublishCocktailsCommand, bool>
{
    public async Task<bool> Handle(PublishCocktailsCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("Publishing cocktails with batch size of {BatchItemCount}", command.BatchItemCount);

        var offset = 0;
        List<Cocktail> cocktails;

        var cocktailIds = command.CocktailIds != null && command.CocktailIds.Length > 0
            ? command.CocktailIds
            : [];

        do
        {
            cocktails = [.. cocktailRepository.CachedItems
                .Where(c => cocktailIds.Length == 0 || cocktailIds.Contains(c.Id))
                .Skip(offset)
                .Take(command.BatchItemCount)];

            if (cocktails.Count == 0)
            {
                break;
            }

            var cocktailModels = cocktails
                .Select(c => cocktailModelConverter.ToCocktailModel(c))
                .ToList();

            try
            {
                await eventBus.PublishAsync(
                    @event: cocktailModels,
                    contentType: "application/json",
                    messageLabel: pubSubConfig.Value.CocktailUpdatesPublisher.Subject,
                    configName: pubSubConfig.Value.CocktailUpdatesPublisher.DaprBuildingBlock,
                    topicName: pubSubConfig.Value.CocktailUpdatesPublisher.TopicName ?? pubSubConfig.Value.CocktailUpdatesPublisher.DaprBuildingBlock,
                    cancellationToken: cancellationToken);

                logger.LogInformation("Published batch of {Count} cocktails", cocktails.Count);
            }
            catch (Exception ex)
            {
                var rawMessage = EventSerializer.ToJsonString(cocktailModels);

                using var messageScope = logger.BeginScope(new Dictionary<string, object>
                {
                    { Monikers.App.ObjectGraph, rawMessage }
                });

                logger.LogCritical(ex, "Failed to publish batch at offset {Offset}", offset);
                throw;
            }

            offset += command.BatchItemCount;

        } while (cocktails.Count > 0);

        logger.LogInformation("Finished publishing cocktails.");

        return true;
    }
}