using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Xunit;

namespace Innovation.Api.Tests;

public class AutoFeedControllerTests
{
    [Fact]
    public async Task GetRmBal_UnknownBarcode_Returns404()
    {
        using var factory = new CustomWebApplicationFactory();
        var client = await factory.CreateClient().WithOperatorLoginAsync();

        var response = await client.GetAsync("/api/rm-bal/DOES-NOT-EXIST");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        (await response.Content.ReadAsStringAsync()).Should().Contain("rm-bal-not-found");
    }

    [Fact]
    public async Task Withdraw_MoreThanBalance_StillSucceeds_BalanceCanGoNegative()
    {
        // Matches the real system: RM_BAL withdrawal doesn't block on
        // insufficient balance at this layer (that's an operator-facing
        // warning in the UI, not a hard API constraint - see README §4.2).
        using var factory = new CustomWebApplicationFactory();
        var client = await factory.CreateClient().WithOperatorLoginAsync();

        var response = await client.PostAsJsonAsync("/api/rm-bal/RM001/withdraw", new { amount = 999999m });

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task GetFeeddoorStep_ConfiguredLine_ReturnsStep()
    {
        using var factory = new CustomWebApplicationFactory();
        var client = await factory.CreateClient().WithOperatorLoginAsync();

        var response = await client.GetAsync("/api/lines/1/feeddoor-step");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetFeeddoorStep_UnconfiguredLine_Returns404()
    {
        using var factory = new CustomWebApplicationFactory();
        var client = await factory.CreateClient().WithOperatorLoginAsync();

        var response = await client.GetAsync("/api/lines/999/feeddoor-step");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        (await response.Content.ReadAsStringAsync()).Should().Contain("setting-not-found");
    }

    [Fact]
    public async Task GetMixTemp_MissingPlan_ReturnsSuccess_NotAnError()
    {
        // ASP.NET Core's default HttpNoContentOutputFormatter turns a null
        // Ok(...) body into 204 - either way, this must NOT be an error
        // response (Backend ROADMAP §8, Phase 3 scenario 6: no MixTemp row
        // means "continue weighing normally", not a warning).
        using var factory = new CustomWebApplicationFactory();
        var client = await factory.CreateClient().WithOperatorLoginAsync();

        var response = await client.GetAsync("/api/plans/999/mix-temp");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}
