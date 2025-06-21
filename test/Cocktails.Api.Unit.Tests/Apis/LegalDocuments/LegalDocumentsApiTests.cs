namespace Cocktails.Api.Unit.Tests.Apis.LegalDocuments;

using FluentAssertions;
using global::Cocktails.Api.Apis.LegalDocuments;
using global::Cocktails.Api.Application.Concerns.LegalDocuments;
using global::Cocktails.Api.Application.Concerns.LegalDocuments.Models;
using global::Cocktails.Api.Infrastructure.Resources.Legal;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Text;
using Xunit;

public class LegalDocumentsApiTests : ServiceTestBase
{
    [Fact]
    public void getprivacypolicy__returns_correct_data()
    {
        // arrange
        var sp = this.SetupEnvironment();
        var services = GetAsParameterServices<LegalDocumentsServices>(sp);

        // act
        var response = LegalDocumentsApi.GetPrivacyPolicy(services) as Ok<LegalDocumentRs>;
        var result = response?.Value;

        // assert
        result.Should().NotBeNull();
        result.Document.Should().NotBeNullOrWhiteSpace();
        result.Document.Should().Contain("The data we collect from you will be stored for no longer than necessary. The length of time");
        result.Document.Should().Be(Encoding.UTF8.GetString(LegalDocuments.privacy_policy));
    }

    [Fact]
    public void gettermsofservice__returns_correct_data()
    {
        // arrange
        var sp = this.SetupEnvironment();
        var services = GetAsParameterServices<LegalDocumentsServices>(sp);

        // act
        var response = LegalDocumentsApi.GetTermsOfService(services) as Ok<LegalDocumentRs>;
        var result = response?.Value;

        // assert
        result.Should().NotBeNull();
        result.Document.Should().NotBeNullOrWhiteSpace();
        result.Document.Should().Contain("You acknowledge, understand and agree that your account is non-transferable and any rights to");
        result.Document.Should().Be(Encoding.UTF8.GetString(LegalDocuments.terms_of_service));
    }
}
