using System.Net.Http.Headers;
using System.Net.Http.Json;
using Innovation.Api.Controllers;

namespace Innovation.Api.Tests;

public static class AuthenticatedClientExtensions
{
    public static async Task<HttpClient> WithOperatorLoginAsync(this HttpClient client)
    {
        var response = await client.PostAsJsonAsync("/api/auth/login", new { username = "operator1", password = "Password123!" });
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadFromJsonAsync<LoginResponse>();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", body!.Token);
        return client;
    }
}
