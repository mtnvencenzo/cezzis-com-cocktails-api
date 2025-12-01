namespace Cocktails.Api.Application.Utilities;

using Cocktails.Api.Domain.Services;
using System.Threading;

public static class FormFileHelpers
{
    public static async Task<byte[]> GetFileBytes(this IFileAccessor fileAccessor, CancellationToken cancellationToken)
    {
        using var ms = new MemoryStream();
        using var s = fileAccessor.GetStream();
        await s.CopyToAsync(ms, cancellationToken);

        return ms.ToArray();
    }
}
