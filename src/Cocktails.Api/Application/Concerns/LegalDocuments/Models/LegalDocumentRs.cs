namespace Cocktails.Api.Application.Concerns.LegalDocuments.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

#pragma warning disable format

/// <summary>The legal document response</summary>
[type: Description("The legal document response")]
public record LegalDocumentRs
(
    // <example>\uFEFF# ONLINE PRIVACY POLICY AGREEMENT\n\nJune 29, 2024\n\n</example>
    [property: Required()]
    [property: Description("The document content")]
    string Document,

    // <example>1</example>
    [property: Required()]
    [property: Description("The format that the document content is in")]
    DocumentFormat Format
);
