using System.Text.Json.Serialization;

namespace YandexDisk.ApiClient.Responses;

public class GetResourcesResponse
{
    public string Name { get; set; }
    
    [JsonPropertyName("_embedded")]
    public ResourceEmbedded Embedded { get; set; }
    
    public string Type { get; set; }
}

public class ResourceEmbedded
{
    public IReadOnlyCollection<ResourceItemMetaInfo> Items { get; set; }
    public string Path { get; set; }
    public long Total { get; set; }
    public int Limit { get; set; }
    public int Offset { get; set; }
}

public class ResourceItemMetaInfo
{
    public string Name { get; set; }
    public string File { get; set; }
    public string Type { get; set; }
    public long Size { get; set; }
    public string MimeType { get; set; }
    public string MediaType { get; set; }
    public string Path { get; set; }
}