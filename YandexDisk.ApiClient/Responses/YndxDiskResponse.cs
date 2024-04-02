namespace YandexDisk.ApiClient.Responses;

public record YndxDiskResponse<T>
{
    public bool Success { get; init; }
    public YndxDiskError? Error { get; init; }
    public T? Result { get; init; }
}
 
public record YndxDiskResponse
{
    public bool Success { get; init; }
    public YndxDiskError? Error { get; init; }
}