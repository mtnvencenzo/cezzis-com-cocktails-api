namespace Cocktails.Api.Application.Concerns.Accounts.Commands;

using Cezzi.Applications;
using global::Cocktails.Api.Application.Concerns.Accounts.Models;
using global::Cocktails.Api.Domain.Aggregates.AccountAggregate;
using MediatR;
using System.Security.Claims;

public record UpdateAccountOwnedAccessibilitySettingsCommand
(
    UpdateAccountOwnedAccessibilitySettingsRq Request,

    ClaimsIdentity Identity

) : IRequest<AccountOwnedProfileRs>;

public class UpdateAccountOwnedAccessibilitySettingsCommandHandler(IAccountRepository accountRepository)
    : IRequestHandler<UpdateAccountOwnedAccessibilitySettingsCommand, AccountOwnedProfileRs>
{
    public async Task<AccountOwnedProfileRs> Handle(UpdateAccountOwnedAccessibilitySettingsCommand command, CancellationToken cancellationToken)
    {
        Guard.NotNull(command, nameof(command));
        Guard.NotNull(command.Request, nameof(command.Request));

        var account = await accountRepository.GetOrCreateLocalAccountFromIdentity(
            claimsIdentity: command.Identity,
            cancellationToken: cancellationToken);

        Guard.NotNull(account);

        account.SetAccessibilitySettings(theme: (AccessibilityTheme)command.Request.Theme);
        account.SetUpdatedOn(modifiedOn: DateTimeOffset.Now);

        accountRepository.Update(account);

        _ = await accountRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return AccountOwnedProfileRs.FromAccount(account);
    }
}
