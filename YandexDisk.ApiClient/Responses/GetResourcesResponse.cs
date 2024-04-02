using System.Text.Json.Serialization;

namespace YandexDisk.ApiClient.Responses;

/// <summary>
///     Get resources response
/// </summary>
public record GetResourcesResponse
{
    public string Name { get; init; }

    [JsonPropertyName("_embedded")]
    public ResourceEmbedded Embedded { get; init; }

    public string Type { get; init; }
}

/// <summary>
///  Resource embedded info
/// </summary>
public record ResourceEmbedded
{
    public IReadOnlyCollection<ResourceItemMetaInfo> Items { get; init; }
    public string Path { get; init; }
    public long Total { get; init; }
    public int Limit { get; init; }
    public int Offset { get; init; }
}

/// <summary>
///     Resource item meta info
/// </summary>
public record ResourceItemMetaInfo
{
    public string Name { get; init; }
    public string File { get; init; }
    public string Type { get; init; }
    public long Size { get; init; }
    public string MimeType { get; init; }
    public string MediaType { get; init; }
    public string Path { get; init; }
}