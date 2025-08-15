namespace Cocktails.Api.Application.Concerns.Accounts.Commands;

using Cezzi.Applications;
using global::Cocktails.Api.Application.Concerns.Accounts.Models;
using global::Cocktails.Api.Domain.Aggregates.AccountAggregate;
using MediatR;
using System.Security.Claims;

public record ManageFavoriteCocktailsCommand
(
    ManageFavoriteCocktailsRq Request,

    ClaimsIdentity Identity

) : IRequest<AccountOwnedProfileRs>;

public class ManageFavoriteCocktailsCommandHandler(IAccountRepository accountRepository)
    : IRequestHandler<ManageFavoriteCocktailsCommand, AccountOwnedProfileRs>
{
    public async Task<AccountOwnedProfileRs> Handle(ManageFavoriteCocktailsCommand command, CancellationToken cancellationToken)
    {
        Guard.NotNull(command, nameof(command));
        Guard.NotNull(command.Request, nameof(command.Request));

        var account = await accountRepository.GetOrCreateLocalAccountFromIdentity(
            claimsIdentity: command.Identity,
            cancellationToken: cancellationToken);

        Guard.NotNull(account);

        if (command.Request.CocktailActions != null && command.Request.CocktailActions.Count > 0)
        {
            account.ManageFavoriteCocktails(
                add: [.. command.Request.CocktailActions
                    .Where(x => x.Action == CocktailFavoritingActionModel.Add)
                    .Select(x => x.CocktailId)],
                remove: [.. command.Request.CocktailActions
                    .Where(x => x.Action == CocktailFavoritingActionModel.Remove)
                    .Select(x => x.CocktailId)]);

            account.SetUpdatedOn(modifiedOn: DateTimeOffset.Now);
            accountRepository.Update(account);

            _ = await accountRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }

        return AccountOwnedProfileRs.FromAccount(account);
    }
}
