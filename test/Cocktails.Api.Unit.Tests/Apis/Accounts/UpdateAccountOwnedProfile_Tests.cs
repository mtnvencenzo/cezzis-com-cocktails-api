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

public class UpdateAccountOwnedProfile_Tests : ServiceTestBase
{
    public UpdateAccountOwnedProfile_Tests() { }

    [Fact]
    public async Task updateownedaccountprofile__returns_matching_account()
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

        var request = new UpdateAccountOwnedProfileRq
        (
            GivenName: faker.Name.FirstName(),
            FamilyName: faker.Name.LastName(),
            DisplayName: faker.Name.FullName(),
            PrimaryAddress: new AccountAddressModel
            (
                AddressLine1: faker.Address.StreetAddress(),
                AddressLine2: faker.Address.SecondaryAddress(),
                City: faker.Address.City(),
                Region: faker.Address.StateAbbr(),
                SubRegion: faker.Address.County(),
                PostalCode: faker.Address.ZipCode(),
                Country: faker.Address.CountryCode(Bogus.DataSets.Iso3166Format.Alpha3)
            )
        );

        var updatedAccount = this.GetUpdatedAccount(account, request);

        var sp = this.SetupEnvironment((services) =>
        {
            services.Replace(new ServiceDescriptor(typeof(IAccountRepository), mockRepo.Object));
        });

        var services = GetAsParameterServices<AccountsServices>(sp);

        // act
        var response = await AccountsApi.UpdateAccountOwnedProfile(request, services);
        var result = response.Result as Ok<AccountOwnedProfileRs>;

        // assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(StatusCodes.Status200OK);
        result.Value.Should().BeEquivalentTo(AccountOwnedProfileRs.FromAccount(updatedAccount));

        mockRepo.Verify(x => x.Update(It.Is<Account>(a => a.Id == account.Id)), Times.Once);
        mockRepo.Verify(x => x.UnitOfWork.SaveEntitiesAsync(It.IsAny<CancellationToken>()), Times.Once);

        this.eventBusMock.Verify(x => x.PublishAsync(
            It.Is<AccountOwnedProfileUpdatedEvent>(x => x.OwnedAccount.Id == updatedAccount.Id),
            "account-svc",
            "pubsub-sb-topics-cocktails-account",
            "fake-sbt-vec-eus-loc-cocktails-account-001",
            "application/json",
            It.Is<string>(s => !string.IsNullOrWhiteSpace(s)),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    private Account GetUpdatedAccount(Account account, UpdateAccountOwnedProfileRq request)
    {
        var js = System.Text.Json.JsonSerializer.Serialize(account);
        var newAccount = System.Text.Json.JsonSerializer.Deserialize<Account>(js);

        newAccount.SetGivenName(givenName: request.GivenName);
        newAccount.SetFamilyName(familyName: request.FamilyName);
        newAccount.SetDisplayName(displayName: request.DisplayName);
        newAccount.SetUpdatedOn(modifiedOn: DateTimeOffset.Now);
        newAccount.SetPrimaryAddress(
            addressLine1: request.PrimaryAddress?.AddressLine1,
            addressLine2: request.PrimaryAddress?.AddressLine2,
            city: request.PrimaryAddress?.City,
            region: request.PrimaryAddress?.Region,
            subRegion: request.PrimaryAddress?.SubRegion,
            postalCode: request.PrimaryAddress?.PostalCode,
            country: request.PrimaryAddress?.Country);

        return newAccount;
    }
}