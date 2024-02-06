namespace YandexDisk.ApiClient.Responses;

public record YndxResponse
{
    public string Href { get; set; }

    public string Method { get; set; }

    public bool Templated { get; set; }
}