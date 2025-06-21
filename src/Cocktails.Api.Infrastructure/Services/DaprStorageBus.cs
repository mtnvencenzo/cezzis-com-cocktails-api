namespace Cocktails.Api.Infrastructure.Services;

using Cezzi.Applications;
using Cezzi.Applications.Extensions;
using Cocktails.Api.Domain;
using Cocktails.Api.Domain.Config;
using Cocktails.Api.Domain.Services;
using Dapr.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

public class DaprStorageBus(
    DaprClient daprClient,
    ILogger<DaprEventBus> logger) : IStorageBus
{
    private readonly DaprClient daprClient = daprClient ?? throw new ArgumentNullException(nameof(daprClient));
    private readonly ILogger logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly JsonSerializerOptions serializerOptions = JsonSerializerOptions.Web;

    public async Task<string> UpsertBlobAsync(
        byte[] data,
        string blobName,
        string contentType,
        string contentDisposition,
        string bindingName,
        Dictionary<string, string> metadata,
        CancellationToken cancellationToken = default)
    {
        Guard.NotNull(data, nameof(data));
        Guard.NotNull(metadata, nameof(metadata));
        Guard.NotNullOrWhiteSpace(blobName, nameof(blobName));
        Guard.NotNullOrWhiteSpace(contentType, nameof(contentType));
        Guard.NotNullOrWhiteSpace(bindingName, nameof(bindingName));

        using var logScope = this.logger.BeginScope(new Dictionary<string, object>
        {
            { Monikers.Azure.AzBlobName, blobName },
            { Monikers.Azure.AzBlobBytesCount, data.Length }
        });

        var bindingRequest = new BindingRequest(bindingName, "create")
        {
            Data = data
        };

        bindingRequest.Metadata.Add("blobName", blobName);
        bindingRequest.Metadata.Add("contentType", contentType);
        bindingRequest.Metadata.Add("extcontentType", contentType);

        metadata.ForEach(x => bindingRequest.Metadata.Add($"ext{x.Key}", x.Value));

        if (contentDisposition != null)
        {
            bindingRequest.Metadata.Add("contentDisposition", contentDisposition);
        }

        var bindingResponse = await this.daprClient.InvokeBindingAsync(bindingRequest, cancellationToken);

        if (bindingResponse != null)
        {
            var result = JsonSerializer.Deserialize<BlobResponse>(bindingResponse.Data.Span, this.serializerOptions);
            return result.BlobUrl.Replace("%2F", "/");
        }

        return null;
    }

    public async Task DeleteBlobAsync(
        string blobName,
        string bindingName,
        CancellationToken cancellationToken = default)
    {
        Guard.NotNullOrWhiteSpace(blobName, nameof(blobName));
        Guard.NotNullOrWhiteSpace(bindingName, nameof(bindingName));

        using var logScope = this.logger.BeginScope(new Dictionary<string, object>
        {
            { Monikers.Azure.AzBlobName, blobName }
        });

        var bindingRequest = new BindingRequest(bindingName, "delete");
        bindingRequest.Metadata.Add("blobName", blobName);
        bindingRequest.Metadata.Add("deleteSnapshots", "include"); // required for azurite storage to delete blobs with snapshots (include or only)  Even though dapr says its optional, it appears azurite requires it

        var bindingResponse = await this.daprClient.InvokeBindingAsync(bindingRequest, cancellationToken);
    }

    private record BlobResponse(string BlobUrl);
}
