namespace Cocktails.Api.Application.Concerns.LegalDocuments.Queries;

using global::Cocktails.Api.Application.Concerns.LegalDocuments.Models;

public interface ILegalDocumentQueries
{
    LegalDocumentRs GetPrivacyPolicy();

    LegalDocumentRs GetTermsOfService();
}
