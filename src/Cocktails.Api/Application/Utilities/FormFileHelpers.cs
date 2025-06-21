namespace Cocktails.Api.Application.Utilities;

using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using System.Threading;

public static class FormFileHelpers
{
    public static async Task<byte[]> GetFileBytes(this IFormFile formFile, CancellationToken cancellationToken)
    {
        using var ms = new MemoryStream();
        using var s = formFile.OpenReadStream();
        await s.CopyToAsync(ms, cancellationToken);

        return ms.ToArray();
    }
}
