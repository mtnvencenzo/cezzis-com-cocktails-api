namespace Cocktails.Api.Domain.Config;

public class AccountAvatarsBlobConfig
{
    public required string DaprBuildingBlock { get; set; }

    /// <summary>Only used for local development when starting the app up to create the blob containers automagically with the azurite emulator</summary>
    public required string ConnectionString { get; set; }

    /// <summary>Only used for local development when starting the app up to create the blob containers automagically with the azurite emulator</summary>
    public required string ContainerName { get; set; }
}
