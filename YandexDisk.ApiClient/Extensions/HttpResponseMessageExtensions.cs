using CSharpFunctionalExtensions;
using YandexDisk.ApiClient.Responses;

namespace YandexDisk.ApiClient.Extensions;

public static class HttpResponseMessageExtensions
{
    public static async Task<Result<T, YndxDiskError>> JsonParseResponseAsync<T>(this HttpResponseMessage httpResponseMessage, CancellationToken ct = default)
    {
        if (!httpResponseMessage.IsSuccessStatusCode)
        {
            var result = await httpResponseMessage.Content.ParseJsonAsync<YndxDiskError>(cancellationToken: ct);
            if (result == null)
            {
                return Result.Failure<T, YndxDiskError>(new YndxDiskError
                {
                    Message = "Unknown error",
                    Description = "Failed to parse error response",
                    Error = "Unknown"
                });
            }
            return Result.Failure<T, YndxDiskError>(result);
        }
        else
        {
            var result = await httpResponseMessage.Content.ParseJsonAsync<T>(cancellationToken: ct);
            if (result == null)
            {
                return Result.Failure<T, YndxDiskError>(new YndxDiskError
                {
                    Message = "Unknown error",
                    Description = "Failed to parse response",
                    Error = "Unknown"
                });
            }
            return Result.Success<T, YndxDiskError>(result);
        }
    }
}