namespace Cocktails.Api.Application.Concerns.LegalDocuments.Queries;

using global::Cocktails.Api.Application.Concerns.LegalDocuments.Models;
using global::Cocktails.Api.Domain.Aggregates.LegalDocumentAggregate;

public class LegalDocumentQueries(ILegalDocumentRepository legalDocumentRepository) : ILegalDocumentQueries
{
    public LegalDocumentRs GetPrivacyPolicy()
    {
        var document = legalDocumentRepository.Items
            .Where(x => x.Classification == DocumentClassificationType.Legal)
            .Where(x => x.Type == DocumentType.PrivacyPolicy)
            .Where(x => x.Format == DocumentFormatType.Markdown)
            .SingleOrDefault();

        return document != null
            ? new LegalDocumentRs
            (
                Format: DocumentFormat.Markdown,
                Document: document.Contents
            ) : null;
    }

    public LegalDocumentRs GetTermsOfService()
    {
        var document = legalDocumentRepository.Items
            .Where(x => x.Classification == DocumentClassificationType.Legal)
            .Where(x => x.Type == DocumentType.TermsOfService)
            .Where(x => x.Format == DocumentFormatType.Markdown)
            .SingleOrDefault();

        return document != null
            ? new LegalDocumentRs
            (
                Format: DocumentFormat.Markdown,
                Document: document.Contents
            ) : null;
    }
}
