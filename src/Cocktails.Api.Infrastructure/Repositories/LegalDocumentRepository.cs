namespace Cocktails.Api.Infrastructure.Repositories;

using Cocktails.Api.Domain.Aggregates.LegalDocumentAggregate;
using Cocktails.Api.Domain.Common;

public class LegalDocumentRepository(LegalDataStore dataStore) : ILegalDocumentRepository
{
    public IUnitOfWork UnitOfWork => dataStore;

    public IQueryable<LegalDocument> Items => dataStore.Documents.AsQueryable();
}
