namespace Cocktails.Api.Domain.Aggregates.LegalDocumentAggregate;

using Cocktails.Api.Domain.Common;

public interface ILegalDocumentRepository : IRepository<LegalDocument>, IReadonlyRepository<LegalDocument>
{
}
