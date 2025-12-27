namespace Cocktails.Api.Application.Concerns.Accounts.Commands;

using Cezzi.Applications;
using FluentValidation;
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

public record ChangeAccountOwnedEmailCommand
(
    ChangeAccountOwnedEmailRq Request,

    bool UpdateIdentityProvider,

    ClaimsIdentity Identity

) : IRequest<AccountOwnedProfileRs>;

public class ChangeAccountOwnedEmailCommandHandler(
    IAccountRepository accountRepository,
    IEventBus eventBus,
    IOptions<PubSubConfig> pubSubConfig,
    ILogger<ChangeAccountOwnedEmailCommandHandler> logger)
    : IRequestHandler<ChangeAccountOwnedEmailCommand, AccountOwnedProfileRs>
{
    public async Task<AccountOwnedProfileRs> Handle(ChangeAccountOwnedEmailCommand command, CancellationToken cancellationToken)
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

        account.SetEmail(email: command.Request.Email);
        account.SetUpdatedOn(modifiedOn: DateTimeOffset.Now);

        accountRepository.Update(account);

        _ = await accountRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        // If not updating the identity provider then return early
        if (!command.UpdateIdentityProvider)
        {
            return AccountOwnedProfileRs.FromAccount(account);
        }

        // Send async event for the identity provider to be updated
        // with the email change.
        var updateEmailEvent = new ChangeAccountOwnedEmailEvent(
            ownedAccountId: account.Id,
            ownedAccountSubjectId: account.SubjectId,
            email: command.Request.Email);
        try
        {
            await eventBus.PublishAsync(
                @event: updateEmailEvent,
                contentType: "application/json",
                messageLabel: pubSubConfig.Value.AccountEmailPublisher.Subject,
                configName: pubSubConfig.Value.AccountEmailPublisher.DaprBuildingBlock,
                topicName: pubSubConfig.Value.AccountEmailPublisher.TopicName ?? pubSubConfig.Value.AccountEmailPublisher.DaprBuildingBlock,
                correlationId: updateEmailEvent.CorrelationId,
                cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            var rawMessage = EventSerializer.ToJsonString(updateEmailEvent);

            using var messageScope = logger.BeginScope(new Dictionary<string, object>
            {
                { Monikers.App.ObjectGraph, rawMessage }
            });

            logger.LogCritical(ex, "Failed to send account email update event to topic");
        }

        return AccountOwnedProfileRs.FromAccount(account);
    }
}

public class ChangeAccountOwnedEmailCommandValidator : AbstractValidator<ChangeAccountOwnedEmailCommand>, IValidator<ChangeAccountOwnedEmailCommand>
{
    public ChangeAccountOwnedEmailCommandValidator()
    {
        this.RuleLevelCascadeMode = CascadeMode.Stop;

        this.RuleFor(x => x.Request).NotNull();
        this.RuleFor(x => x.Request.Email).NotEmpty().EmailAddress();
    }
}