namespace Cocktails.Api.Application.IntegrationEvents;

using Cezzi.Applications;
using Cezzi.Applications.Extensions;
using Cocktails.Api.Apis.Integrations;
using Cocktails.Api.Application.Exceptions;
using Cocktails.Api.Domain.Aggregates.AccountAggregate;
using Cocktails.Api.Domain.Services;
using Cocktails.Api.Infrastructure.Services;
using Cocktails.Common.Emails;
using FluentValidation;
using MediatR;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using System;
using System.Text.Json.Serialization;

public class AccountOwnedProfileUpdatedEvent(Account ownedAccount) : IIntegrationEvent, IRequest<bool>
{
    [JsonInclude]
    public string CorrelationId { get; set; } = Guid.NewGuid().ToString();

    [JsonInclude]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [JsonInclude]
    public DateTimeOffset CreationDate { get; set; } = DateTimeOffset.UtcNow;

    [JsonInclude]
    public Account OwnedAccount { get; } = ownedAccount ?? throw new ArgumentNullException(nameof(ownedAccount));
}

public class AccountProfileUpdatedEventHandler(
    IMsGraphClient msGraphClient,
    ILogger<AccountProfileUpdatedEventHandler> logger) : IRequestHandler<AccountOwnedProfileUpdatedEvent, bool>
{
    public async Task<bool> Handle(AccountOwnedProfileUpdatedEvent command, CancellationToken cancellationToken)
    {
        Guard.NotNull(command, nameof(command));
        Guard.NotNull(command.OwnedAccount, nameof(command.OwnedAccount));

        try
        {
            await msGraphClient.PatchUser(
                subjectId: command.OwnedAccount.SubjectId,
                cancellationToken: cancellationToken,
                user: new User
                {
                    GivenName = command.OwnedAccount.GivenName,
                    Surname = command.OwnedAccount.FamilyName,
                    DisplayName = command.OwnedAccount.DisplayName.NullIfNullOrWhiteSpace() ?? $"{command.OwnedAccount.GivenName} {command.OwnedAccount.FamilyName}",
                    StreetAddress = command.OwnedAccount.PrimaryAddress?.AddressLine1.NullIfNullOrWhiteSpace(),
                    City = command.OwnedAccount.PrimaryAddress?.City.NullIfNullOrWhiteSpace(),
                    State = command.OwnedAccount.PrimaryAddress?.Region.NullIfNullOrWhiteSpace(),
                    PostalCode = command.OwnedAccount.PrimaryAddress?.PostalCode.NullIfNullOrWhiteSpace(),
                    Country = command.OwnedAccount.PrimaryAddress?.Country.NullIfNullOrWhiteSpace()
                });

            return true;
        }
        catch (Exception ex) when (ex is not CocktailsApiValidationException)
        {
            logger.LogError(ex, "Failed to sync owned account with identity provider");
            throw;
        }
    }
}

public class AccountProfileUpdatedEventValidator : AbstractValidator<AccountOwnedProfileUpdatedEvent>, IValidator<AccountOwnedProfileUpdatedEvent>
{
    public AccountProfileUpdatedEventValidator()
    {
        this.RuleLevelCascadeMode = CascadeMode.Stop;

        this.RuleFor(x => x.OwnedAccount).NotNull();
        this.RuleFor(x => x.OwnedAccount.GivenName).NotEmpty();
        this.RuleFor(x => x.OwnedAccount.FamilyName).NotEmpty();
    }
}