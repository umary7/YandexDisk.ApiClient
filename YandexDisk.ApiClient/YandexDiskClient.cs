using System.Net;
using System.Net.Http.Headers;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using YandexDisk.ApiClient.Extensions;
using YandexDisk.ApiClient.Responses;

namespace YandexDisk.ApiClient;

public sealed class YandexDiskClient : IYandexDiskClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<YandexDiskClient> _logger;

    public YandexDiskClient(HttpClient httpClient, ILogger<YandexDiskClient>? logger = null)
    {
        _logger = logger ?? NullLogger<YandexDiskClient>.Instance;
        _httpClient = httpClient;
    }

    public YandexDiskClient(string oauthToken, ILogger<YandexDiskClient> logger)
    {
        ArgumentNullException.ThrowIfNull(oauthToken, nameof(oauthToken));
        _logger = logger;
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://cloud-api.yandex.net/v1/disk/"),
            DefaultRequestHeaders =
            {
                Authorization = new AuthenticationHeaderValue("OAuth", oauthToken),
                Accept = { MediaTypeWithQualityHeaderValue.Parse("application/json") }
            }
        };
    }

    public YandexDiskClient(string oauthToken) : this(oauthToken, NullLogger<YandexDiskClient>.Instance)
    {
    }

    public async Task<Result<DiskInfo, YndxDiskError>> GetDiskInfoAsync(CancellationToken ct = default)
    {
        try
        {
            var response = await SendAsync(new HttpRequestMessage(HttpMethod.Get, _httpClient.BaseAddress), ct)
                .ConfigureAwait(false);
            return await response.JsonParseResponseAsync<DiskInfo>(ct).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while getting disk info");
            return Result.Failure<DiskInfo, YndxDiskError>(new YndxDiskError
            {
                Error = ex.Message,
                Message = ex.Message
            });
        }
    }

    public async Task<Result<GetResourcesResponse, YndxDiskError>> GetResources(string path, int limit = 50,
        int offset = 0, CancellationToken ct = default)
    {
        try
        {
            var response = await SendAsync(new HttpRequestMessage(HttpMethod.Get,
                new Uri("resources", UriKind.Relative)
                    .AddParameters(
                        ("path", path),
                        ("limit", limit.ToString()),
                        ("offset", offset.ToString()))), ct).ConfigureAwait(false);

            return await response.JsonParseResponseAsync<GetResourcesResponse>(ct).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while getting resources");
            return Result.Failure<GetResourcesResponse, YndxDiskError>(new YndxDiskError
            {
                Error = ex.Message,
                Message = ex.Message
            });
        }
    }

    public async Task<Result<GetFilesResponse, YndxDiskError>> GetFiles(int limit, int offset, string? sort = null,
        string? mediaType = null, string? fields = null, CancellationToken ct = default)
    {
        try
        {
            var uri = new Uri("resources/files", UriKind.Relative)
                .AddParameters(
                    ("limit", limit.ToString()),
                    ("offset", offset.ToString()));
            if (sort != null) uri = uri.AddParameters(("sort", sort));
            if (mediaType != null) uri = uri.AddParameters(("media_type", mediaType));
            if (fields != null) uri = uri.AddParameters(("fields", fields));

            var response = await SendAsync(new HttpRequestMessage(HttpMethod.Get, uri), ct).ConfigureAwait(false);

            return await response.JsonParseResponseAsync<GetFilesResponse>(ct).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while getting files");
            return Result.Failure<GetFilesResponse, YndxDiskError>(new YndxDiskError
            {
                Error = ex.Message,
                Message = ex.Message
            });
        }
    }

    public async Task<Result<CreateFolderResponse, YndxDiskError>> CreateFolder(string path)
    {
        try
        {
            var response = await SendAsync(new HttpRequestMessage(HttpMethod.Put,
                new Uri("resources", UriKind.Relative)
                    .AddParameters(
                        ("path", path)))).ConfigureAwait(false);

            return await response.JsonParseResponseAsync<CreateFolderResponse>().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while creating folder: {Error}", ex.Message);
            return Result.Failure<CreateFolderResponse, YndxDiskError>(new YndxDiskError
            {
                Error = ex.Message,
                Message = ex.Message
            });
        }
    }

    public async Task<Result<YndxResponse, YndxDiskError>> GetDownloadLink(string path, CancellationToken ct = default)
    {
        try
        {
            var response = await SendAsync(new HttpRequestMessage(HttpMethod.Get,
                new Uri("resources/download", UriKind.Relative)
                    .AddParameters(
                        ("path", path))), ct).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                return await response.JsonParseResponseAsync<YndxResponse>(ct).ConfigureAwait(false);
            }

            _logger.LogError("Error while getting download link: {ResponseCode}", response.StatusCode);
            return Result.Failure<YndxResponse, YndxDiskError>(new YndxDiskError
            {
                Error = response.StatusCode.ToString(),
                Message = response.StatusCode.ToString()
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while getting download link: {Error}", ex.Message);
            return Result.Failure<YndxResponse, YndxDiskError>(new YndxDiskError
            {
                Error = ex.Message,
                Message = ex.Message
            });
        }
    }

    public async Task<Result<YndxResponse, YndxDiskError>> CopyResource(string from, string to, bool overwrite = true,
        bool forceAsync = false, CancellationToken ct = default)
    {
        try
        {
            var response = await SendAsync(new HttpRequestMessage(HttpMethod.Post,
                new Uri("resources/copy", UriKind.Relative)
                    .AddParameters(
                        ("from", from),
                        ("path", to),
                        ("overwrite", overwrite.ToString()),
                        ("force_async", forceAsync.ToString())
                    )), ct).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                return await response.JsonParseResponseAsync<YndxResponse>(ct).ConfigureAwait(false);
            }

            _logger.LogError("Error while copying resource: {ResponseCode}", response.StatusCode);
            return Result.Failure<YndxResponse, YndxDiskError>(new YndxDiskError
            {
                Error = response.StatusCode.ToString(),
                Message = response.StatusCode.ToString()
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while copying resource: {Error}", ex.Message);
            return Result.Failure<YndxResponse, YndxDiskError>(new YndxDiskError
            {
                Error = ex.Message,
                Message = ex.Message
            });
        }
    }

    public async Task<Result<YndxResponse, YndxDiskError>> MoveResource(string from, string to, bool overwrite = true,
        bool forceAsync = false, CancellationToken ct = default)
    {
        try
        {
            var response = await SendAsync(new HttpRequestMessage(HttpMethod.Post,
                new Uri("resources/move", UriKind.Relative)
                    .AddParameters(
                        ("from", from),
                        ("path", to),
                        ("overwrite", overwrite.ToString()),
                        ("force_async", forceAsync.ToString())
                    )), ct).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                return await response.JsonParseResponseAsync<YndxResponse>(ct).ConfigureAwait(false);
            }

            _logger.LogError("Error while moving resource: {ResponseCode}", response.StatusCode);
            return Result.Failure<YndxResponse, YndxDiskError>(new YndxDiskError
            {
                Error = response.StatusCode.ToString(),
                Message = response.StatusCode.ToString()
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while moving resource: {Error}", ex.Message);
            return Result.Failure<YndxResponse, YndxDiskError>(new YndxDiskError
            {
                Error = ex.Message,
                Message = ex.Message
            });
        }
    }

    public async Task<Result<YndxResponse, YndxDiskError>> DeleteResource(string path, CancellationToken ct = default)
    {
        try
        {
            var response = await SendAsync(new HttpRequestMessage(HttpMethod.Delete,
                new Uri("resources", UriKind.Relative)
                    .AddParameters(
                        ("path", path))), ct).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                return Result.Success<YndxResponse, YndxDiskError>(new YndxResponse());
            }

            _logger.LogError("Error while deleting resource: {ResponseCode}", response.StatusCode);
            return Result.Failure<YndxResponse, YndxDiskError>(new YndxDiskError
            {
                Error = response.StatusCode.ToString(),
                Message = response.StatusCode.ToString()
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while deleting resource: {Error}", ex.Message);
            return Result.Failure<YndxResponse, YndxDiskError>(new YndxDiskError
            {
                Error = ex.Message,
                Message = ex.Message
            });
        }
    }

    public async Task<Result<YndxResponse, YndxDiskError>> UploadFile(string destinationPath, string sourcePath,
        bool overwrite = true, CancellationToken cancellationToken = default)
    {
        try
        {
            var yDiskResponse =
                await UploadResource(destinationPath, overwrite, cancellationToken).ConfigureAwait(false);

            if (yDiskResponse.IsFailure) return Result.Failure<YndxResponse, YndxDiskError>(yDiskResponse.Error);

            _logger.LogInformation("Uploading {FileName}", sourcePath);

            using var content = new StreamContent(File.OpenRead(sourcePath));

            var response = await SendAsync(new HttpRequestMessage(HttpMethod.Put, yDiskResponse.Value.Href)
            {
                Content = content
            }, cancellationToken).ConfigureAwait(false);

            if (response.StatusCode == HttpStatusCode.Created)
            {
                return await response.JsonParseResponseAsync<YndxResponse>(ct: cancellationToken)
                    .ConfigureAwait(false);
            }

            _logger.LogError("{FileName} was not uploaded. Response code: {ResponseCode}", sourcePath,
                response.StatusCode);
            return Result.Failure<YndxResponse, YndxDiskError>(new YndxDiskError
            {
                Error = response.StatusCode.ToString(),
                Message = response.StatusCode.ToString()
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{FileName} was not uploaded. Error: {Message}", sourcePath, ex.Message);
            return Result.Failure<YndxResponse, YndxDiskError>(new YndxDiskError
            {
                Error = ex.Message,
                Message = ex.Message
            });
        }
    }

    public async Task<Result<YndxResponse, YndxDiskError>> UploadFile(string destinationPath,
        StreamContent streamContent, bool overwrite = true, CancellationToken cancellationToken = default)
    {
        try
        {
            var yDiskResponse =
                await UploadResource(destinationPath, overwrite, cancellationToken).ConfigureAwait(false);

            if (yDiskResponse.IsFailure) return Result.Failure<YndxResponse, YndxDiskError>(yDiskResponse.Error);

            _logger.LogInformation("Uploading {FileName}", destinationPath);

            var request = new HttpRequestMessage(HttpMethod.Put, yDiskResponse.Value.Href)
            {
                Content = streamContent
            };

            var response = await SendAsync(request, cancellationToken).ConfigureAwait(false);

            if (response.StatusCode == HttpStatusCode.Created)
            {
                return await response.JsonParseResponseAsync<YndxResponse>(ct: cancellationToken)
                    .ConfigureAwait(false);
            }

            _logger.LogError("{FileName} was not uploaded. Response code: {ResponseCode}", destinationPath,
                response.StatusCode);
            return Result.Failure<YndxResponse, YndxDiskError>(new YndxDiskError
            {
                Error = response.StatusCode.ToString(),
                Message = response.StatusCode.ToString()
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{FileName} was not uploaded. Error: {Message}", destinationPath, ex.Message);
            return Result.Failure<YndxResponse, YndxDiskError>(new YndxDiskError
            {
                Error = ex.Message,
                Message = ex.Message
            });
        }
    }

    /// <summary>
    ///    Create link for uploading file to the specified path
    /// </summary>
    /// <param name="destinationPath">Destination path</param>
    /// <param name="overwrite">Overwrite existing resource</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns><see cref="UploadResourceResponse"/> with upload link</returns>
    private async Task<Result<UploadResourceResponse, YndxDiskError>> UploadResource(string destinationPath,
        bool overwrite = true, CancellationToken ct = default)
    {
        try
        {
            var httpResponse = await SendAsync(new HttpRequestMessage(HttpMethod.Get,
                new Uri("resources/upload", UriKind.Relative)
                    .AddParameters(
                        ("path", destinationPath),
                        ("overwrite", overwrite.ToString()))), ct).ConfigureAwait(false);

            return await httpResponse.JsonParseResponseAsync<UploadResourceResponse>(ct: ct).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while uploading resource: {Error}", ex.Message);
            return Result.Failure<UploadResourceResponse, YndxDiskError>(new YndxDiskError
            {
                Error = ex.Message,
                Message = ex.Message
            });
        }
    }

    public async Task<Result<AsyncOperationResponse, YndxDiskError>> GetAsyncOperationStatus(string operationId,
        CancellationToken ct = default)
    {
        try
        {
            var response = await SendAsync(new HttpRequestMessage(HttpMethod.Get,
                new Uri($"operations/{operationId}", UriKind.Relative)), ct).ConfigureAwait(false);

            return await response.JsonParseResponseAsync<AsyncOperationResponse>(ct).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while getting async operation status: {Error}", ex.Message);
            return Result.Failure<AsyncOperationResponse, YndxDiskError>(new YndxDiskError
            {
                Error = ex.Message,
                Message = ex.Message
            });
        }
    }

    private async Task<HttpResponseMessage> SendAsync(HttpRequestMessage requestMessage, CancellationToken ct = default)
    {
        try
        {
            ct.ThrowIfCancellationRequested();

            var response = await _httpClient.SendAsync(requestMessage, ct).ConfigureAwait(false);
            _logger.LogDebug("{Method} {Uri}: {StatusCode} {StatusCodeReason}", response.RequestMessage?.Method,
                response.RequestMessage?.RequestUri, (int)response.StatusCode, response.ReasonPhrase);

            if (!_logger.IsEnabled(LogLevel.Trace)) return response;

            if (requestMessage.Content != null)
            {
                var requestContent = await requestMessage.Content.ReadAsStringAsync(ct).ConfigureAwait(false);
                _logger.LogTrace("Request: {Request}", requestContent);
            }

            var responseContent = await response.Content.ReadAsStringAsync(ct).ConfigureAwait(false);
            _logger.LogTrace("{Response}", responseContent);

            return response;
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Request was cancelled");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while sending request: {RequestUri}. Error: {Message}",
                requestMessage.RequestUri,
                ex.Message);
            throw;
        }
    }

    public void Dispose()
    {
        _httpClient.Dispose();
    }
}