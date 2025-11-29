namespace Cocktails.Api.Domain.Config;

public class BlobStorageConfig
{
    public const string SectionName = "BlobStorage";

    public required string CdnHostName { get; set; }

    public required AccountAvatarsBlobConfig AccountAvatars { get; set; }
}
