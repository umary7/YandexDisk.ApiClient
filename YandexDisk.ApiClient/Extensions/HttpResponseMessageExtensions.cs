using System.Text.Json;
using CSharpFunctionalExtensions;
using YandexDisk.ApiClient.Responses;

namespace YandexDisk.ApiClient.Extensions;

public static class HttpResponseMessageExtensions
{
    public static async Task<Result<T, YndxDiskError>> JsonParseResponseAsync<T>(
        this HttpResponseMessage httpResponseMessage, CancellationToken ct = default)
    {
        if (httpResponseMessage.IsSuccessStatusCode)
        {
            if (httpResponseMessage.Content.Headers.ContentLength == 0 ||
                httpResponseMessage.StatusCode == System.Net.HttpStatusCode.NoContent)
            {
                // Return success with default(T) for no-content responses.
                return Result.Success<T, YndxDiskError>(default);
            }

            try
            {
                var result = await httpResponseMessage.Content.ParseJsonAsync<T>(ct);

                if (result == null) return Result.Success<T, YndxDiskError>(default);

                return Result.Success<T, YndxDiskError>(result);
            }
            catch (JsonException)
            {
                return Result.Failure<T, YndxDiskError>(new YndxDiskError
                {
                    Message = "Error parsing success response.",
                    Description = "Failed to parse the success response from JSON.",
                    Error = "ParseError"
                });
            }
        }

        try
        {
            var errorResult = await httpResponseMessage.Content.ParseJsonAsync<YndxDiskError>(ct);
            if (errorResult == null)
            {
                return Result.Failure<T, YndxDiskError>(new YndxDiskError
                {
                    Message = "No content in error response.",
                    Description = "The server returned a non-success status code without any content.",
                    Error = "NoContent"
                });
            }

            errorResult.Status = (int)httpResponseMessage.StatusCode;
            return Result.Failure<T, YndxDiskError>(errorResult);
        }
        catch (JsonException)
        {
            return Result.Failure<T, YndxDiskError>(new YndxDiskError
            {
                Message = "Error parsing error response.",
                Description = "Failed to parse the error response from JSON.",
                Error = "ParseError"
            });
        }
    }
}