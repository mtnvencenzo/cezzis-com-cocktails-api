namespace Cocktails.Api.Infrastructure.Services;

using Azure.Storage.Blobs;
using Cocktails.Api.Domain.Config;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

public class StorageInitializer(
    IOptions<BlobStorageConfig> config,
    ILogger<StorageInitializer> logger)
{
    public async Task InitializeAsync()
    {
        try
        {
            logger.LogInformation("Starting storage initialization");

            try
            {
                var blobServiceClient = new BlobServiceClient(config.Value.AccountAvatars.ConnectionString);

                var containerClient = blobServiceClient.GetBlobContainerClient(config.Value.AccountAvatars.ContainerName);

                var exists = await containerClient.ExistsAsync();

                if (!exists)
                {
                    await blobServiceClient.CreateBlobContainerAsync(
                        blobContainerName: config.Value.AccountAvatars.ContainerName,
                        publicAccessType: Azure.Storage.Blobs.Models.PublicAccessType.BlobContainer);

                    logger.LogInformation("Created storage container {ContainerId}", config.Value.AccountAvatars.ContainerName);
                }
                else
                {
                    logger.LogInformation("Storage container {ContainerId} already exists", config.Value.AccountAvatars.ContainerName);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error creating container: {Message}", ex.Message);
                throw;
            }

            logger.LogInformation("Storage initialization completed successfully.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during storage initialization");
            throw;
        }
    }
}