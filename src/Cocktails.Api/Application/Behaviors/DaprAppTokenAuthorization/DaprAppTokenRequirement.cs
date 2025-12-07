namespace Cocktails.Api.Application.Behaviors.DaprAppTokenAuthorization;

using Microsoft.AspNetCore.Authorization;

public class DaprAppTokenRequirement : IAuthorizationRequirement
{
    public const string PolicyName = "DaprAppToken";

    public DaprAppTokenRequirement() { }
}