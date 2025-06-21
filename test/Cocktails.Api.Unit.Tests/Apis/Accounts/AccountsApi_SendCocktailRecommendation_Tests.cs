namespace Cocktails.Api.Unit.Tests.Apis.Accounts;

using Cezzi.Security.Recaptcha;
using FluentAssertions;
using FluentValidation.Results;
using global::Cocktails.Api.Application.Exceptions;
using global::Cocktails.Api.Domain.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Xunit;
using global::Cocktails.Api.Application.IntegrationEvents;
using global::Cocktails.Api.Apis.Accounts;
using global::Cocktails.Api.Application.Concerns.Accounts.Models;
using global::Cocktails.Api.Application.Concerns.Accounts.Commands;

public class AccountsApi_SendCocktailRecommendation_Tests : ServiceTestBase
{
    private readonly Mock<IEventBus> eventBusMock;

    public AccountsApi_SendCocktailRecommendation_Tests()
    {
        this.eventBusMock = new Mock<IEventBus>();
    }

    [Fact]
    public async Task sendrecommendation__invokes_service_bus_and_returns_correct_data()
    {
        var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{\r\n  \"success\": true,\r\n  \"challenge_ts\": \"2024-05-26T12:32:12\",\r\n  \"hostname\": \"cezzis.com\",\r\n  \"error-codes\": []\r\n}")
        };

        var httpClientMock = new Mock<HttpClient>();

        httpClientMock
            .Setup(x => x.SendAsync(
                It.IsAny<HttpRequestMessage>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(httpResponse);

        this.httpClientFactoryMock
            .Setup(x => x.CreateClient(It.Is<string>(x => x == RecaptchaSiteVerifyService.HttpClientName)))
            .Returns(httpClientMock.Object);

        // Arrange
        var sp = this.SetupEnvironment((services) => { services.Replace(new ServiceDescriptor(typeof(IEventBus), this.eventBusMock.Object)); });
        var services = GetAsParameterServices<AccountsServices>(sp);
        var request = new CocktailRecommendationRq
        (
            Recommendation: new CocktailRecommendationModel
            (
                Name: "Ron's Cocktail",
                Ingredients: "Test ingredients",
                Directions: "Test directions"
            ),
            VerificationCode: GuidString()
        );

        // act
        var response = (await AccountsApi.SendCocktailRecommendation(request, services))?.Result as Accepted;

        // assert
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(StatusCodes.Status202Accepted);
        response.Location.Should().Be(string.Empty);

        this.eventBusMock
            .Verify(x => x.PublishAsync(
                It.Is<CocktailRecommendationEmailEvent>(x =>
                    x.Subject == "New Cocktail Recommendation for Cezzis.Com" &&
                    x.Attachments.Count == 0 &&
                    x.From.Address == "cezzi@cezzis.com" &&
                    x.From.DisplayName == "DoNotReply" &&
                    x.To.Count == 1 &&
                    x.To.First().Address == "cezzi@cezzis.com" &&
                    x.To.First().DisplayName == "Caesar" &&
                    x.Body.Contains(request.Recommendation.GetRecommendation())),
                It.Is<string>(x => x == "email-svc"),
                It.Is<string>(x => x == "pubsub-sb-topics-cocktails-email"),
                It.Is<string>(x => x == "fake-sbt-vec-eus-loc-cocktails-email-001"),
                It.Is<string>(x => x == "application/json"),
                It.Is<CancellationToken>(x => x == this.httpContext.RequestAborted)), Times.Once());

        this.eventBusMock.VerifyNoOtherCalls();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async Task sendrecommendation__returns_bad_request_for_invalid_cocktail_name(string cocktailName)
    {
        // arrange
        var sp = this.SetupEnvironment((services) => { services.Replace(new ServiceDescriptor(typeof(IEventBus), this.eventBusMock.Object)); });
        var services = GetAsParameterServices<AccountsServices>(sp);

        var request = new CocktailRecommendationRq
        (
            Recommendation: new CocktailRecommendationModel
            (
                Name: cocktailName,
                Ingredients: "Test ingredients",
                Directions: "Test directions"
            ),
            VerificationCode: GuidString()
        );

        // act
        var ex = await Assert.ThrowsAsync<CocktailsApiValidationException>(async () => await AccountsApi.SendCocktailRecommendation(request, services));
        ex.Should().NotBeNull();

        // assert
        ex.Errors.Should().NotBeNull();
        ex.Errors.Should().HaveCount(1);
        ex.Errors[0].AttemptedValue.Should().Be(cocktailName);
        ex.Errors[0].ErrorCode.Should().Be("NotEmptyValidator");
        ex.Errors[0].ErrorMessage.Should().Be("Cocktail name is required");

        this.eventBusMock.VerifyNoOtherCalls();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async Task sendrecommendation__returns_bad_request_for_invalid_cocktail_ingredients(string ingredients)
    {
        // Arrange
        var sp = this.SetupEnvironment((services) => { services.Replace(new ServiceDescriptor(typeof(IEventBus), this.eventBusMock.Object)); });
        var services = GetAsParameterServices<AccountsServices>(sp);

        var request = new CocktailRecommendationRq
        (
            Recommendation: new CocktailRecommendationModel
            (
                Name: "Rons Cocktail",
                Ingredients: ingredients,
                Directions: "Test directions"
            ),
            VerificationCode: GuidString()
        );

        // act
        var ex = await Assert.ThrowsAsync<CocktailsApiValidationException>(async () => await AccountsApi.SendCocktailRecommendation(request, services));
        ex.Should().NotBeNull();

        // assert
        ex.Errors.Should().NotBeNull();
        ex.Errors.Should().HaveCount(1);
        ex.Errors[0].AttemptedValue.Should().Be(ingredients);
        ex.Errors[0].ErrorCode.Should().Be("NotEmptyValidator");
        ex.Errors[0].ErrorMessage.Should().Be("Ingredients are required");

        this.eventBusMock.VerifyNoOtherCalls();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async Task sendrecommendation__returns_bad_request_for_invalid_cocktail_directions(string directions)
    {
        // Arrange
        var sp = this.SetupEnvironment((services) => { services.Replace(new ServiceDescriptor(typeof(IEventBus), this.eventBusMock.Object)); });
        var services = GetAsParameterServices<AccountsServices>(sp);

        var request = new CocktailRecommendationRq
        (
            Recommendation: new CocktailRecommendationModel
            (
                Name: "Rons Cocktail",
                Ingredients: "Test ingredients",
                Directions: directions
            ),
            VerificationCode: "test"
        );

        // act
        var ex = await Assert.ThrowsAsync<CocktailsApiValidationException>(async () => await AccountsApi.SendCocktailRecommendation(request, services));
        ex.Should().NotBeNull();

        // assert
        ex.Errors.Should().NotBeNull();
        ex.Errors.Should().ContainSingle();
        ex.Errors[0].AttemptedValue.Should().Be(directions);
        ex.Errors[0].ErrorCode.Should().Be("NotEmptyValidator");
        ex.Errors[0].ErrorMessage.Should().Be("Directions are required");

        this.eventBusMock.VerifyNoOtherCalls();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async Task sendrecommendation__returns_bad_request_for_invalid_verification_code(string verificationCode)
    {
        // Arrange
        var sp = this.SetupEnvironment((services) => { services.Replace(new ServiceDescriptor(typeof(IEventBus), this.eventBusMock.Object)); });
        var services = GetAsParameterServices<AccountsServices>(sp);

        var request = new CocktailRecommendationRq
        (
            Recommendation: new CocktailRecommendationModel
            (
                Name: "Rons Cocktail",
                Ingredients: "Test ingredients",
                Directions: "Do it"
            ),
            VerificationCode: verificationCode
        );

        // act
        var ex = await Assert.ThrowsAsync<CocktailsApiValidationException>(async () => await AccountsApi.SendCocktailRecommendation(request, services));
        ex.Should().NotBeNull();

        // assert
        ex.Errors.Should().NotBeNull();
        ex.Errors.Should().HaveCount(1);
        ex.Errors[0].AttemptedValue.Should().Be(verificationCode);
        ex.Errors[0].ErrorCode.Should().Be("NotEmptyValidator");
        ex.Errors[0].ErrorMessage.Should().Be("Recaptcha code required");

        this.eventBusMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task sendrecommendation__returns_bad_gateway_when_message_send_fails()
    {
        var mockLogger = new Mock<ILogger<CocktailRecommendationCommandHandler>>();
        var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{\r\n  \"success\": true,\r\n  \"challenge_ts\": \"2024-05-26T12:32:12\",\r\n  \"hostname\": \"cezzis.com\",\r\n  \"error-codes\": []\r\n}")
        };

        var httpClientMock = new Mock<HttpClient>();

        httpClientMock
            .Setup(x => x.SendAsync(
                It.IsAny<HttpRequestMessage>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(httpResponse);

        this.httpClientFactoryMock
            .Setup(x => x.CreateClient(It.Is<string>(x => x == RecaptchaSiteVerifyService.HttpClientName)))
            .Returns(httpClientMock.Object);

        // Arrange
        var sp = this.SetupEnvironment((services) =>
        {
            services.Replace(new ServiceDescriptor(typeof(IEventBus), this.eventBusMock.Object));
            services.Replace(new ServiceDescriptor(typeof(ILogger<CocktailRecommendationCommandHandler>), mockLogger.Object));
        });
        var services = GetAsParameterServices<AccountsServices>(sp);

        var request = new CocktailRecommendationRq
        (
            Recommendation: new CocktailRecommendationModel
            (
                Name: "Rons Cocktail",
                Ingredients: "Test ingredients",
                Directions: "Do it"
            ),
            VerificationCode: Guid.NewGuid().ToString()
        );

        this.eventBusMock
            .Setup(x => x.PublishAsync(
                It.IsAny<CocktailRecommendationEmailEvent>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .Throws(new Exception("Send failed"));

        // act
        var response = (await AccountsApi.SendCocktailRecommendation(request, services))?.Result as JsonHttpResult<ProblemDetails>;
        var result = response?.Value;

        // assert
        result.Should().NotBeNull();
        response.StatusCode.Should().Be((int)HttpStatusCode.BadGateway);

        result.Detail.Should().Be("One or more validation errors has occurred");
        result.Title.Should().Be("Validation error");
        result.Type.Should().Be("ValidationFailure");
        result.Status.Should().Be(StatusCodes.Status502BadGateway);
        result.Extensions.Should().NotBeNull();
        result.Extensions.Count.Should().Be(1);

        var failures = result.Extensions["errors"] as List<ValidationFailure>;
        failures.Should().NotBeNull();
        failures.Count.Should().Be(1);
        failures[0].AttemptedValue.Should().BeNull();
        failures[0].ErrorCode.Should().BeNull();
        failures[0].ErrorMessage.Should().Be("Failed to send recommendation");

        this.eventBusMock
            .Verify(x => x.PublishAsync(
                It.Is<CocktailRecommendationEmailEvent>(x =>
                    x.Subject == "New Cocktail Recommendation for Cezzis.Com" &&
                    x.Attachments.Count == 0 &&
                    x.From.Address == "cezzi@cezzis.com" &&
                    x.From.DisplayName == "DoNotReply" &&
                    x.To.Count == 1 &&
                    x.To.First().Address == "cezzi@cezzis.com" &&
                    x.To.First().DisplayName == "Caesar" &&
                    x.Body.Contains(request.Recommendation.GetRecommendation())),
                It.Is<string>(x => x == "email-svc"),
                It.Is<string>(x => x == "pubsub-sb-topics-cocktails-email"),
                It.Is<string>(x => x == "fake-sbt-vec-eus-loc-cocktails-email-001"),
                It.Is<string>(x => x == "application/json"),
                It.Is<CancellationToken>(x => x == this.httpContext.RequestAborted)), Times.Once());

        this.eventBusMock.VerifyNoOtherCalls();

        mockLogger.Verify(logger => logger.Log(
            It.Is<LogLevel>(x => x == LogLevel.Critical),
            It.Is<EventId>(x => x.Id == 0),
            It.Is<It.IsAnyType>((@object, @type) => @object.ToString() == "Failed to send email message to topic" && @type.Name == "FormattedLogValues"),
            It.Is<Exception>(x => x.Message == "Send failed"),
            It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once);
    }
}
