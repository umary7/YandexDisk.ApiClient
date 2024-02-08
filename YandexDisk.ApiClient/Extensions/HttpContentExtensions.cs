using System.Net.Http.Json;
using System.Text.Json;
using YandexDisk.ApiClient.Formatting;

namespace YandexDisk.ApiClient.Extensions;

public static class HttpContentExtensions
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new JsonSerializerOptions
    {
        PropertyNamingPolicy = new SnakeCaseNamingPolicy()
    };

    public static async Task<T?> ParseJsonAsync<T>(this HttpContent content, CancellationToken cancellationToken = default)
    {
        try
        {
            return await content.ReadFromJsonAsync<T>(JsonSerializerOptions, cancellationToken);
        }
        catch (JsonException)
        {
            return default;
        }
    }
}