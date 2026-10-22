using System.Net;
using System.Net.Http.Json;

namespace lab1.Pages;

public static class HttpClientExtensions
{
    // GET-запит, який повертає null замість винятку, якщо API відповів 404.
    // Усі інші помилки (500, 401 тощо) як і раніше кидають виняток.
    public static async Task<T?> GetOrNullAsync<T>(this HttpClient client, string url) where T : class
    {
        var response = await client.GetAsync(url);
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<T>();
    }
}
