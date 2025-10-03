namespace Cocktails.Api.Application.Concerns.Integrations.Events;

using FluentValidation;
using global::Cocktails.Api.Domain.Services;
using global::Cocktails.Api.Infrastructure.Services;
using MediatR;
using System;
using System.Text.Json.Serialization;

public class ChangeAccountOwnedEmailEvent(string ownedAccountId, string ownedAccountSubjectId, string email) : IIntegrationEvent, IRequest<bool>
{
    [JsonInclude]
    public string CorrelationId { get; set; } = Guid.NewGuid().ToString();

    [JsonInclude]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [JsonInclude]
    public DateTimeOffset CreationDate { get; set; } = DateTimeOffset.Now;

    [JsonInclude]
    public string OwnedAccountId { get; } = ownedAccountId ?? throw new ArgumentNullException(nameof(ownedAccountId));

    [JsonInclude]
    public string OwnedAccountSubjectId { get; } = ownedAccountSubjectId ?? throw new ArgumentNullException(nameof(ownedAccountSubjectId));

    [JsonInclude]
    public string Email { get; } = email ?? throw new ArgumentNullException(nameof(email));
}

public class ChangeAccountOwnedEmailEventHandler(IAuth0ManagementClient managementClient) : IRequestHandler<ChangeAccountOwnedEmailEvent, bool>
{
    public async Task<bool> Handle(ChangeAccountOwnedEmailEvent command, CancellationToken cancellationToken)
    {
        await managementClient.ChangeUserEmail(
            subjectId: command.OwnedAccountSubjectId,
            email: command.Email,
            cancellationToken: cancellationToken);

        return true;
    }
}

public class ChangeAccountOwnedEmailEventValidator : AbstractValidator<ChangeAccountOwnedEmailEvent>, IValidator<ChangeAccountOwnedEmailEvent>
{
    public ChangeAccountOwnedEmailEventValidator()
    {
        this.RuleLevelCascadeMode = CascadeMode.Stop;

        this.RuleFor(x => x.OwnedAccountId).NotEmpty().MinimumLength(Guid.NewGuid().ToString().Length);
        this.RuleFor(x => x.OwnedAccountSubjectId).NotEmpty();
        this.RuleFor(x => x.Email).NotEmpty().EmailAddress();
    }
}