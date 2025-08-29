namespace Cocktails.Api.Domain.Config;

public class AccountAvatarsBlobConfig
{
    public string DaprBuildingBlock { get; set; }

    /// <summary>Only used for local development when starting the app up to create the blob containers automagically with the azurite emulator</summary>
    public string ConnectionString { get; set; }

    /// <summary>Only used for local development when starting the app up to create the blob containers automagically with the azurite emulator</summary>
    public string ContainerName { get; set; }
}
