using System.Net.Http.Json;
using FluentAssertions;
using Innovation.Api.Tests;
using Innovation.TotalWeight_PLC.Interfaces.Views;
using Innovation.TotalWeight_PLC.Presenter.Implementations;
using Innovation.TotalWeight_PLC.Service;
using Innovation.TotalWeight_PLC.ViewModel;
using NSubstitute;
using Xunit;

namespace Innovation.E2E.Tests;

// Full stack: real Desktop Presenter_TotalWeight -> real ApiClient ->
// Kestrel TestServer (real Innovation.Api pipeline: auth, controllers,
// EF Core) -> a real (temporary) SQLite database. Only the WinForms view
// itself is a test double, since driving actual window messages headlessly
// is impractical - Phase 4's manual walkthrough covers that gap (see
// docs/DEMO_SCRIPT.md).
public class HappyPathWeighingE2ETests
{
    private static async Task<IApiClient> BuildAuthenticatedApiClientAsync(CustomWebApplicationFactory factory)
    {
        var httpClient = factory.CreateClient();
        var loginResponse = await httpClient.PostAsJsonAsync("api/auth/login", new { username = "operator1", password = "Password123!" });
        loginResponse.EnsureSuccessStatusCode();
        var body = await loginResponse.Content.ReadFromJsonAsync<LoginResponseBody>();
        httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", body!.Token);
        return new ApiClient(httpClient);
    }

    private sealed record LoginResponseBody(string Token, string Username, string FullName);

    [Fact]
    public async Task FullFlow_ScanBarcode_WeighStep_Accept_Save_PersistsToRealDatabase()
    {
        using var factory = new CustomWebApplicationFactory();
        var api = await BuildAuthenticatedApiClientAsync(factory);

        var view = Substitute.For<IView_TotalWeight>();
        view.Steps.Returns(new System.ComponentModel.BindingList<StepRowViewModel>());
        var asyncRunner = Substitute.For<Innovation.Mvp.Core.Async.IAsyncOperationRunner>();
        asyncRunner.RunAsync(Arg.Any<string>(), Arg.Any<Func<Task>>(), Arg.Any<Action?>())
            .Returns(ci => ci.Arg<Func<Task>>()());

        var presenter = new Presenter_TotalWeight(view, api, asyncRunner);

        await presenter.LoadKanbanAsync("KB0000001");
        view.KbTogetherId.Should().Be(1);
        var step = view.Steps.Single();

        await presenter.SubmitStepWeightAsync(step.StepNo, 10.05m);
        await presenter.SaveAsync();
        await presenter.AcceptStepAsync(step.StepNo);

        // Verify against a fresh client hitting the same real database -
        // proves the save actually persisted, not just an in-memory echo.
        var verifyResponse = await api.GetRmBalAsync("RM001");
        verifyResponse.Balance.Should().Be(500.00m - 10.05m);
        view.Steps.Single().Accepted.Should().BeTrue();
    }
}
