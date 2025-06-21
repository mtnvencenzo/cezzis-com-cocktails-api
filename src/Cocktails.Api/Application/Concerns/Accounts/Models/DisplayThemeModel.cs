namespace Cocktails.Api.Application.Concerns.Accounts.Models;

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

#pragma warning disable format

[type: Description("The display themes available for profile accessibility settings")]
public enum DisplayThemeModel
{
    Light = 1,
    Dark = 2
}