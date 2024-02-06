namespace YandexDisk.ApiClient.Responses;

public record YndxDiskError
{
    public string Message { get; init; }
    
    public string Description { get; init; }
    
    public string Error { get; init; }
}