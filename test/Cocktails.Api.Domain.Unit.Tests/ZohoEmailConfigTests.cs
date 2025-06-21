namespace Cocktails.Api.Domain.Unit.Tests;

using Cocktails.Api.Domain.Config;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Xunit;

public class ZohoEmailConfigTests
{
    [Fact]
    public void zohoemailconfig__sectionname_should_return_correct_value()
    {
        // Arrange
        var expectedSectionName = "ZohoEmail";

        // Act
        var sectionName = ZohoEmailConfig.SectionName;

        // Assert
        sectionName.Should().Be(expectedSectionName);
    }

    [Fact]
    public void zohoemailconfig__smtphost_should_set_and_get_correctly()
    {
        // Arrange
        var zohoEmailConfig = new ZohoEmailConfig();
        var expectedSmtpHost = "smtp.example.com";

        // Act
        zohoEmailConfig.SmtpHost = expectedSmtpHost;
        var smtpHost = zohoEmailConfig.SmtpHost;

        // Assert
        smtpHost.Should().Be(expectedSmtpHost);
    }

    [Fact]
    public void zohoemailconfig__smtpport_should_set_and_get_correctly()
    {
        // Arrange
        var zohoEmailConfig = new ZohoEmailConfig();
        var expectedSmtpPort = 587;

        // Act
        zohoEmailConfig.SmtpPort = expectedSmtpPort;
        var smtpPort = zohoEmailConfig.SmtpPort;

        // Assert
        smtpPort.Should().Be(expectedSmtpPort);
    }

    [Fact]
    public void zohoemailconfig__cezziusername_should_set_and_get_correctly()
    {
        // Arrange
        var zohoEmailConfig = new ZohoEmailConfig
        {
            DefaultSender = new ZohoEmailSenderConfig()
        };

        var expectedCezziUsername = "cezzi@example.com";

        // Act
        zohoEmailConfig.DefaultSender.EmailAddress = expectedCezziUsername;

        var cezziUsername = zohoEmailConfig.DefaultSender.EmailAddress;

        // Assert
        cezziUsername.Should().Be(expectedCezziUsername);
    }

    [Fact]
    public void zohoemailconfig__cezziapppassword_should_set_and_get_correctly()
    {
        // Arrange
        var zohoEmailConfig = new ZohoEmailConfig
        {
            DefaultSender = new ZohoEmailSenderConfig()
        };
        var expectedCezziAppPassword = "cezzi123";

        // Act
        zohoEmailConfig.DefaultSender.AppPassword = expectedCezziAppPassword;

        var cezziAppPassword = zohoEmailConfig.DefaultSender.AppPassword;

        // Assert
        cezziAppPassword.Should().Be(expectedCezziAppPassword);
    }
}
