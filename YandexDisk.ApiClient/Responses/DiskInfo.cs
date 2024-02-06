namespace YandexDisk.ApiClient.Responses;

public record DiskInfo
{
    public ulong PaidMaxFileSize { get; init; }
    public ulong TotalSpace { get; init; }
    public ulong UsedSpace { get; init; }
    public DiskUser User { get; init; }
}

public record DiskUser
{
    public long Uid { get; init; }
    public string Login { get; init; }
}