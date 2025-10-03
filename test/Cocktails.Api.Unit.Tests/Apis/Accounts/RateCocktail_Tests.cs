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
using Bogus;
using Moq;
using global::Cocktails.Api.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using global::Cocktails.Api.Application.Concerns.Integrations.Events;

public class RateCocktail_Tests : ServiceTestBase
{
    public RateCocktail_Tests() { }

    [Fact]
    public async Task ratecocktail__creates_new_ratings_entry_for_account()
    {
        // Arrange
        var faker = new Faker();

        var (account, claimsIdentity) = GetAccount(GuidString());
        this.claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        var accounts = new List<Account> { account };
        this.accountDbContextMock.Setup(x => x.Accounts).ReturnsDbSet(accounts);
        this.accountDbContextMock.Setup(x => x.CocktailRatings).ReturnsDbSet([]);

        var newRatings = new AccountCocktailRatings(account.RatingsId, account.SubjectId);
        var acctRatingsRepo = new Mock<AccountCocktailRatingsRepository>(this.accountDbContextMock.Object) { CallBase = true };
        acctRatingsRepo.Setup(x => x.Add(It.IsAny<AccountCocktailRatings>())).Returns((AccountCocktailRatings a) => newRatings);

        var request = new RateCocktailRq
        (
            CocktailId: "paloma",
            Stars: faker.Random.Int(1, 5)
        );

        var sp = this.SetupEnvironment((services) =>
        {
            services.Replace(new ServiceDescriptor(typeof(IAccountCocktailRatingsRepository), acctRatingsRepo.Object));
        });

        var services = GetAsParameterServices<AccountsServices>(sp);

        // act
        var response = await AccountsApi.RateCocktail(request, services);
        var result = response.Result as Created<RateCocktailRs>;

        // assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(StatusCodes.Status201Created);

        result.Value.Should().BeEquivalentTo(new RateCocktailRs
        (
            CocktailId: request.CocktailId,
            CocktailRating: new AccountCocktailRatingModel(
                OneStars: request.Stars == 1 ? 1 : 0,
                TwoStars: request.Stars == 2 ? 1 : 0,
                ThreeStars: request.Stars == 3 ? 1 : 0,
                FourStars: request.Stars == 4 ? 1 : 0,
                FiveStars: request.Stars == 5 ? 1 : 0,
                TotalStars: request.Stars,
                Rating: request.Stars,
                RatingCount: 1
            ),
            Ratings:
            [
                new (request.CocktailId, request.Stars)
            ]
        ));

        acctRatingsRepo.Verify(x => x.Add(It.Is<AccountCocktailRatings>(a => a.Id == account.RatingsId && a.SubjectId == account.SubjectId)), Times.Once);
        acctRatingsRepo.Verify(x => x.UnitOfWork.SaveEntitiesAsync(It.IsAny<CancellationToken>()), Times.Once);

        this.eventBusMock.Verify(x => x.PublishAsync(
            It.Is<CocktailRatingEvent>(x =>
                x.CocktailId == request.CocktailId &&
                x.Stars == request.Stars),
            "cocktail-ratings-svc",
            "pubsub-sb-topics-cocktails-rating",
            "fake-sbt-vec-eus-loc-cocktails-rating-001",
            "application/json",
            It.IsAny<CancellationToken>()), Times.Once);
    }
}