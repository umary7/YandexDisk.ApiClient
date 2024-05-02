namespace YandexDisk.ApiClient.Responses;

public record YndxDiskError
{
    /// <summary>
    ///     Error message.
    /// </summary>
    public string Message { get; init; }

    /// <summary>
    ///     Error description.
    /// </summary>
    public string Description { get; init; }

    /// <summary>
    ///     Error name.
    /// </summary>
    public string Error { get; init; }

    /// <summary>
    ///     HTTP status code.
    /// </summary>
    public int Status { get; internal set; } = -1;
}