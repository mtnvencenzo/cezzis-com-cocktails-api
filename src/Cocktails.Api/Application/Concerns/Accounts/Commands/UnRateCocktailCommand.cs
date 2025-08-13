namespace Cocktails.Api.Application.Concerns.Accounts.Commands;

using Cezzi.Applications;
using global::Cocktails.Api.Application.Concerns.Accounts.Models;
using global::Cocktails.Api.Application.IntegrationEvents;
using global::Cocktails.Api.Domain;
using global::Cocktails.Api.Domain.Aggregates.AccountAggregate;
using global::Cocktails.Api.Domain.Aggregates.CocktailAggregate;
using global::Cocktails.Api.Domain.Config;
using global::Cocktails.Api.Domain.Services;
using global::Cocktails.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;

#pragma warning disable format

public record UnRateCocktailCommand(string CocktailId, ClaimsIdentity Identity) : IRequest<bool>;

public class UnRateCocktailCommandHandler(
    IAccountRepository accountRepository,
    ICocktailRepository cocktailRepository,
    IAccountCocktailRatingsRepository accountCocktailRatingsRepository,
    IEventBus eventBus,
    IOptions<PubSubConfig> pubSubConfig,
    ILogger<UnRateCocktailCommandHandler> logger) : IRequestHandler<UnRateCocktailCommand, bool>
{
    public async Task<bool> Handle(UnRateCocktailCommand command, CancellationToken cancellationToken)
    {
        var account = await accountRepository.GetOrCreateLocalAccountFromIdentity(
            claimsIdentity: command.Identity,
            cancellationToken: cancellationToken);

        using var scope = logger.BeginScope(new Dictionary<string, object>
        {
            { Monikers.Account.AccountId, account?.Id },
            { Monikers.Account.SubjectId, account?.SubjectId },
            { Monikers.Cocktails.CocktailId, command?.CocktailId }
        });

        Guard.NotNull(account);

        var accountCocktailRatings = !string.IsNullOrWhiteSpace(account.SubjectId)
            ? await accountCocktailRatingsRepository.Items
                .WithPartitionKey(account.SubjectId)
                .Where(x => !string.IsNullOrWhiteSpace(account.RatingsId) || x.Id == account.RatingsId)
                .Where(x => x.SubjectId == account.SubjectId)
                .FirstOrDefaultAsync()
            : null;

        if (accountCocktailRatings == null && !accountCocktailRatings.Ratings.Any(x => x.CocktailId == command.CocktailId))
        {
            logger.LogWarning("Cocktail has not been rated");
            return false;
        }

        var cocktail = cocktailRepository.CachedItems.FirstOrDefault(x => x.Id == command.CocktailId);

        if (cocktail == null)
        {
            // cocktail not found
            logger.LogWarning("Cocktail not found to un rate");
            return false;
        }

        accountCocktailRatings.RemoveRating(command.CocktailId, out var stars);
        if (stars == null)
        {
            logger.LogWarning("Cocktail has not been rated");
            return false;
        }

        await accountCocktailRatingsRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        var cocktailRatingEvent = new CocktailRatingEvent(
            ownedAccountId: account.Id,
            ownedAccountSubjectId: account.SubjectId,
            cocktailId: command.CocktailId,
            stars: stars.Value)
            {
                DecrementRating = true
            };

        try
        {
            await eventBus.PublishAsync(
                @event: cocktailRatingEvent,
                messageLabel: "cocktail-ratings-svc",
                contentType: "application/json",
                configName: pubSubConfig.Value.CocktailRatingPublisher.DaprBuildingBlock,
                topicName: pubSubConfig.Value.CocktailRatingPublisher.TopicName,
                cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            var rawMessage = EventSerializer.ToJsonString(cocktailRatingEvent);

            using var messageScope = logger.BeginScope(new Dictionary<string, object>
            {
                { Monikers.App.ObjectGraph, rawMessage }
            });

            logger.LogCritical(ex, "Failed to send cocktail rating to topic");
        }

        return true;
    }
}