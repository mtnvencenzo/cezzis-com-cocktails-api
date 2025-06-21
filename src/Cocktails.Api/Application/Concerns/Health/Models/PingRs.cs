namespace Cocktails.Api.Application.Concerns.Health.Models;

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

#pragma warning disable format

/// <summary>The cocktail recipe response</summary>
[type: Description("The cocktail recipe response")]
public record PingRs
(
    [property: Required()]
    [property: Description("The name of the machine.")]
    string MachineName = null,

    [property: Required()]
    [property: Description("The version")]
    string Version = null,

    [property: Required()]
    [property: Description("Gets a value indicating whether [is64 bit operating system]")]
    bool Is64BitOperatingSystem = true,

    [property: Required()]
    [property: Description("Gets a value indicating whether [is64 bit process]")]
    bool Is64BitProcess = true,

    [property: Required()]
    [property: Description("Gets the processor count")]
    int ProcessorCount = 1,

    [property: Required()]
    [property: Description("Gets the os version")]
    string OSVersion = null,

    [property: Required()]
    [property: Description("Gets the working set")]
    long WorkingSet = 0
);