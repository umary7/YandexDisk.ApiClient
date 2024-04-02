using System.Text.Json.Serialization;

namespace YandexDisk.ApiClient.Responses;

public record GetFilesResponse()
{
    public YandexFile[] Items { get; init; }
    public int Limit { get; init; }
    public int Offset { get; init; }
}

public record YandexFile
{
    public string Name { get; init; }
    public string ResourceId { get; init; }
    public string File { get; init; }
    public long Size { get; init; }
    public YandexFileShare Share { get; init; }
    public string PhotosliceTime { get; init; }
    public string MediaType { get; init; }
    public string Type { get; init; }
    public string MimeType { get; init; }
    public string PublicUrl { get; init; }
    public string Path { get; init; }
    public string Md5 { get; init; }
    public string PublicKey { get; init; }
    public string Sha256 { get; init; }
    public string Created { get; init; }
    public string Modified { get; init; }
    [JsonPropertyName("_embedded")]
    public YandexFileEmbedded Embedded { get; init; }
}

public record YandexFileShare
{
    public bool IsRoot { get; init; }
    public bool IsOwned { get; init; }
    public string Rights { get; init; }
}

public record YandexFileEmbedded
{
    public YandexFile[] Items { get; init; }
    public string Sort { get; init; }
    public string Path { get; init; }
    public int Limit { get; init; }
    public int Offset { get; init; }
    public int Total { get; init; }
}
