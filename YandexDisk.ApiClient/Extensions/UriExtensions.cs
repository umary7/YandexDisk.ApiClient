using System.Web;

namespace YandexDisk.ApiClient.Extensions;

public static class UriExtensions
{
    public static Uri AddParameters(this Uri uri, params (string Name, string Value)[] parameters)
    {
        if (!parameters.Any()) return uri;

        var qs = string.Empty;
        var queryCollection = HttpUtility.ParseQueryString(qs);
        foreach (var parameter in parameters)
        {
            queryCollection[parameter.Name] = parameter.Value;
        }

        return queryCollection.Count == 0
            ? new Uri(uri.ToString())
            : new Uri($"{uri}?{queryCollection}", UriKind.Relative);
    }
}