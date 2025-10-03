namespace Cocktails.Api.Application.Concerns.Accounts.Commands;

using Cezzi.Applications;
using global::Cocktails.Api.Application.Concerns.Accounts.Models;
using global::Cocktails.Api.Domain.Aggregates.AccountAggregate;
using global::Cocktails.Api.Infrastructure.Services;
using MediatR;
using System.Security.Claims;

public record ChangeAccountOwnedUsernameCommand
(
    ChangeAccountOwnedUsernameRq Request,

    ClaimsIdentity Identity

) : IRequest<bool>;

public class ChangeAccountOwnedUsernameCommandHandler(
    IAccountRepository accountRepository,
    IAuth0ManagementClient auth0ManagementClient,
    ILogger<ChangeAccountOwnedUsernameCommandHandler> logger)
    : IRequestHandler<ChangeAccountOwnedUsernameCommand, bool>
{
    public async Task<bool> Handle(ChangeAccountOwnedUsernameCommand command, CancellationToken cancellationToken)
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

        try
        {
            await auth0ManagementClient.ChangeUserUsername(
                subjectId: account.SubjectId,
                username: command.Request.Username,
                cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to change user username in Auth0.");
            throw;
        }

        return true;
    }
}
