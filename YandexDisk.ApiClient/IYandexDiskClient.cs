using CSharpFunctionalExtensions;
using YandexDisk.ApiClient.Responses;

namespace YandexDisk.ApiClient;

public interface IYandexDiskClient : IDisposable
{
    /// <summary>
    ///    Get meta information about the disk
    /// </summary>
    /// <param name="ct">Cancellation token</param>
    /// <returns><see cref="DiskInfo"/> with meta information about the disk</returns>
    Task<Result<DiskInfo, YndxDiskError>> GetDiskInfoAsync(CancellationToken ct = default);

    /// <summary>
    ///     Get meta information about the resources in the specified folder
    /// </summary>
    /// <param name="path">Resource path</param>
    /// <param name="limit">Number of nested resources to return</param>
    /// <param name="offset">Number of resources to skip</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns><see cref="GetResourcesResponse"/> with meta information about the resources</returns>
    Task<Result<GetResourcesResponse, YndxDiskError>> GetResources(string path, int limit = 50,
        int offset = 0, CancellationToken ct = default);

    /// <summary>
    ///     Get file list ordered by name
    /// </summary>
    /// <param name="limit">Number of files to return</param>
    /// <param name="offset">Number of files to skip</param>
    /// <param name="sort">Field to sort by</param>
    /// <param name="mediaType">Media type to filter by</param>
    /// <param name="fields">Fields to include in the response</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns><see cref="GetFilesResponse"/> with meta information about the files</returns>
    Task<Result<GetFilesResponse, YndxDiskError>> GetFiles(int limit, int offset, string? sort = null,
        string? mediaType = null, string? fields = null, CancellationToken ct = default);

    /// <summary>
    ///     Create folder on the disk
    /// </summary>
    /// <param name="path">Folder path</param>
    /// <returns><see cref="CreateFolderResponse"/> with meta information about the created folder</returns>
    Task<Result<CreateFolderResponse, YndxDiskError>> CreateFolder(string path);

    /// <summary>
    ///     Get download link for the resource
    /// </summary>
    /// <param name="path">Resource path</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns><see cref="YndxResponse"/> with download link</returns>
    Task<Result<YndxResponse, YndxDiskError>> GetDownloadLink(string path, CancellationToken ct = default);

    /// <summary>
    ///     Copy resource to the specified path
    /// </summary>
    /// <param name="from">Resource path</param>
    /// <param name="to">Destination path</param>
    /// <param name="overwrite">Overwrite existing resource</param>
    /// <param name="forceAsync">Force async operation</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns><see cref="YndxResponse"/> with operation status</returns>
    Task<Result<YndxResponse, YndxDiskError>> CopyResource(string from, string to, bool overwrite = true,
        bool forceAsync = false, CancellationToken ct = default);

    /// <summary>
    ///     Move resource to the specified path
    /// </summary>
    /// <param name="from">Resource path</param>
    /// <param name="to">Destination path</param>
    /// <param name="overwrite">Overwrite existing resource</param>
    /// <param name="forceAsync">Force async operation</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns><see cref="YndxResponse"/> with operation status</returns>
    Task<Result<YndxResponse, YndxDiskError>> MoveResource(string from, string to, bool overwrite = true,
        bool forceAsync = false, CancellationToken ct = default);

    /// <summary>
    ///     Delete resource (file or folder)
    /// </summary>
    /// <param name="path">Resource path</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns><see cref="YndxResponse"/> with operation status</returns>
    Task<Result<YndxResponse, YndxDiskError>> DeleteResource(string path, CancellationToken ct = default);

    /// <summary>
    ///     Upload file to the specified path
    /// </summary>
    /// <param name="destinationPath">Destination path</param>
    /// <param name="sourcePath">Local file path</param>
    /// <param name="overwrite">Overwrite existing resource</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns><see cref="YndxResponse"/> with operation status</returns>
    Task<Result<YndxResponse, YndxDiskError>> UploadFile(string destinationPath, string sourcePath,
        bool overwrite = true, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Upload file to the specified path
    /// </summary>
    /// <param name="destinationPath">Destination path</param>
    /// <param name="streamContent">Stream content</param>
    /// <param name="overwrite">Overwrite existing resource</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns><see cref="YndxResponse"/> with operation status</returns>
    Task<Result<YndxResponse, YndxDiskError>> UploadFile(string destinationPath,
        StreamContent streamContent, bool overwrite = true, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Get information about the asynchronous operation
    /// </summary>
    /// <param name="operationId">Operation ID</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns><see cref="AsyncOperationResponse"/> with operation status</returns>
    Task<Result<AsyncOperationResponse, YndxDiskError>> GetAsyncOperationStatus(string operationId,
        CancellationToken ct = default);
}