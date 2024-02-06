namespace YandexDisk.ApiClient.Responses;

/// <summary>
///   Yandex.Disk info
/// </summary>
public record DiskInfo
{
    public ulong PaidMaxFileSize { get; init; }
    public ulong TotalSpace { get; init; }
    public ulong UsedSpace { get; init; }
    public DiskUser User { get; init; }
}

/// <summary>
///   Yandex.Disk user
/// </summary>
public record DiskUser
{
    public long Uid { get; init; }
    public string Login { get; init; }
}