namespace Cocktails.Api.Infrastructure.Services;

using Microsoft.Graph.Models;

public interface IMsGraphClient
{
    Task<User> GetUser(string subjectId, CancellationToken cancellationToken);

    Task PatchUser(string subjectId, User user, CancellationToken cancellationToken);
}
