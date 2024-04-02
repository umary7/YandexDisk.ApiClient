using System.Text.Json;
using Humanizer;

namespace YandexDisk.ApiClient.Formatting;

public class SnakeCaseNamingPolicy : JsonNamingPolicy
{
    public override string ConvertName(string name) => name.Underscore();
}