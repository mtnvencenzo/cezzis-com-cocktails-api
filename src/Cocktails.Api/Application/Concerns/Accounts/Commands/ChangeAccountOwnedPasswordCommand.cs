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

public record ChangeAccountOwnedPasswordCommand
(
    ChangeAccountOwnedPasswordRq Request,

    ClaimsIdentity Identity

) : IRequest<bool>;

public class ChangeAccountOwnedPasswordCommandHandler(
    IAccountRepository accountRepository,
    IEventBus eventBus,
    IOptions<PubSubConfig> pubSubConfig,
    ILogger<ChangeAccountOwnedPasswordCommandHandler> logger)
    : IRequestHandler<ChangeAccountOwnedPasswordCommand, bool>
{
    public async Task<bool> Handle(ChangeAccountOwnedPasswordCommand command, CancellationToken cancellationToken)
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

        if (!string.Equals(account.Email, command.Request.Email, StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException("The email provided does not match the account email.");
        }

        // Send async event for the identity provider to be updated
        // with the password change.
        var updatePasswordEvent = new ChangeAccountOwnedPasswordEvent(
            ownedAccountId: account.Id,
            ownedAccountSubjectId: account.SubjectId,
            email: command.Request.Email);

        try
        {
            await eventBus.PublishAsync(
                @event: updatePasswordEvent,
                contentType: "application/json",
                messageLabel: pubSubConfig.Value.AccountPasswordPublisher.Subject,
                configName: pubSubConfig.Value.AccountPasswordPublisher.DaprBuildingBlock,
                topicName: pubSubConfig.Value.AccountPasswordPublisher.TopicName,
                cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            var rawMessage = EventSerializer.ToJsonString(updatePasswordEvent);

            using var messageScope = logger.BeginScope(new Dictionary<string, object>
            {
                { Monikers.App.ObjectGraph, rawMessage }
            });

            logger.LogCritical(ex, "Failed to send account password update event to topic");
        }

        return true;
    }
}

public class ChangeAccountOwnedPasswordCommandValidator : AbstractValidator<ChangeAccountOwnedPasswordCommand>, IValidator<ChangeAccountOwnedPasswordCommand>
{
    public ChangeAccountOwnedPasswordCommandValidator()
    {
        this.RuleLevelCascadeMode = CascadeMode.Stop;

        this.RuleFor(x => x.Request).NotNull();
        this.RuleFor(x => x.Request.Email).NotEmpty().EmailAddress();
    }
}
