namespace Cocktails.Api.Application.Concerns.Accounts.Commands;

using Cezzi.Applications;
using global::Cocktails.Api.Application.Concerns.Accounts.Models;
using global::Cocktails.Api.Application.IntegrationEvents;
using global::Cocktails.Api.Domain;
using global::Cocktails.Api.Domain.Aggregates.AccountAggregate;
using global::Cocktails.Api.Domain.Config;
using global::Cocktails.Api.Domain.Services;
using global::Cocktails.Common;
using MediatR;
using Microsoft.Extensions.Options;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

public record UpdateAccountOwnedProfileCommand
(
    UpdateAccountOwnedProfileRq Request,

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

        var account = await accountRepository.GetOrCreateLocalAccountFromIdentity(
            claimsIdentity: command.Identity,
            cancellationToken: cancellationToken);

        Guard.NotNull(account);

        account.SetName(givenName: command.Request.GivenName, familyName: command.Request.FamilyName);
        account.SetDisplayName(displayName: command.Request.DisplayName);

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

        var accountUpdatedEvent = new AccountOwnedProfileUpdatedEvent(account);

        try
        {
            await eventBus.PublishAsync(
                @event: accountUpdatedEvent,
                messageLabel: "account-svc",
                contentType: "application/json",
                configName: pubSubConfig.Value.AccountPublisher.DaprBuildingBlock,
                topicName: pubSubConfig.Value.AccountPublisher.TopicName,
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

        return new AccountOwnedProfileRs(
            SubjectId: account.SubjectId,
            GivenName: account.GivenName,
            FamilyName: account.FamilyName,
            Email: account.Email,
            LoginEmail: account.LoginEmail ?? account.Email,
            AvatarUri: account.AvatarUri,
            DisplayName: account.DisplayName,
            PrimaryAddress: account.PrimaryAddress != null
                ? new AccountAddressModel(
                    AddressLine1: account.PrimaryAddress.AddressLine1 ?? string.Empty,
                    AddressLine2: account.PrimaryAddress.AddressLine2 ?? string.Empty,
                    City: account.PrimaryAddress.City ?? string.Empty,
                    Region: account.PrimaryAddress.Region ?? string.Empty,
                    SubRegion: account.PrimaryAddress.SubRegion ?? string.Empty,
                    PostalCode: account.PrimaryAddress.PostalCode ?? string.Empty,
                    Country: account.PrimaryAddress.Country ?? string.Empty)
                : null,
            Accessibility: account.Accessibility != null
                ? new AccountAccessibilitySettingsModel(Theme: (DisplayThemeModel)account.Accessibility.Theme)
                : null,
            FavoriteCocktails: account.FavoriteCocktails ?? []);
    }
}
