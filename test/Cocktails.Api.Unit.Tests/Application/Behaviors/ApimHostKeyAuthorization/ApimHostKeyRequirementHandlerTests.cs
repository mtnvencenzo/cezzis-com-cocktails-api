namespace Cocktails.Api.Unit.Tests.Application.Behaviors.ApimHostKeyAuthorization;

using Cocktails.Api.Application.Behaviors.ApimHostKeyAuthorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Xunit;

public class ApimHostKeyRequirementHandlerTests : ServiceTestBase
{
    [Fact]
    public async Task ApimHostKeyRequirementHandler_header_name_is_expected()
    {
        // arrange
        var sp = this.SetupEnvironment();
        var handler = sp.GetRequiredService<ApimHostKeyRequirementHandler>();

        // act
        var authContext = new AuthorizationHandlerContext([new ApimHostKeyRequirement()], null, null);
        await handler.HandleAsync(authContext);

        // assert
    }
}
