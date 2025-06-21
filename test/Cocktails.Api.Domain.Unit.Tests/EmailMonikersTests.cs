namespace Cocktails.Api.Domain.Unit.Tests;

using Cocktails.Api.Domain;
using FluentAssertions;
using Xunit;

public class EmailMonikersTests
{
    [Fact]
    public void SmtpHost_Should_Return_Correct_Value()
    {
        // Arrange
        var emailMonikers = new EmailMonikers();

        // Act
        var result = emailMonikers.SmtpHost;

        // Assert
        result.Should().Be("@cz_email_smtp_host");
    }

    [Fact]
    public void SmtpPort_Should_Return_Correct_Value()
    {
        // Arrange
        var emailMonikers = new EmailMonikers();

        // Act
        var result = emailMonikers.SmtpPort;

        // Assert
        result.Should().Be("@cz_email_smtp_port");
    }

    [Fact]
    public void EmailTo_Should_Return_Correct_Value()
    {
        // Arrange
        var emailMonikers = new EmailMonikers();

        // Act
        var result = emailMonikers.EmailTo;

        // Assert
        result.Should().Be("@cz_email_to");
    }

    [Fact]
    public void EmailCc_Should_Return_Correct_Value()
    {
        // Arrange
        var emailMonikers = new EmailMonikers();

        // Act
        var result = emailMonikers.EmailCc;

        // Assert
        result.Should().Be("@cz_email_cc");
    }

    [Fact]
    public void EmailBcc_Should_Return_Correct_Value()
    {
        // Arrange
        var emailMonikers = new EmailMonikers();

        // Act
        var result = emailMonikers.EmailBcc;

        // Assert
        result.Should().Be("@cz_email_bcc");
    }

    [Fact]
    public void EmailFrom_Should_Return_Correct_Value()
    {
        // Arrange
        var emailMonikers = new EmailMonikers();

        // Act
        var result = emailMonikers.EmailFrom;

        // Assert
        result.Should().Be("@cz_email_from");
    }

    [Fact]
    public void EmailReplyTo_Should_Return_Correct_Value()
    {
        // Arrange
        var emailMonikers = new EmailMonikers();

        // Act
        var result = emailMonikers.EmailReplyTo;

        // Assert
        result.Should().Be("@cz_email_replyto");
    }

    [Fact]
    public void EmailSubject_Should_Return_Correct_Value()
    {
        // Arrange
        var emailMonikers = new EmailMonikers();

        // Act
        var result = emailMonikers.EmailSubject;

        // Assert
        result.Should().Be("@cz_email_subject");
    }
}
