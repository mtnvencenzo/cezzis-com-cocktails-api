namespace Cocktails.Api.Application.Concerns.Accounts.Commands;

using Cezzi.Applications;
using global::Cocktails.Api.Application.Concerns.Accounts.Models;
using global::Cocktails.Api.Domain.Aggregates.AccountAggregate;
using global::Cocktails.Api.Infrastructure.Services;
using MediatR;
using System.Security.Claims;

public record ChangeAccountOwnedPasswordCommand
(
    ChangeAccountOwnedPasswordRq Request,

    ClaimsIdentity Identity

) : IRequest<bool>;

public class ChangeAccountOwnedPasswordCommandHandler(IAuth0ManagementClient auth0ManagementClient, IAccountRepository accountRepository)
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

        await auth0ManagementClient.InitiateChangePasswordFlow(
            email: account.Email,
            cancellationToken: cancellationToken);

        return true;
    }
}
