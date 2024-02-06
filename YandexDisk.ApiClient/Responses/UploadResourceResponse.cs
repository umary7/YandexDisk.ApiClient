namespace YandexDisk.ApiClient.Responses;

/// <summary>
///     Upload resource response. Returns href for uploading file.
/// </summary>
public record UploadResourceResponse
{
    public string OperationId { get; init; }
    public string Href { get; init; }
    public string Method { get; init; }
    public bool Templated { get; init; }
}