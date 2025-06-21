namespace Cocktails.Api.Application.Concerns.Accounts.Models;

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

#pragma warning disable format

/// <summary>The profile image upload response</summary>
[type: Description("The profile image upload response")]
public record UploadProfileImageRs
(
    // <example>4493e636-6b1d-4t81-97b9-00d696c1g2f2</example>
    [property: Required()]
    [property: Description("The uri for the image that was uploaded")]
    string ImageUri
);