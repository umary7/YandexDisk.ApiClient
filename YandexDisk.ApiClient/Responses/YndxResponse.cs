namespace YandexDisk.ApiClient.Responses;

public record YndxResponse
{
    public string Href { get; init; }

    public string Method { get; init; }

    public bool Templated { get; init; }
}