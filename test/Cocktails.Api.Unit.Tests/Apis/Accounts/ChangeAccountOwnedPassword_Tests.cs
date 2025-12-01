namespace Cocktails.Api.Unit.Tests.Apis.Accounts;

using Bogus;
using FluentAssertions;
using global::Cocktails.Api.Apis.Accounts;
using global::Cocktails.Api.Application.Concerns.Accounts.Models;
using global::Cocktails.Api.Application.Concerns.Integrations.Events;
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

public class ChangeAccountOwnedPassword_Tests : ServiceTestBase
{
    public ChangeAccountOwnedPassword_Tests() { }

    [Fact]
    public async Task changeaccountownedpassword__returns_matching_account()
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

        var request = new ChangeAccountOwnedPasswordRq
        (
            Email: account.Email
        );

        var updatedAccount = this.GetUpdatedAccount(account, request);

        var sp = this.SetupEnvironment((services) =>
        {
            services.Replace(new ServiceDescriptor(typeof(IAccountRepository), mockRepo.Object));
        });

        var services = GetAsParameterServices<AccountsServices>(sp);

        // act
        var response = await AccountsApi.ChangeAccountOwnedPassword(request, services);
        var result = response.Result as NoContent;

        // assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(StatusCodes.Status204NoContent);

        mockRepo.Verify(x => x.UnitOfWork.SaveEntitiesAsync(It.IsAny<CancellationToken>()), Times.Never);

        this.eventBusMock.Verify(x => x.PublishAsync(
            It.Is<ChangeAccountOwnedPasswordEvent>(x =>
                x.Email == request.Email &&
                x.OwnedAccountId == account.Id &&
                x.OwnedAccountSubjectId == account.SubjectId),
                "account-password-svc",
                "pubsub-sb-topics-cocktails-account-password",
                "fake-sbt-vec-eus-loc-cocktails-account-password-001",
                "application/json",
            It.IsAny<CancellationToken>()), Times.Once);
    }

    private Account GetUpdatedAccount(Account account, ChangeAccountOwnedPasswordRq request)
    {
        var js = System.Text.Json.JsonSerializer.Serialize(account);
        var newAccount = System.Text.Json.JsonSerializer.Deserialize<Account>(js);

        newAccount.SetEmail(request.Email);

        return newAccount;
    }
}