namespace Cocktails.Api.Infrastructure.Services;

using Auth0.AuthenticationApi;
using Auth0.AuthenticationApi.Models;
using Auth0.ManagementApi;
using Auth0.ManagementApi.Clients;
using Auth0.ManagementApi.Models;
using Cocktails.Api.Domain.Config;
using Microsoft.Extensions.Options;
using System.Threading;

public class Auth0ManagementClient(
    IAuth0ManagementTokenService auth0ManagementTokenService,
    IOptions<Auth0Config> auth0Config) : IAuth0ManagementClient
{
    public async Task<User> GetUser(string subjectId, CancellationToken cancellationToken)
    {
        try
        {
            var usersClient = await this.GetUsersClient();

            var user = await usersClient.GetAsync(subjectId, cancellationToken: cancellationToken);

            return user;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to get user from Auth0 due to invalid argument.", ex);
        }
    }

    public async Task PatchUser(string subjectId, User user, CancellationToken cancellationToken)
    {
        var userUpdateRequest = new UserUpdateRequest
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            FullName = user.FullName,
            UserMetadata = user.UserMetadata,
            ClientId = auth0Config.Value.ClientId,
            Connection = auth0Config.Value.DatabaseConnectionName
        };

        try
        {
            var usersClient = await this.GetUsersClient();

            await usersClient.UpdateAsync(subjectId, userUpdateRequest, cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to update user {subjectId} in Auth0.", ex);
        }
    }

    public async Task InitiateChangePasswordFlow(string email, CancellationToken cancellationToken)
    {
        try
        {
            var auth0Client = new AuthenticationApiClient(new Uri(auth0Config.Value.Domain));

            var request = new ChangePasswordRequest
            {
                Email = email,
                ClientId = auth0Config.Value.ClientId,
                Connection = auth0Config.Value.DatabaseConnectionName
            };

            await auth0Client.ChangePasswordAsync(request, cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to initiate change password flow for user {email} in Auth0.", ex);
        }
    }

    public async Task ChangeUserEmail(string subjectId, string email, CancellationToken cancellationToken)
    {
        try
        {
            var usersClient = await this.GetUsersClient();

            var userEmailRequest = new UserUpdateRequest
            {
                Email = email,
                EmailVerified = false,
                VerifyEmail = true,
                ClientId = auth0Config.Value.ClientId,
                Connection = auth0Config.Value.DatabaseConnectionName
            };

            await usersClient.UpdateAsync(subjectId, userEmailRequest, cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to initiate change email flow for user {email} in Auth0.", ex);
        }
    }

    public async Task ChangeUserUsername(string subjectId, string username, CancellationToken cancellationToken)
    {
        try
        {
            var usersClient = await this.GetUsersClient();

            var userEmailRequest = new UserUpdateRequest
            {
                UserName = username,
                ClientId = auth0Config.Value.ClientId,
                Connection = auth0Config.Value.DatabaseConnectionName
            };

            await usersClient.UpdateAsync(subjectId, userEmailRequest, cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to initiate change username flow for user {username} in Auth0.", ex);
        }
    }

    private async Task<IUsersClient> GetUsersClient()
    {
        var managementApiClient = new ManagementApiClient(
            await auth0ManagementTokenService.GetManagementApiTokenAsync(),
            new Uri($"{auth0Config.Value.ManagementDomain}/api/v2/"));

        return managementApiClient.Users;
    }
}
