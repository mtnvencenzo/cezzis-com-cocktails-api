namespace Cocktails.Api.Infrastructure.Services;

using System.Threading.Tasks;

public interface IAuth0ManagementTokenService
{
    Task<string> GetManagementApiTokenAsync();
}