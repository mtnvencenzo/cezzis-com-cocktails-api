namespace Cocktails.Api.Domain.Services;

using System.Text.Json.Serialization;

public interface IIntegrationEvent
{
    [JsonInclude]
    string Id { get; set; }

    [JsonInclude]
    string CorrelationId { get; set; }

    [JsonInclude]
    DateTimeOffset CreationDate { get; set; }
}

