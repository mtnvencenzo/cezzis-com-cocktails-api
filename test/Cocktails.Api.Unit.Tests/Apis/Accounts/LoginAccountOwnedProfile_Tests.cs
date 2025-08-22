namespace Cocktails.Api.Unit.Tests.Apis.Accounts;

using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Threading.Tasks;
using Xunit;
using global::Cocktails.Api.Apis.Accounts;
using global::Cocktails.Api.Application.Concerns.Accounts.Models;
using System.Security.Claims;
using global::Cocktails.Api.Domain.Aggregates.AccountAggregate;
using Moq.EntityFrameworkCore;
using Moq;
using global::Cocktails.Api.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

public class LoginAccountOwnedProfile_Tests : ServiceTestBase
{
    public LoginAccountOwnedProfile_Tests() { }

    [Fact]
    public async Task loginaccountownedprofile__returns_matching_account()
    {
        // Arrange
        var (account, claimsIdentity) = GetAccount(GuidString());
        var (unmatchedAccount, _) = GetAccount(GuidString());
        this.claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        var accounts = new List<Account> { unmatchedAccount, account };
        this.accountDbContextMock.Setup(x => x.Accounts).ReturnsDbSet(accounts);

        var sp = this.SetupEnvironment();
        var services = GetAsParameterServices<AccountsServices>(sp);

        // act
        var response = await AccountsApi.LoginAccountOwnedProfile(services);
        var result = response.Result as Ok<AccountOwnedProfileRs>;

        // assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(StatusCodes.Status200OK);
        result.Value.Should().BeEquivalentTo(AccountOwnedProfileRs.FromAccount(account));
    }

    [Fact]
    public async Task loginaccountownedprofile__returns_new_account_when_not_matched()
    {
        // Arrange
        var subjectId = GuidString();

        var (account, _) = GetAccount(subjectId);
        var (unmatchedAccount, claimsIdentity) = GetAccount(GuidString(), onlyWithClaims: true);
        this.claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        var accounts = new List<Account> { account };
        this.accountDbContextMock.Setup(x => x.Accounts).ReturnsDbSet(accounts);

        var sp = this.SetupEnvironment((services) =>
        {
            var mockRepo = new Mock<AccountRepository>(this.accountDbContextMock.Object) { CallBase = true };
            mockRepo.Setup(x => x.Add(It.IsAny<Account>())).Returns((Account a) => unmatchedAccount);
            services.Replace(new ServiceDescriptor(typeof(IAccountRepository), mockRepo.Object));
        });

        var services = GetAsParameterServices<AccountsServices>(sp);

        // act
        var response = await AccountsApi.LoginAccountOwnedProfile(services);
        var result = response.Result as Created<AccountOwnedProfileRs>;

        // assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(StatusCodes.Status201Created);
        result.Value.Should().BeEquivalentTo(AccountOwnedProfileRs.FromAccount(unmatchedAccount));
    }
}