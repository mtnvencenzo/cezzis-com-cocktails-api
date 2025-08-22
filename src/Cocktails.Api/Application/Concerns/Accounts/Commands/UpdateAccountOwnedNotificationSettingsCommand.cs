namespace Cocktails.Api.Application.Concerns.Accounts.Commands;

using Cezzi.Applications;
using global::Cocktails.Api.Application.Concerns.Accounts.Models;
using global::Cocktails.Api.Domain.Aggregates.AccountAggregate;
using MediatR;
using System.Security.Claims;

public record UpdateAccountOwnedNotificationSettingsCommand
(
    UpdateAccountOwnedNotificationSettingsRq Request,

    ClaimsIdentity Identity

) : IRequest<AccountOwnedProfileRs>;

public class UpdateAccountOwnedNotificationSettingsCommandHandler(IAccountRepository accountRepository)
    : IRequestHandler<UpdateAccountOwnedNotificationSettingsCommand, AccountOwnedProfileRs>
{
    public async Task<AccountOwnedProfileRs> Handle(UpdateAccountOwnedNotificationSettingsCommand command, CancellationToken cancellationToken)
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

        account.SetOnNewCocktailAdditionsNotification(
            onNewCocktailAdditions: Enum.Parse<CocktailUpdatedNotification>(command.Request.OnNewCocktailAdditions.ToString()));

        account.SetUpdatedOn(modifiedOn: DateTimeOffset.Now);

        accountRepository.Update(account);

        _ = await accountRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return AccountOwnedProfileRs.FromAccount(account);
    }
}
