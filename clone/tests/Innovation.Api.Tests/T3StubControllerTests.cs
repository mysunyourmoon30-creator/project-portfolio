using System.Net;
using FluentAssertions;
using Xunit;

namespace Innovation.Api.Tests;

public class T3StubControllerTests
{
    public static IEnumerable<object[]> T3Routes => new List<object[]>
    {
        new object[] { HttpMethod.Get, "/api/trays" },
        new object[] { HttpMethod.Post, "/api/trays" },
        new object[] { HttpMethod.Post, "/api/manual-mode/enable" },
        new object[] { HttpMethod.Post, "/api/manual-mode/cancel-kanban" },
        new object[] { HttpMethod.Post, "/api/cleaning/start" },
    };

    [Theory]
    [MemberData(nameof(T3Routes))]
    public async Task AnyT3Endpoint_Returns501WithNotImplementedProblemType(HttpMethod method, string route)
    {
        using var factory = new CustomWebApplicationFactory();
        var client = await factory.CreateClient().WithOperatorLoginAsync();

        var response = await client.SendAsync(new HttpRequestMessage(method, route));

        response.StatusCode.Should().Be(HttpStatusCode.NotImplemented);
        (await response.Content.ReadAsStringAsync()).Should().Contain("not-implemented");
    }
}
