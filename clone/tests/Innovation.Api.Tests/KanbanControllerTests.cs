using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Innovation.Services.Contracts;
using Xunit;

namespace Innovation.Api.Tests;

public class KanbanControllerTests
{
    [Fact]
    public async Task GetKanban_ExistingBarcode_ReturnsPlanData()
    {
        using var factory = new CustomWebApplicationFactory();
        var client = await factory.CreateClient().WithOperatorLoginAsync();

        var response = await client.GetAsync("/api/kanban/KB0000001");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<KanbanDetailDto>();
        body!.Barcode.Should().Be("KB0000001");
        body.Steps.Should().ContainSingle(s => s.StepNo == 1 && s.Target == 10.00m);
    }

    [Fact]
    public async Task GetPendingKanbans_ReturnsSeededDemoKanban()
    {
        using var factory = new CustomWebApplicationFactory();
        var client = await factory.CreateClient().WithOperatorLoginAsync();

        var response = await client.GetAsync("/api/kanbans");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<List<KanbanSummaryDto>>();
        body.Should().ContainSingle(k => k.Barcode == "KB0000001" && k.Status == "Pending");
    }

    [Fact]
    public async Task GetKanban_UnknownBarcode_Returns404WithBarcodeNotFoundType()
    {
        using var factory = new CustomWebApplicationFactory();
        var client = await factory.CreateClient().WithOperatorLoginAsync();

        var response = await client.GetAsync("/api/kanban/DOES-NOT-EXIST");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        (await response.Content.ReadAsStringAsync()).Should().Contain("barcode-not-found");
    }

    [Fact]
    public async Task SaveTotalWeight_HappyPath_PersistsAndWithdrawsRmBal_InOneTransaction()
    {
        using var factory = new CustomWebApplicationFactory();
        var client = await factory.CreateClient().WithOperatorLoginAsync();
        var kanban = await (await client.GetAsync("/api/kanban/KB0000001")).Content.ReadFromJsonAsync<KanbanDetailDto>();

        var saveResponse = await client.PostAsJsonAsync("/api/totalweight", new SaveTotalWeightRequestDto(
            kanban!.KbTogetherId,
            new List<StepWeightDto> { new(1, 10.05m) }));

        saveResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var saved = await saveResponse.Content.ReadFromJsonAsync<SaveTotalWeightResultDto>();
        saved!.TotalActualWeight.Should().Be(10.05m);

        var rmBalResponse = await client.GetAsync("/api/rm-bal/RM001");
        var rmBal = await rmBalResponse.Content.ReadFromJsonAsync<RmBalDto>();
        rmBal!.Balance.Should().Be(500.00m - 10.05m); // withdrawn in the same SaveTotalWeight transaction
    }

    [Fact]
    public async Task SaveTotalWeight_CalledTwiceForSameKanban_ReturnsConflict()
    {
        using var factory = new CustomWebApplicationFactory();
        var client = await factory.CreateClient().WithOperatorLoginAsync();
        var kanban = await (await client.GetAsync("/api/kanban/KB0000001")).Content.ReadFromJsonAsync<KanbanDetailDto>();
        var request = new SaveTotalWeightRequestDto(kanban!.KbTogetherId, new List<StepWeightDto> { new(1, 10.00m) });

        var first = await client.PostAsJsonAsync("/api/totalweight", request);
        var second = await client.PostAsJsonAsync("/api/totalweight", request);

        first.StatusCode.Should().Be(HttpStatusCode.OK);
        second.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task Accept_WithoutSubmittingCurrentStepWeight_Returns409()
    {
        using var factory = new CustomWebApplicationFactory();
        var client = await factory.CreateClient().WithOperatorLoginAsync();
        var kanban = await (await client.GetAsync("/api/kanban/KB0000001")).Content.ReadFromJsonAsync<KanbanDetailDto>();

        var response = await client.PostAsJsonAsync("/api/totalweight/accept", new AcceptStepRequestDto(kanban!.KbTogetherId, 1));

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
        (await response.Content.ReadAsStringAsync()).Should().Contain("step-not-accepted");
    }

    [Fact]
    public async Task Accept_AfterSubmittingWeight_Succeeds()
    {
        using var factory = new CustomWebApplicationFactory();
        var client = await factory.CreateClient().WithOperatorLoginAsync();
        var kanban = await (await client.GetAsync("/api/kanban/KB0000001")).Content.ReadFromJsonAsync<KanbanDetailDto>();

        await client.PostAsJsonAsync("/api/totalweight", new SaveTotalWeightRequestDto(
            kanban!.KbTogetherId, new List<StepWeightDto> { new(1, 10.00m) }));
        var response = await client.PostAsJsonAsync("/api/totalweight/accept", new AcceptStepRequestDto(kanban.KbTogetherId, 1));

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}
