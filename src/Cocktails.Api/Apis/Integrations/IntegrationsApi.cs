namespace Cocktails.Api.Apis.Integrations;

using Cocktails.Api.Application.Exceptions;
using Cocktails.Api.Application.IntegrationEvents;
using Cocktails.Api.Domain;
using Cocktails.Api.Domain.Config;
using Cocktails.Common;
using Dapr;
using FluentAssertions.Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Graph;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using System.Threading;
using static System.Runtime.InteropServices.JavaScript.JSType;

public static class IntegrationsApi
{
    public static RouteGroupBuilder MapIntegrationsApiApiV1(this IEndpointRouteBuilder app)
    {
        var emailSubscriberOptions = app.ServiceProvider.GetRequiredService<IOptions<PubSubConfig>>().Value.EmailSubscriber;
        var accountSubscriberOptions = app.ServiceProvider.GetRequiredService<IOptions<PubSubConfig>>().Value.AccountSubscriber;
        var cocktailRatingSubscriberOptions = app.ServiceProvider.GetRequiredService<IOptions<PubSubConfig>>().Value.CocktailRatingSubscriber;

        var groupBuilder = app.MapGroup("/integrations")
            .WithTags("Integrations")
            .ExcludeFromDescription()
            .AllowAnonymous();

        groupBuilder.MapPost("/zoho/email", SendZohoEmail)
            .WithName(nameof(SendZohoEmail))
            .WithDisplayName(nameof(SendZohoEmail))
            .WithDescription("Sends a zoho email message to zoho's smtp servers")
            .WithTopic(emailSubscriberOptions.DaprBuildingBlock, emailSubscriberOptions.QueueName);

        // dapr can only POST and not PUT
        groupBuilder.MapPost("/accounts/owned/profile", UpdateIdentityProfile)
            .WithName(nameof(UpdateIdentityProfile))
            .WithDisplayName(nameof(UpdateIdentityProfile))
            .WithDescription("Syncs an account profile with the identity provider")
            .WithTopic(accountSubscriberOptions.DaprBuildingBlock, accountSubscriberOptions.QueueName);

        groupBuilder.MapPost("/cocktails/ratings", UpdateCocktailRating)
            .WithName(nameof(UpdateCocktailRating))
            .WithDisplayName(nameof(UpdateCocktailRating))
            .WithDescription("Updates the rating on a cocktail for a single user account rating")
            .WithTopic(cocktailRatingSubscriberOptions.DaprBuildingBlock, cocktailRatingSubscriberOptions.QueueName);

        return groupBuilder;
    }

    /// <summary>Sends an email message to zoho email servers</summary>
    /// <param name="cloudEvent"></param>
    /// <param name="integrationServices"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async static Task<Results<Ok, JsonHttpResult<ProblemDetails>>> SendZohoEmail(
        [FromBody] CloudEvent<CocktailRecommendationEmailEvent> cloudEvent,
        [AsParameters] IntegrationsServices integrationServices,
        CancellationToken cancellationToken)
    {
        using var logScope = integrationServices.Logger.BeginScope(new Dictionary<string, object>
        {
            { Monikers.ServiceBus.MsgId, cloudEvent?.Data?.Id },
            { Monikers.ServiceBus.MsgCorrelationId, cloudEvent?.Data?.CorrelationId },
            { Monikers.ServiceBus.MsgSubject, cloudEvent?.Subject }
        });

        _ = await integrationServices.Mediator.Send(
            request: cloudEvent.Data,
            cancellationToken: cancellationToken);

        return TypedResults.Ok();
    }

    /// <summary>Syncs an account profile with the identity provider</summary>
    /// <param name="cloudEvent"></param>
    /// <param name="integrationServices"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async static Task<Results<Ok, JsonHttpResult<ProblemDetails>>> UpdateIdentityProfile(
        [FromBody] CloudEvent<AccountOwnedProfileUpdatedEvent> cloudEvent,
        [AsParameters] IntegrationsServices integrationServices,
        CancellationToken cancellationToken)
    {
        using var logScope = integrationServices.Logger.BeginScope(new Dictionary<string, object>
        {
            { Monikers.ServiceBus.MsgId, cloudEvent?.Data?.Id },
            { Monikers.ServiceBus.MsgCorrelationId, cloudEvent?.Data?.CorrelationId },
            { Monikers.ServiceBus.MsgSubject, cloudEvent?.Subject }
        });

        _ = await integrationServices.Mediator.Send(
            request: cloudEvent.Data,
            cancellationToken: cancellationToken);

        return TypedResults.Ok();
    }

    /// <summary>Updates the rating on a cocktail for a single user account rating</summary>
    /// <param name="cloudEvent"></param>
    /// <param name="integrationServices"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async static Task<Results<Ok, JsonHttpResult<ProblemDetails>>> UpdateCocktailRating(
        [FromBody] CloudEvent<CocktailRatingEvent> cloudEvent,
        [AsParameters] IntegrationsServices integrationServices,
        CancellationToken cancellationToken)
    {
        using var logScope = integrationServices.Logger.BeginScope(new Dictionary<string, object>
        {
            { Monikers.ServiceBus.MsgId, cloudEvent?.Data?.Id },
            { Monikers.ServiceBus.MsgCorrelationId, cloudEvent?.Data?.CorrelationId },
            { Monikers.ServiceBus.MsgSubject, cloudEvent?.Subject }
        });

        _ = await integrationServices.Mediator.Send(
            request: cloudEvent.Data,
            cancellationToken: cancellationToken);

        return TypedResults.Ok();
    }
}
