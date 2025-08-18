namespace Cocktails.Api.Unit.Tests.Apis.Accounts;

using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using global::Cocktails.Api.Apis.Accounts;
using global::Cocktails.Api.Application.Concerns.Accounts.Models;
using System.Security.Claims;
using global::Cocktails.Api.Domain.Aggregates.AccountAggregate;
using Moq.EntityFrameworkCore;
using Bogus;
using global::Cocktails.Api.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

public class GetCocktailRatings_Tests : ServiceTestBase
{
    public GetCocktailRatings_Tests() { }

    [Fact]
    public async Task getcocktailratings__returns_empty_list_for_valid_user_without_ratings()
    {
        // Arrange
        var subjectId = GuidString();

        var (account, claimsIdentity) = GetAccount(subjectId);
        var (unmatchedAccount, _) = GetAccount(GuidString());
        this.claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        var accounts = new List<Account> { account, unmatchedAccount };
        this.accountDbContextMock.Setup(x => x.Accounts).ReturnsDbSet(accounts);
        this.accountDbContextMock.Setup(x => x.CocktailRatings).ReturnsDbSet([]);

        var sp = this.SetupEnvironment();
        var services = GetAsParameterServices<AccountsServices>(sp);

        // act
        var response = await AccountsApi.GetCocktailRatings(services);
        var result = response.Result as Ok<AccountCocktailRatingsRs>;

        // assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(StatusCodes.Status200OK);
        result.Value.Should().BeEquivalentTo(new AccountCocktailRatingsRs(Ratings: []));
    }

    [Fact]
    public async Task getcocktailratings__returns_filled_list_for_valid_user_without_ratings()
    {
        // Arrange
        var subjectId = GuidString();

        var (account, claimsIdentity) = GetAccount(subjectId);
        var (unmatchedAccount, _) = GetAccount(GuidString());
        this.claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        var accounts = new List<Account> { account, unmatchedAccount };
        this.accountDbContextMock.Setup(x => x.Accounts).ReturnsDbSet(accounts);

        var ratings = new AccountCocktailRatingItem[]
        {
            new("adonis", 5),
            new("mai-tai", 4),
            new("white-russian", 3),
            new("mojito", 2),
            new("negroni", 1)
        };

        this.accountDbContextMock.Setup(x => x.CocktailRatings).ReturnsDbSet([
            new AccountCocktailRatings(account.RatingsId, subjectId).AddRatings(ratings),
            new AccountCocktailRatings(unmatchedAccount.RatingsId, unmatchedAccount.SubjectId).AddRatings([])
        ]);

        var sp = this.SetupEnvironment();
        var services = GetAsParameterServices<AccountsServices>(sp);

        // act
        var response = (await AccountsApi.GetCocktailRatings(services))?.Result as Ok<AccountCocktailRatingsRs>;

        // assert
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(StatusCodes.Status200OK);
        response.Value.Should().BeEquivalentTo(new AccountCocktailRatingsRs(Ratings: [.. ratings.Select(x => new AccountCocktailRatingsModel(CocktailId: x.CocktailId, Stars: x.Stars))]));
    }

    [Fact]
    public async Task getcocktailratings__returns_empty_list_for_invalid_user_without_ratings()
    {
        // Arrange
        var subjectId = GuidString();

        var (account, _) = GetAccount(subjectId);
        var (unmatchedAccount, claimsIdentity) = GetAccount(GuidString());
        this.claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        var accounts = new List<Account> { account };
        this.accountDbContextMock.Setup(x => x.Accounts).ReturnsDbSet(accounts);
        this.accountDbContextMock.Setup(x => x.CocktailRatings).ReturnsDbSet([]);

        var sp = this.SetupEnvironment((services) =>
        {
            var mockRepo = new Mock<AccountRepository>(this.accountDbContextMock.Object) { CallBase = true };
            mockRepo.Setup(x => x.Add(It.IsAny<Account>())).Returns((Account a) => unmatchedAccount);
            services.Replace(new ServiceDescriptor(typeof(IAccountRepository), mockRepo.Object));
        });

        var services = GetAsParameterServices<AccountsServices>(sp);

        // act
        var response = (await AccountsApi.GetCocktailRatings(services))?.Result as Ok<AccountCocktailRatingsRs>;

        // assert
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(StatusCodes.Status200OK);
        response.Value.Should().BeEquivalentTo(new AccountCocktailRatingsRs(Ratings: []));
    }
}
