namespace Cocktails.Api.Application.Concerns.LegalDocuments.Models;

using System.ComponentModel;

#pragma warning disable format

/// <summary>The document format type</summary>
[type: Description("The document format type")]
public enum DocumentFormat
{
    /// <summary>Specifies that the document is in markdown format</summary>
    [field: Description("Specifies that the document is in markdown format")]
    Markdown = 1
}
