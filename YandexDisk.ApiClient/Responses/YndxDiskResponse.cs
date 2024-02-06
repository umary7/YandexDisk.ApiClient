namespace YandexDisk.ApiClient.Responses;

public class YndxDiskResponse<T>
{
    public bool Success { get; set; }
    public YndxDiskError? Error { get; set; }
    public T? Result { get; set; }
}
 
public class YndxDiskResponse
{
    public bool Success { get; set; }
    public YndxDiskError? Error { get; set; }
}