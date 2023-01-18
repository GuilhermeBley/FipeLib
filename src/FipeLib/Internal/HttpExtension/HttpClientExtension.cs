namespace FipeLib.Internal.HttpExtension;

internal static class HttpClientExtension
{
    public static async Task<HttpResponseMessage> PostUrlEncondedAsync(this HttpClient client, string url, params KeyValuePair<string, string>[] xxx_url_urlencoded)
    {
        using var requestMessage = new HttpRequestMessage(HttpMethod.Post, url) { Content = new FormUrlEncodedContent(xxx_url_urlencoded) };
        return await client.SendAsync(requestMessage);
    }

    public static async Task<HttpResponseMessage> PostAsync(this HttpClient client, string url, Dictionary<string, string> xxx_url_urlencoded)
    {
        return await client.PostUrlEncondedAsync(url, ((IEnumerable<KeyValuePair<string, string>>)xxx_url_urlencoded).ToArray());
    }

    public static async Task<HttpResponseMessage> PostUrlEncondedAsync(this HttpClient client, string url, params (string key, string value)[] xxx_url_urlencoded)
    {
        KeyValuePair<string, string>[] array = 
            xxx_url_urlencoded.Select(tuple => new KeyValuePair<string, string>(tuple.key, tuple.value)).ToArray();
        return await client.PostUrlEncondedAsync(url, array);
    }
}