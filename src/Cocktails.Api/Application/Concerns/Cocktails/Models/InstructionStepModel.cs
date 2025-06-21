namespace Cocktails.Api.Application.Concerns.Cocktails.Models;

using global::Cocktails.Api.Application.Behaviors;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

#pragma warning disable format

/// <summary>The instructions step model</summary>
[type: Description("The instructions step model")]
public record InstructionStepModel
(
    [property: Required()]
    [property: Description("The displayable value for the instruction step")]
    [property: OpenApiExampleDoc<string>("Combine the gin, lemon juice, raspberry syrup, and egg white in a shaker without ice and dry shake (shake without ice) for about 10 seconds to make the foam.")]
    string Display,

    // <example>1</example>
    [property: Required()]
    [property: Description("The order of the instruction step in which it should be performed")]
    [property: OpenApiExampleDoc<int>(1)]
    int Order
);