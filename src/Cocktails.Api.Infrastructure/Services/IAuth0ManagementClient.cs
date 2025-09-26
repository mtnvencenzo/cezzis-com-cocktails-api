namespace Cocktails.Api.Infrastructure.Services;

using Auth0.ManagementApi.Models;

public interface IAuth0ManagementClient
{
    Task<User> GetUser(string subjectId, CancellationToken cancellationToken);

    Task PatchUser(string subjectId, User user, CancellationToken cancellationToken);
}
