namespace Cocktails.Api.Application.Behaviors.ApimHostKeyAuthorization;

using Microsoft.AspNetCore.Authorization;

public class ApimHostKeyRequirement : IAuthorizationRequirement
{
    public const string PolicyName = "ApimHostKey";

    public ApimHostKeyRequirement() { }
}