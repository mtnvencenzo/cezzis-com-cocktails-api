namespace Cocktails.Api.Infrastructure.Services;

using Cocktails.Api.Domain.Config;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

public class Auth0ManagementTokenService(IOptions<Auth0Config> auth0Config) : IAuth0ManagementTokenService
{
    private readonly IOptions<Auth0Config> auth0Config = auth0Config;
    private readonly SemaphoreSlim _tokenSemaphore = new(1, 1);
    private string _currentAccessToken;
    private DateTime _tokenExpiration;

    public async Task<string> GetManagementApiTokenAsync()
    {
        await this.EnsureTokenIsValidAsync();
        return this._currentAccessToken;
    }

    private async Task<string> GetManagementApiTokenInternalAsync()
    {
        using var client = new HttpClient();

        try
        {
            var data = $"grant_type=client_credentials&client_id={this.auth0Config.Value.ManagementM2MClientId}&client_secret={this.auth0Config.Value.ManagementM2MClientSecret}&audience={Uri.EscapeDataString(this.auth0Config.Value.ManagementDomain)}%2Fapi%2Fv2%2F";

            var content = new StringContent(data, Encoding.UTF8, "application/x-www-form-urlencoded");
            var response = await client.PostAsync($"{this.auth0Config.Value.ManagementDomain}/oauth/token", content);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();

            var tokenResponse = JsonSerializer.Deserialize<JsonNode>(responseContent);

            if (tokenResponse is JsonObject jsonObject && jsonObject.ContainsKey("access_token"))
            {
                return tokenResponse["access_token"].ToString();
            }

            throw new InvalidOperationException("Failed to parse Auth0 token response as json object.");
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException("Failed to parse Auth0 token response.", ex);
        }
    }

    private async Task EnsureTokenIsValidAsync()
    {
        // Double-check locking pattern for thread safety
        if (string.IsNullOrEmpty(this._currentAccessToken) || this._tokenExpiration <= DateTime.UtcNow.AddMinutes(5))
        {
            await this._tokenSemaphore.WaitAsync();
            try
            {
                // Recheck condition after acquiring the lock in case another thread already refreshed the token
                if (string.IsNullOrEmpty(this._currentAccessToken) || this._tokenExpiration <= DateTime.UtcNow.AddMinutes(5))
                {
                    this._currentAccessToken = await this.GetManagementApiTokenInternalAsync();
                    // In a real scenario, the token response would also contain 'expires_in'
                    // to calculate the exact _tokenExpiration. For simplicity, we'll assume a fixed duration.
                    this._tokenExpiration = DateTime.UtcNow.AddHours(18); // Assuming default 24-hour validity (making it a little shorter)
                }
            }
            finally
            {
                this._tokenSemaphore.Release();
            }
        }
    }
}