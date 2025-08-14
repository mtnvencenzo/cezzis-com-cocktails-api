namespace Cocktails.Api.Domain.Unit.Tests.Aggregates.AccountAggregate;

using Cocktails.Api.Domain.Aggregates.AccountAggregate;
using Cocktails.Api.Domain.Exceptions;
using FluentAssertions;
using System.Security.Claims;
using Xunit;

public class AccountTests
{
    [Fact]
    public void CreateAccount_ShouldInitializeWithDefaultValues()
    {
        // arrange
        var accountId = Guid.NewGuid().ToString();
        var claimsAccount = new ClaimsAccount(new ClaimsIdentity(
        [
            new(ClaimTypes.NameIdentifier, accountId),
            new(ClaimTypes.GivenName, "John"),
            new(ClaimTypes.Surname, "Doe"),
            new("emails", "test@test.com")
        ]));

        // act
        var account = new Account(claimsAccount);

        // assert
        account.SubjectId.Should().Be(accountId);
        account.GivenName.Should().Be("John");
        account.FamilyName.Should().Be("Doe");
        account.Notifications.Should().BeNull();
    }

    [Fact]
    public void SetUpdateCocktailNotification_ShouldUpdateNotification()
    {
        // arrange
        var accountId = Guid.NewGuid().ToString();
        var claimsAccount = new ClaimsAccount(new ClaimsIdentity(
        [
            new(ClaimTypes.NameIdentifier, accountId),
            new(ClaimTypes.GivenName, "John"),
            new(ClaimTypes.Surname, "Doe"),
            new("emails", "test@test.com")
        ]));
        var account = new Account(claimsAccount);

        // act
        account.SetUpdateCocktailNotification(CocktailUpdateNotification.None);

        // assert
        account.Notifications.NewCocktails.Should().Be(CocktailUpdateNotification.None);
    }

    [Fact]
    public void CreateAccount_ShouldThrowException_WhenClaimsAccountIsNull()
    {
        // arrange
        ClaimsAccount claimsAccount = null;

        // act
        var act = () => new Account(claimsAccount);

        // assert
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'claimsAccount')");
    }

    [Fact]
    public void SetName_ShouldUpdateGivenNameAndFamilyName()
    {
        // arrange
        var accountId = Guid.NewGuid().ToString();
        var claimsAccount = new ClaimsAccount(new ClaimsIdentity(
        [
            new(ClaimTypes.NameIdentifier, accountId),
            new(ClaimTypes.GivenName, "John"),
            new(ClaimTypes.Surname, "Doe"),
            new("emails", "test@test.com")
        ]));
        var account = new Account(claimsAccount);

        // act
        account.SetName("Jane", "Smith");

        // assert
        account.GivenName.Should().Be("Jane");
        account.FamilyName.Should().Be("Smith");
    }

    [Fact]
    public void SetEmail_ShouldUpdateEmail()
    {
        // arrange
        var accountId = Guid.NewGuid().ToString();
        var claimsAccount = new ClaimsAccount(new ClaimsIdentity(
        [
            new(ClaimTypes.NameIdentifier, accountId),
            new(ClaimTypes.GivenName, "John"),
            new(ClaimTypes.Surname, "Doe"),
            new("emails", "test@test.com")
        ]));
        var account = new Account(claimsAccount);

        // act
        account.SetEmail("test1@test.com");

        // assert
        account.Email.Should().Be("test1@test.com");
    }

    [Fact]
    public void SetLoginEmail_ShouldThrowExceptionIfAlreadySpecified()
    {
        // arrange
        var accountId = Guid.NewGuid().ToString();
        var claimsAccount = new ClaimsAccount(new ClaimsIdentity(
        [
            new(ClaimTypes.NameIdentifier, accountId),
            new(ClaimTypes.GivenName, "John"),
            new(ClaimTypes.Surname, "Doe"),
            new("emails", "test@test.com")
        ]));
        var account = new Account(claimsAccount);

        // act
        var act = () => account.SetLoginEmail("test1@test.com");

        // assert
        act.Should().Throw<CocktailsApiDomainException>()
            .WithMessage("Account login email has already been specified");
    }
}