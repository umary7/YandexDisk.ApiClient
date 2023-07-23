using System.Text.Json;
using Humanizer;

namespace YandexDisk.ApiClient.Extensions;

public class SnakeCaseNamingPolicy : JsonNamingPolicy
{
    public override string ConvertName(string name) => name.Underscore();
}