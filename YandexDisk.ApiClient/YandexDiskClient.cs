using System.Net;
using System.Net.Http.Headers;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using YandexDisk.ApiClient.Extensions;
using YandexDisk.ApiClient.Responses;

namespace YandexDisk.ApiClient;

public sealed class YandexDiskClient : IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<YandexDiskClient> _logger;

    public YandexDiskClient(HttpClient httpClient, ILogger<YandexDiskClient> logger)
    {
        _logger = logger;
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
        int offset = 0,
        CancellationToken ct = default)
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

    public async Task<Result<YndxResponse, YndxDiskError>> UploadFile(string destinationPath, string sourcePath,
        bool overwrite = true,
        CancellationToken cancellationToken = default)
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
            _logger.LogError(ex, "{FileName} was not uploaded. Exception: {Message}", sourcePath, ex.Message);
            return Result.Failure<YndxResponse, YndxDiskError>(new YndxDiskError
            {
                Error = ex.Message,
                Message = ex.Message
            });
        }
    }

    private async Task<Result<UploadResourceResponse, YndxDiskError>> UploadResource(string destinationPath,
        bool overwrite = true,
        CancellationToken ct = default)
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

    private async Task<HttpResponseMessage> SendAsync(HttpRequestMessage requestMessage,
        CancellationToken ct = default)
    {
        try
        {
            ct.ThrowIfCancellationRequested();

            var response = await _httpClient.SendAsync(requestMessage, ct).ConfigureAwait(false);
            _logger.LogDebug("{Method} {Uri} : {StatusCode}", response.RequestMessage?.Method,
                response.RequestMessage?.RequestUri, response.StatusCode);

            if (_logger.IsEnabled(LogLevel.Trace))
            {
                var requestContent = await response.RequestMessage?.Content?.ReadAsStringAsync(ct);
                _logger.LogTrace("Request: {Request}", requestContent);

                var responseContent = await response.Content.ReadAsStringAsync(ct);
                _logger.LogTrace("{Response}", responseContent);
            }

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