namespace Cocktails.Api.Domain.Services;

using System.Threading;
using System.Threading.Tasks;

public interface IStorageBus
{
    Task<string> UpsertBlobAsync(
        byte[] data,
        string blobName,
        string contentType,
        string contentDisposition,
        string bindingName,
        Dictionary<string, string> metadata,
        CancellationToken cancellationToken = default);

    Task DeleteBlobAsync(
        string blobName,
        string bindingName,
        CancellationToken cancellationToken = default);
}
