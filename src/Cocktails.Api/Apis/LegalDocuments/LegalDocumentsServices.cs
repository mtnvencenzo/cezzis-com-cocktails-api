namespace Cocktails.Api.Apis.LegalDocuments;

using Cocktails.Api.Application.Concerns.LegalDocuments.Queries;

/// <summary>
/// 
/// </summary>
/// <param name="queries"></param>
public class LegalDocumentsServices(ILegalDocumentQueries queries)
{
    /// <summary></summary>
    public ILegalDocumentQueries Queries { get; } = queries ?? throw new ArgumentNullException(nameof(queries));
}