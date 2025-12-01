namespace Cocktails.Api.Unit.Tests.Apis.Accounts;

using Bogus;
using FluentAssertions;
using global::Cocktails.Api.Apis.Accounts;
using global::Cocktails.Api.Application.Concerns.Accounts.Models;
using global::Cocktails.Api.Domain.Aggregates.AccountAggregate;
using global::Cocktails.Api.Infrastructure.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using Moq.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

public class UpdateAccountOwnedNotificationSettings_Tests : ServiceTestBase
{
    public UpdateAccountOwnedNotificationSettings_Tests() { }

    [Fact]
    public async Task updatenotificationsettings__returns_matching_account()
    {
        // Arrange
        var faker = new Faker();
        var mockRepo = new Mock<AccountRepository>(this.accountDbContextMock.Object) { CallBase = true };
        mockRepo.Setup(x => x.Update(It.IsAny<Account>()));

        var (account, claimsIdentity) = GetAccount(GuidString());
        var (unmatchedAccount, _) = GetAccount(GuidString());
        this.claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        var accounts = new List<Account> { unmatchedAccount, account };
        this.accountDbContextMock.Setup(x => x.Accounts).ReturnsDbSet(accounts);

        var request = new UpdateAccountOwnedNotificationSettingsRq
        (
            OnNewCocktailAdditions: faker.PickRandom<CocktailUpdatedNotificationModel>()
        );

        var updatedAccount = this.GetUpdatedAccount(account, request);

        var sp = this.SetupEnvironment((services) =>
        {
            services.Replace(new ServiceDescriptor(typeof(IAccountRepository), mockRepo.Object));
        });

        var services = GetAsParameterServices<AccountsServices>(sp);

        // act
        var response = await AccountsApi.UpdateAccountOwnedNotificationSettings(request, services);
        var result = response.Result as Ok<AccountOwnedProfileRs>;

        // assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(StatusCodes.Status200OK);
        result.Value.Should().BeEquivalentTo(AccountOwnedProfileRs.FromAccount(updatedAccount));
        result.Value.Notifications.OnNewCocktailAdditions.Should().Be(request.OnNewCocktailAdditions);

        mockRepo.Verify(x => x.Update(It.Is<Account>(a => a.Id == account.Id)), Times.Once);
        mockRepo.Verify(x => x.UnitOfWork.SaveEntitiesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    private Account GetUpdatedAccount(Account account, UpdateAccountOwnedNotificationSettingsRq request)
    {
        var js = System.Text.Json.JsonSerializer.Serialize(account);
        var newAccount = System.Text.Json.JsonSerializer.Deserialize<Account>(js);

        var onNewCocktailAdditions = Enum.Parse<CocktailUpdatedNotification>(request.OnNewCocktailAdditions.ToString());

        newAccount.SetOnNewCocktailAdditionsNotification(onNewCocktailAdditions);

        return newAccount;
    }
}