namespace Cocktails.Api.Unit.Tests.Application.Queries.LegalDocuments;

using FluentAssertions;
using global::Cocktails.Api.Application.Concerns.LegalDocuments;
using global::Cocktails.Api.Application.Concerns.LegalDocuments.Models;
using global::Cocktails.Api.Application.Concerns.LegalDocuments.Queries;
using global::Cocktails.Api.Domain.Aggregates.LegalDocumentAggregate;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using Xunit;

public class LegalDocumentQueriesTests : ServiceTestBase
{
    private readonly Mock<ILegalDocumentRepository> mockLegalRepository;
    private readonly IQueryable<LegalDocument> fakeLegalDocuments;

    public LegalDocumentQueriesTests()
    {
        this.mockLegalRepository = new Mock<ILegalDocumentRepository>();

        List<LegalDocument> documents = [
            new LegalDocument("terms", "terms of service", DocumentClassificationType.Legal, DocumentType.TermsOfService, DocumentFormatType.Markdown),
            new LegalDocument("privacy-policy", "privacy policy", DocumentClassificationType.Legal, DocumentType.PrivacyPolicy, DocumentFormatType.Markdown)
        ];

        this.fakeLegalDocuments = documents.AsQueryable();

        this.mockLegalRepository
            .SetupGet(repo => repo.Items)
            .Returns(this.fakeLegalDocuments);
    }

    [Fact]
    public void getprivacypolicy__returns_correct_privacy_policy()
    {
        // arrange
        var sp = this.SetupEnvironment((services) =>
        {
            services.Replace(new ServiceDescriptor(typeof(ILegalDocumentRepository), this.mockLegalRepository.Object));
        });

        var svc = sp.GetRequiredService<ILegalDocumentQueries>();

        // act
        var result = svc.GetPrivacyPolicy();

        // assert
        result.Should().NotBeNull();
        result.Document.Should().Be("privacy policy");
        result.Format.Should().Be(DocumentFormat.Markdown);
    }

    [Fact]
    public void gettermsofservice__returns_correct_terms()
    {
        // arrange
        var sp = this.SetupEnvironment((services) =>
        {
            services.Replace(new ServiceDescriptor(typeof(ILegalDocumentRepository), this.mockLegalRepository.Object));
        });

        var queries = sp.GetRequiredService<ILegalDocumentQueries>();

        // act
        var result = queries.GetTermsOfService();

        // assert
        result.Should().NotBeNull();
        result.Document.Should().Be("terms of service");
        result.Format.Should().Be(DocumentFormat.Markdown);
    }
}
