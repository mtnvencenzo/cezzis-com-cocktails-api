namespace Cocktails.Api.Unit.Tests.Application.Behaviors.ApimHostKeyAuthorization;

using Cocktails.Api.Application.Behaviors.ApimHostKeyAuthorization;
using FluentAssertions;
using Xunit;

public class ApimHostKeyRequirementTests
{
    [Fact]
    public void ApimHostKeyRequirement_policy_name_is_expected()
    {
        // act
        var policyName = ApimHostKeyRequirement.PolicyName;

        // assert
        policyName.Should().Be("ApimHostKey");
    }
}
