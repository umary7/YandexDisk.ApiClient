using System.Net;
using System.Net.Http.Headers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using YandexDisk.ApiClient.Extensions;
using YandexDisk.ApiClient.Models;

namespace YandexDisk.ApiClient;

public class YandexDiskClient : IDisposable
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

    public async Task<YndxDiskResponse<DiskInfo>> GetDiskInfoAsync(CancellationToken ct = default)
    {
        var response = await SendAsync(new HttpRequestMessage(HttpMethod.Get, _httpClient.BaseAddress), ct)
            .ConfigureAwait(false);
        return await response.JsonParseResponseAsync<DiskInfo>(ct).ConfigureAwait(false);
    }

    public async Task<YndxDiskResponse<GetResourcesResponse>> GetResources(string path, int limit = 50, int offset = 0,
        CancellationToken ct = default)
    {
        var response = await SendAsync(new HttpRequestMessage(HttpMethod.Get,
            new Uri("resources", UriKind.Relative)
                .AddParameters(
                    ("path", path),
                    ("limit", limit.ToString()),
                    ("offset", offset.ToString()))), ct).ConfigureAwait(false);

        return await response.JsonParseResponseAsync<GetResourcesResponse>(ct).ConfigureAwait(false);
    }

    public async Task<YndxDiskResponse<CreateFolderResponse>> CreateFolder(string path)
    {
        var response = await SendAsync(new HttpRequestMessage(HttpMethod.Put,
            new Uri("resources", UriKind.Relative)
                .AddParameters(
                    ("path", path)))).ConfigureAwait(false);

        return await response.JsonParseResponseAsync<CreateFolderResponse>().ConfigureAwait(false);
    }

    public async Task<YndxDiskResponse> UploadFile(string destinationPath, string sourcePath, bool overwrite = true,
        CancellationToken cancellationToken = default)
    {
        var yDiskResponse = await UploadResource(destinationPath, overwrite, cancellationToken).ConfigureAwait(false);

        if (!yDiskResponse.Success) return new YndxDiskResponse { Success = false };

        _logger.LogInformation("Uploading {FileName}", sourcePath);

        try
        {
            using var content = new StreamContent(File.OpenRead(sourcePath));

            var response = await SendAsync(new HttpRequestMessage(HttpMethod.Put, yDiskResponse.Result?.Href)
            {
                Content = content
            }, cancellationToken).ConfigureAwait(false);
            
            if (response.StatusCode == HttpStatusCode.Created)
            {
                return new YndxDiskResponse { Success = true };
            }

            _logger.LogError("{FileName} was not uploaded. Response code: {ResponseCode}", sourcePath,
                response.StatusCode);
            return new YndxDiskResponse { Success = false };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{FileName} was not uploaded. Exception: {Message}", sourcePath, ex.Message);
            return new YndxDiskResponse { Success = false };
        }
    }

    private async Task<YndxDiskResponse<UploadResourceResponse>> UploadResource(string destinationPath,
        bool overwrite = true,
        CancellationToken ct = default)
    {
        var httpResponse = await SendAsync(new HttpRequestMessage(HttpMethod.Get,
            new Uri("resources/upload", UriKind.Relative)
                .AddParameters(
                    ("path", destinationPath),
                    ("overwrite", overwrite.ToString()))), ct).ConfigureAwait(false);

        return await httpResponse.JsonParseResponseAsync<UploadResourceResponse>(ct: ct).ConfigureAwait(false);
    }

    private async Task<HttpResponseMessage> SendAsync(HttpRequestMessage requestMessage,
        CancellationToken ct = default)
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

    public void Dispose()
    {
        _httpClient.Dispose();
    }
}