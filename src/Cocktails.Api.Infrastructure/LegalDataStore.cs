namespace Cocktails.Api.Infrastructure;

using Cocktails.Api.Domain.Aggregates.LegalDocumentAggregate;
using Cocktails.Api.Domain.Common;
using Cocktails.Api.Infrastructure.Resources.Legal;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;

public class LegalDataStore : IUnitOfWork
{
    private readonly static List<LegalDocument> legalDocuments = [];
    private readonly static Lock sync = new();

    public ReadOnlyCollection<LegalDocument> Documents => this.GetLegalDocuments().AsReadOnly();

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public void Dispose() { }

    private List<LegalDocument> GetLegalDocuments()
    {
        if (legalDocuments.Count > 0)
        {
            return legalDocuments;
        }

        using (sync.EnterScope())
        {
            if (legalDocuments.Count == 0)
            {
                legalDocuments.Add(new LegalDocument(
                    id: "privacy-policy",
                    contents: Encoding.UTF8.GetString(LegalDocuments.privacy_policy),
                    classification: DocumentClassificationType.Legal,
                    type: DocumentType.PrivacyPolicy,
                    format: DocumentFormatType.Markdown));

                legalDocuments.Add(new LegalDocument(
                    id: "terms-of-service",
                    contents: Encoding.UTF8.GetString(LegalDocuments.terms_of_service),
                    classification: DocumentClassificationType.Legal,
                    type: DocumentType.TermsOfService,
                    format: DocumentFormatType.Markdown));
            }
        }

        return legalDocuments;
    }
}
