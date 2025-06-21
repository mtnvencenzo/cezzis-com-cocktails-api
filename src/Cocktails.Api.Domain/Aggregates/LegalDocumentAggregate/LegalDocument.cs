namespace Cocktails.Api.Domain.Aggregates.LegalDocumentAggregate;

using Cocktails.Api.Domain.Common;
using Cocktails.Api.Domain.Exceptions;

public class LegalDocument : Entity, IAggregateRoot
{
    public LegalDocument(string id, string contents, DocumentClassificationType classification, DocumentType type, DocumentFormatType format)
    {
        this.Id = !string.IsNullOrWhiteSpace(id)
            ? id
            : throw new CocktailsApiDomainException($"{nameof(id)} must be supplied and not empty");

        this.Contents = !string.IsNullOrWhiteSpace(contents)
            ? contents
            : throw new CocktailsApiDomainException($"{nameof(contents)} must be supplied and not empty");

        this.Classification = classification != DocumentClassificationType.None
            ? classification
            : throw new CocktailsApiDomainException($"{nameof(classification)} cannot be {nameof(DocumentClassificationType.None)}");

        this.Type = type != DocumentType.None
            ? type
            : throw new CocktailsApiDomainException($"{nameof(type)} cannot be {nameof(DocumentType.None)}");

        this.Format = format != DocumentFormatType.None
            ? format
            : throw new CocktailsApiDomainException($"{nameof(format)} cannot be {nameof(DocumentFormatType.None)}");
    }

    public string Contents { get; }

    public DocumentClassificationType Classification { get; }

    public DocumentType Type { get; }

    public DocumentFormatType Format { get; }
}
