namespace Cocktails.Api.Domain.Config;

public class LocalhostImagesConfig
{
    public const string SectionName = "LocalhostImages";

    public bool Enabled { get; set; }

    public string Path { get; set; }
}
