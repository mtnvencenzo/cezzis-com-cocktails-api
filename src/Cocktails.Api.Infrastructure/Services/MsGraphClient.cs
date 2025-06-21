namespace Cocktails.Api.Infrastructure.Services;

using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using System.Threading;
using Microsoft.Graph.Models;

public class MsGraphClient(GraphServiceClient graphServiceClient) : IMsGraphClient
{
    public async Task<User> GetUser(string subjectId, CancellationToken cancellationToken)
    {
        var users = await graphServiceClient.Users.GetByIds.PostAsGetByIdsPostResponseAsync(
            body: new Microsoft.Graph.Users.GetByIds.GetByIdsPostRequestBody
            {
                Ids = [subjectId]
            },
            cancellationToken: cancellationToken);

        return users?.Value?.FirstOrDefault(x => x.Id == subjectId) as User;
    }

    public async Task PatchUser(string subjectId, User user, CancellationToken cancellationToken) => await graphServiceClient.Users[subjectId].PatchAsync(user, cancellationToken: cancellationToken);
}
