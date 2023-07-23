using YandexDisk.ApiClient.Models;

namespace YandexDisk.ApiClient.Extensions;

public static class HttpResponseMessageExtensions
{
    public static async Task<YndxDiskResponse<T>> JsonParseResponseAsync<T>(this HttpResponseMessage httpResponseMessage, CancellationToken ct = default)
    {
        if (!httpResponseMessage.IsSuccessStatusCode)
        {
            var result = await httpResponseMessage.Content.ParseJsonAsync<YndxDiskError>(cancellationToken: ct);
            return new YndxDiskResponse<T> { Error = result, Result = default };
        }
        else
        {
            var result = await httpResponseMessage.Content.ParseJsonAsync<T>(cancellationToken: ct);
            return new YndxDiskResponse<T> { Success = true, Result = result };
        }
    }
}