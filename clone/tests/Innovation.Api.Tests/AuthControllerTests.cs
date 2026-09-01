using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Innovation.Api.Controllers;
using Xunit;

namespace Innovation.Api.Tests;

public class AuthControllerTests
{
    [Fact]
    public async Task Login_WithValidCredentials_ReturnsJwt()
    {
        using var factory = new CustomWebApplicationFactory();
        var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/auth/login", new { username = "operator1", password = "Password123!" });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<LoginResponse>();
        body!.Token.Should().NotBeNullOrWhiteSpace();
        body.Username.Should().Be("operator1");
    }

    [Fact]
    public async Task Login_WithWrongPassword_Returns401ProblemDetails()
    {
        using var factory = new CustomWebApplicationFactory();
        var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/auth/login", new { username = "operator1", password = "wrong" });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        var body = await response.Content.ReadAsStringAsync();
        body.Should().Contain("invalid-credentials");
    }
}
