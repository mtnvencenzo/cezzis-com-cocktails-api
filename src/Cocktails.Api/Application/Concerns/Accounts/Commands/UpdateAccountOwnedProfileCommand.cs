namespace Cocktails.Api.Application.Concerns.Accounts.Commands;

using Cezzi.Applications;
using global::Cocktails.Api.Application.Concerns.Accounts.Models;
using global::Cocktails.Api.Application.Concerns.Integrations.Events;
using global::Cocktails.Api.Domain;
using global::Cocktails.Api.Domain.Aggregates.AccountAggregate;
using global::Cocktails.Api.Domain.Config;
using global::Cocktails.Api.Domain.Services;
using global::Cocktails.Common;
using MediatR;
using Microsoft.Extensions.Options;
using System.Security.Claims;

public record UpdateAccountOwnedProfileCommand
(
    UpdateAccountOwnedProfileRq Request,

    bool UpdateIdentityProvider,

    ClaimsIdentity Identity

) : IRequest<AccountOwnedProfileRs>;

public class UpdateAccountOwnedProfileCommandHandler(
    IAccountRepository accountRepository,
    IEventBus eventBus,
    IOptions<PubSubConfig> pubSubConfig,
    ILogger<UpdateAccountOwnedProfileCommandHandler> logger) : IRequestHandler<UpdateAccountOwnedProfileCommand, AccountOwnedProfileRs>
{
    public async Task<AccountOwnedProfileRs> Handle(UpdateAccountOwnedProfileCommand command, CancellationToken cancellationToken)
    {
        Guard.NotNull(command, nameof(command));
        Guard.NotNull(command.Request, nameof(command.Request));

        var account = await accountRepository.GetLocalAccountFromIdentity(
            claimsIdentity: command.Identity,
            cancellationToken: cancellationToken);

        if (account == null)
        {
            throw new ArgumentNullException(nameof(account), "Failed to get account from identity.");
        }

        account.SetGivenName(givenName: command.Request.GivenName);
        account.SetFamilyName(familyName: command.Request.FamilyName);
        account.SetDisplayName(displayName: command.Request.DisplayName);
        account.SetUpdatedOn(modifiedOn: DateTimeOffset.Now);

        if (command.Request.PrimaryAddress != null)
        {
            account.SetPrimaryAddress(
                addressLine1: command.Request.PrimaryAddress?.AddressLine1,
                addressLine2: command.Request.PrimaryAddress?.AddressLine2,
                city: command.Request.PrimaryAddress?.City,
                region: command.Request.PrimaryAddress?.Region,
                subRegion: command.Request.PrimaryAddress?.SubRegion,
                postalCode: command.Request.PrimaryAddress?.PostalCode,
                country: command.Request.PrimaryAddress?.Country);
        }
        else
        {
            account.ClearPrimaryAddress();
        }

        accountRepository.Update(account);

        _ = await accountRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        // If not updating the identity provider then return early
        if (!command.UpdateIdentityProvider)
        {
            return AccountOwnedProfileRs.FromAccount(account);
        }

        var accountUpdatedEvent = new AccountOwnedProfileUpdatedEvent(account);

        try
        {
            await eventBus.PublishAsync(
                @event: accountUpdatedEvent,
                contentType: "application/json",
                messageLabel: pubSubConfig.Value.AccountPublisher.Subject,
                configName: pubSubConfig.Value.AccountPublisher.DaprBuildingBlock,
                topicName: pubSubConfig.Value.AccountPublisher.TopicName ?? pubSubConfig.Value.AccountPublisher.DaprBuildingBlock,
                correlationId: accountUpdatedEvent.CorrelationId,
                cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            var rawMessage = EventSerializer.ToJsonString(accountUpdatedEvent);

            using var messageScope = logger.BeginScope(new Dictionary<string, object>
            {
                { Monikers.App.ObjectGraph, rawMessage }
            });

            logger.LogCritical(ex, "Failed to send email message to topic");
        }

        return AccountOwnedProfileRs.FromAccount(account);
    }
}
