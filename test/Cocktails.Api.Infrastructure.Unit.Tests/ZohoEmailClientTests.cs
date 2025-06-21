namespace Cocktails.Api.Infrastructure.Unit.Tests;

using Cocktails.Api.Infrastructure.Services;
using Cocktails.Common.Emails;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Linq;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

public class ZohoEmailClientTests : ServiceTestBase
{
    [Fact]
    public async Task zohoemailclient__sendemail_validmessage_sends_email()
    {
        // arrange
        var sp = this.SetupEnvironment();

        var message = new EmailMessage
        {
            Subject = "Test Subject",
            Body = "Test Body",
            From = new EmailAddress { Address = "HOOPlah1@cezzi.com", DisplayName = "From User" },
            To = [new() { Address = "to@example.com", DisplayName = "To User" }]
        };

        var zohoEmailClient = sp.GetRequiredService<IZohoEmailClient>();

        // Act
        await zohoEmailClient.SendEmail(message, default);

        // Assert
        this.smtpClientFactoryMock
            .Verify(x => x.CreateClient(
                It.Is<string>(x => x == "smtp-cezzis-unit-test.com"),
                It.Is<int>(x => x == 999)), Times.Once);

        this.smtpClientMock
            .Verify(x => x.SendMailAsync(
                It.Is<MailMessage>(x =>
                    x.Subject == "Test Subject" &&
                    x.Body == "Test Body" &&
                    x.From.Address == "hooplah1@cezzi.com" &&
                    x.From.DisplayName == "From User" &&
                    x.To.First().Address == "to@example.com" &&
                    x.To.First().DisplayName == "To User"),
                It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task zohoemailclient__sendemail_validmessage_and_only_subject_sends_email()
    {
        // arrange
        var sp = this.SetupEnvironment();

        var message = new EmailMessage
        {
            Subject = "Test Subject",
            Body = null,
            From = new EmailAddress { Address = "hooplah1@cezzi.com", DisplayName = "From User" },
            To = [new() { Address = "to@example.com", DisplayName = "To User" }]
        };

        var zohoEmailClient = sp.GetRequiredService<IZohoEmailClient>();

        // Act
        await zohoEmailClient.SendEmail(message, default);

        // Assert
        this.smtpClientFactoryMock
            .Verify(x => x.CreateClient(
                It.Is<string>(x => x == "smtp-cezzis-unit-test.com"),
                It.Is<int>(x => x == 999)), Times.Once);

        this.smtpClientMock
            .Verify(x => x.SendMailAsync(
                It.Is<MailMessage>(x =>
                    x.Subject == "Test Subject" &&
                    x.Body == string.Empty &&
                    x.From.Address == "hooplah1@cezzi.com" &&
                    x.From.DisplayName == "From User" &&
                    x.To.First().Address == "to@example.com" &&
                    x.To.First().DisplayName == "To User"),
                It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task zohoemailclient__sendemail_validmessage_and_only_body_sends_email()
    {
        // arrange
        var sp = this.SetupEnvironment();

        var message = new EmailMessage
        {
            Subject = null,
            Body = "Test body",
            From = new EmailAddress { Address = "hooplah1@cezzi.com", DisplayName = "From User" },
            To = [new() { Address = "to@example.com", DisplayName = "To User" }]
        };

        var zohoEmailClient = sp.GetRequiredService<IZohoEmailClient>();

        // act
        await zohoEmailClient.SendEmail(message, default);

        // assert
        this.smtpClientFactoryMock
            .Verify(x => x.CreateClient(
                It.Is<string>(x => x == "smtp-cezzis-unit-test.com"),
                It.Is<int>(x => x == 999)), Times.Once);

        this.smtpClientMock
            .Verify(x => x.SendMailAsync(
                It.Is<MailMessage>(x =>
                    x.Subject == string.Empty &&
                    x.Body == "Test body" &&
                    x.From.Address == "hooplah1@cezzi.com" &&
                    x.From.DisplayName == "From User" &&
                    x.To.First().Address == "to@example.com" &&
                    x.To.First().DisplayName == "To User"),
                It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SendEmail_EmptyBody_AndSubject_LogsWarning()
    {
        var loggerMock = new Mock<ILogger<ZohoEmailClient>>();

        // arrange
        var sp = this.SetupEnvironment((services) =>
        {
            services.Replace(new ServiceDescriptor(typeof(ILogger<ZohoEmailClient>), loggerMock.Object));
        });

        var message = new EmailMessage
        {
            Subject = "",
            Body = "",
            From = new EmailAddress { Address = "from@example.com", DisplayName = "From User" },
            To = [new() { Address = "to@example.com", DisplayName = "To User" }]
        };

        var zohoEmailClient = sp.GetRequiredService<IZohoEmailClient>();

        // Act
        await zohoEmailClient.SendEmail(message, default);

        // Assert
        loggerMock.Verify(logger => logger.Log(
            It.Is<LogLevel>(x => x == LogLevel.Warning),
            It.Is<EventId>(x => x.Id == 0),
            It.Is<It.IsAnyType>((@object, @type) => @object.ToString() == "Email subject and body are empty, skipping sending email" && @type.Name == "FormattedLogValues"),
            It.Is<Exception>(x => x == null),
            It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once);
    }

    [Fact]
    public async Task SendEmail_EmptyFromAddress_LogsWarning()
    {
        var loggerMock = new Mock<ILogger<ZohoEmailClient>>();

        // arrange
        var sp = this.SetupEnvironment((services) =>
        {
            services.Replace(new ServiceDescriptor(typeof(ILogger<ZohoEmailClient>), loggerMock.Object));
        });

        var message = new EmailMessage
        {
            Subject = "Test Subject",
            Body = "Test Body",
            From = new EmailAddress { Address = "", DisplayName = "From User" },
            To = [new() { Address = "to@example.com", DisplayName = "To User" }]
        };

        var zohoEmailClient = sp.GetRequiredService<IZohoEmailClient>();

        // Act
        await zohoEmailClient.SendEmail(message, default);

        // Assert
        loggerMock.Verify(logger => logger.Log(
            It.Is<LogLevel>(x => x == LogLevel.Warning),
            It.Is<EventId>(x => x.Id == 0),
            It.Is<It.IsAnyType>((@object, @type) => @object.ToString() == "Email from address is empty, skipping sending email" && @type.Name == "FormattedLogValues"),
            It.Is<Exception>(x => x == null),
            It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once);
    }

    [Fact]
    public async Task SendEmail_EmptyToAddress_LogsWarning()
    {
        // arrange
        var loggerMock = new Mock<ILogger<ZohoEmailClient>>();

        var sp = this.SetupEnvironment((services) =>
        {
            services.Replace(new ServiceDescriptor(typeof(ILogger<ZohoEmailClient>), loggerMock.Object));
        });

        var message = new EmailMessage
        {
            Subject = "Test Subject",
            Body = "Test Body",
            From = new EmailAddress { Address = "from@example.com", DisplayName = "From User" },
            To = []
        };

        var zohoEmailClient = sp.GetRequiredService<IZohoEmailClient>();

        // Act
        await zohoEmailClient.SendEmail(message, default);

        // Assert
        loggerMock.Verify(logger => logger.Log(
            It.Is<LogLevel>(x => x == LogLevel.Warning),
            It.Is<EventId>(x => x.Id == 0),
            It.Is<It.IsAnyType>((@object, @type) => @object.ToString() == "Email to address is empty, skipping sending email" && @type.Name == "FormattedLogValues"),
            It.Is<Exception>(x => x == null),
            It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once);
    }

    [Fact]
    public async Task SendEmail_CancellationRequested_LogsWarning()
    {
        // arrange
        var loggerMock = new Mock<ILogger<ZohoEmailClient>>();

        var sp = this.SetupEnvironment((services) =>
        {
            services.Replace(new ServiceDescriptor(typeof(ILogger<ZohoEmailClient>), loggerMock.Object));
        });

        var message = new EmailMessage
        {
            Subject = "Test Subject",
            Body = "Test Body",
            From = new EmailAddress { Address = "hooplah1@cezzi.com", DisplayName = "From User" },
            To = [new() { Address = "to@example.com", DisplayName = "To User" }]
        };

        var zohoEmailClient = sp.GetRequiredService<IZohoEmailClient>();
        var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;
        cancellationTokenSource.Cancel();

        // Act
        await zohoEmailClient.SendEmail(message, cancellationToken);

        // Assert
        loggerMock.Verify(logger => logger.Log(
            It.Is<LogLevel>(x => x == LogLevel.Warning),
            It.Is<EventId>(x => x.Id == 0),
            It.Is<It.IsAnyType>((@object, @type) => @object.ToString() == "Cancellation requested, skipping sending email" && @type.Name == "FormattedLogValues"),
            It.Is<Exception>(x => x == null),
            It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once);
    }

    [Fact]
    public async Task SendEmail_ExceptionThrown_LogsErrorAndThrows()
    {
        // arrange
        var loggerMock = new Mock<ILogger<ZohoEmailClient>>();
        var exception = new Exception("Test Exception");

        this.smtpClientMock
            .Setup(x => x.SendMailAsync(It.IsAny<MailMessage>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(exception);

        var sp = this.SetupEnvironment((services) =>
        {
            services.Replace(new ServiceDescriptor(typeof(ILogger<ZohoEmailClient>), loggerMock.Object));
        });

        var message = new EmailMessage
        {
            Subject = "Test Subject",
            Body = "Test Body",
            From = new EmailAddress { Address = "hooplah1@cezzi.com", DisplayName = "From User" },
            To = [new() { Address = "to@example.com", DisplayName = "To User" }]
        };

        var zohoEmailClient = sp.GetRequiredService<IZohoEmailClient>();

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(async () => await zohoEmailClient.SendEmail(message, default));

        loggerMock.Verify(logger => logger.Log(
            It.Is<LogLevel>(x => x == LogLevel.Error),
            It.Is<EventId>(x => x.Id == 0),
            It.Is<It.IsAnyType>((@object, @type) => @object.ToString() == "Email send failed" && @type.Name == "FormattedLogValues"),
            It.Is<Exception>(x => x == exception),
            It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once);

        this.smtpClientFactoryMock
            .Verify(x => x.CreateClient(
                It.Is<string>(x => x == "smtp-cezzis-unit-test.com"),
                It.Is<int>(x => x == 999)), Times.Once);

        this.smtpClientMock
            .Verify(x => x.SendMailAsync(
                It.Is<MailMessage>(x =>
                    x.Subject == "Test Subject" &&
                    x.Body == "Test Body" &&
                    x.From.Address == "hooplah1@cezzi.com" &&
                    x.From.DisplayName == "From User" &&
                    x.To.First().Address == "to@example.com" &&
                    x.To.First().DisplayName == "To User"),
                It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SendEmail_Invalid_Sender_LogsWarning()
    {
        // arrange
        var loggerMock = new Mock<ILogger<ZohoEmailClient>>();

        var sp = this.SetupEnvironment((services) =>
        {
            services.Replace(new ServiceDescriptor(typeof(ILogger<ZohoEmailClient>), loggerMock.Object));
        });

        var message = new EmailMessage
        {
            Subject = "Test Subject",
            Body = "Test Body",
            From = new EmailAddress { Address = "not-hooplah1@cezzi.com", DisplayName = "From User" },
            To = [new() { Address = "to@example.com", DisplayName = "To User" }]
        };

        var zohoEmailClient = sp.GetRequiredService<IZohoEmailClient>();
        var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;
        cancellationTokenSource.Cancel();

        // Act
        await zohoEmailClient.SendEmail(message, cancellationToken);

        // Assert
        loggerMock.Verify(logger => logger.Log(
            It.Is<LogLevel>(x => x == LogLevel.Warning),
            It.Is<EventId>(x => x.Id == 0),
            It.Is<It.IsAnyType>((@object, @type) => @object.ToString() == "Mismatched sender email, skipping sending email" && @type.Name == "FormattedLogValues"),
            It.Is<Exception>(x => x == null),
            It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once);
    }
}
