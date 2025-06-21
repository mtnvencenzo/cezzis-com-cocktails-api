namespace Cocktails.Api.Application.Concerns.LocalImages.Queries;

using global::Cocktails.Api.Domain.Config;
using Microsoft.Extensions.Options;

public class LocalImagesQueries(IOptions<LocalhostImagesConfig> imagesConfig) : ILocalImagesQueries
{
    public string GetImageFilePath(string imageName)
    {
        var file = new FileInfo($"{imagesConfig.Value.Path}\\{imageName}");

        if (file.Exists)
        {
            return file.FullName;
        }

        return null;
    }
}
