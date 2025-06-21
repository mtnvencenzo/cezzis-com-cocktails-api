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

public record RateCocktailCommand(string CocktailId, int Stars, ClaimsIdentity Identity) : IRequest<RateCocktailRs>;

public class RateCocktailCommandHandler(
    IAccountRepository accountRepository,
    ICocktailRepository cocktailRepository,
    IAccountCocktailRatingsRepository accountCocktailRatingsRepository,
    IEventBus eventBus,
    IOptions<PubSubConfig> pubSubConfig,
    ILogger<RateCocktailCommandHandler> logger) : IRequestHandler<RateCocktailCommand, RateCocktailRs>
{
    public async Task<RateCocktailRs> Handle(RateCocktailCommand command, CancellationToken cancellationToken)
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

        if (accountCocktailRatings != null && accountCocktailRatings.Ratings.Any(x => x.CocktailId == command.CocktailId))
        {
            logger.LogWarning("Cocktail already rated");
            return null;
        }

        var cocktail = cocktailRepository.CachedItems.FirstOrDefault(x => x.Id == command.CocktailId);

        if (cocktail == null)
        {
            // cocktail not found
            logger.LogWarning("Cocktail not found to rate");
            return null;
        }

        if (accountCocktailRatings == null)
        {
            // Update the account with a ratings id as a sortof foreign key
            // to the soon to be added ratings document
            if (string.IsNullOrWhiteSpace(account.RatingsId))
            {
                account.SetRatingsId(Guid.NewGuid().ToString());
                accountRepository.Update(account);
            }

            accountCocktailRatings = new AccountCocktailRatings(id: account.RatingsId, subjectId: account.SubjectId);
            accountCocktailRatingsRepository.Add(accountCocktailRatings);
        }

        accountCocktailRatings.AddRating(cocktailId: command.CocktailId, stars: command.Stars);

        await accountCocktailRatingsRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        var cocktailRatingEvent = new CocktailRatingEvent(
            ownedAccountId: account.Id,
            ownedAccountSubjectId: account.SubjectId,
            cocktailId: command.CocktailId,
            stars: command.Stars);

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

        // The actual real persistence is handle through
        // eventing but we're incrementing here for the response
        // which should be 'mostly' accurate to what it will be
        // after the event is processed
        var eventualCocktailRating = new CocktailRating
        (
            cocktail.Rating?.OneStars ?? 0,
            cocktail.Rating?.TwoStars ?? 0,
            cocktail.Rating?.ThreeStars ?? 0,
            cocktail.Rating?.FourStars ?? 0,
            cocktail.Rating?.FiveStars ?? 0,
            cocktail.Rating?.RatingCount ?? 0
        ).Increment(command.Stars);

        return new RateCocktailRs
        (
            [.. accountCocktailRatings.Ratings.Select(x => new AccountCocktailRatingsModel(x.CocktailId, command.Stars))],
            command.CocktailId,
            new AccountCocktailRatingModel
            (
                eventualCocktailRating.OneStars,
                eventualCocktailRating.TwoStars,
                eventualCocktailRating.ThreeStars,
                eventualCocktailRating.FourStars,
                eventualCocktailRating.FiveStars,
                eventualCocktailRating.TotalStars,
                eventualCocktailRating.Rating,
                eventualCocktailRating.RatingCount
            )
        );
    }
}