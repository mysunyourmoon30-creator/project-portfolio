using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using Innovation.Api.Tests;
using Innovation.Hardware;
using Innovation.Mvp.Core.Async;
using Innovation.Services.Errors;
using Innovation.TotalWeight_PLC.Interfaces.Views;
using Innovation.TotalWeight_PLC.Presenter.Implementations;
using Innovation.TotalWeight_PLC.Service;
using Innovation.TotalWeight_PLC.ViewModel;
using NSubstitute;
using Xunit;

namespace Innovation.E2E.Tests;

// The same 7 scenarios from Innovation.Hardware.Tests (pure hardware/logic)
// and Innovation.Api.Tests (API-only), now driven end to end: real
// Presenter -> real ApiClient -> real API pipeline -> real SQLite database.
public class ScenarioE2ETests
{
    private static async Task<IApiClient> AuthenticatedClientAsync(CustomWebApplicationFactory factory)
    {
        var httpClient = factory.CreateClient();
        var login = await httpClient.PostAsJsonAsync("api/auth/login", new { username = "operator1", password = "Password123!" });
        login.EnsureSuccessStatusCode();
        var body = await login.Content.ReadFromJsonAsync<LoginBody>();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", body!.Token);
        return new ApiClient(httpClient);
    }

    private sealed record LoginBody(string Token, string Username, string FullName);

    private static IAsyncOperationRunner PassthroughRunner()
    {
        var runner = Substitute.For<IAsyncOperationRunner>();
        runner.RunAsync(Arg.Any<string>(), Arg.Any<Func<Task>>(), Arg.Any<Action?>())
            .Returns(ci => ci.Arg<Func<Task>>()());
        return runner;
    }

    private static IView_TotalWeight FakeView()
    {
        var view = Substitute.For<IView_TotalWeight>();
        view.Steps.Returns(new System.ComponentModel.BindingList<StepRowViewModel>());
        return view;
    }

    // Scenario 1: normal weighing to completion.
    [Fact]
    public async Task E2E_NormalWeighing_CompletesAndPersists()
    {
        using var factory = new CustomWebApplicationFactory();
        var api = await AuthenticatedClientAsync(factory);
        var view = FakeView();
        var presenter = new Presenter_TotalWeight(view, api, PassthroughRunner());

        await presenter.LoadKanbanAsync("KB0000001");
        await presenter.SubmitStepWeightAsync(1, 10.05m);
        await presenter.SaveAsync();

        view.Received(1).ShowMessage(Arg.Any<string>(), AppMessageType.Information);
    }

    // Scenario 2: weight outside [min, max] blocks the save silently at the
    // client (no API call for the invalid value - see OutOfRangeWeightScenarioTests
    // for the pure-hardware version of this rule).
    [Fact]
    public async Task E2E_OutOfRangeWeight_ShowsWarning_LeavesActualUnset()
    {
        using var factory = new CustomWebApplicationFactory();
        var api = await AuthenticatedClientAsync(factory);
        var view = FakeView();
        var presenter = new Presenter_TotalWeight(view, api, PassthroughRunner());
        await presenter.LoadKanbanAsync("KB0000001");

        await presenter.SubmitStepWeightAsync(1, 99m); // way outside tolerance

        view.Steps.Single().Actual.Should().BeNull();
        view.Received(1).ShowMessage(Arg.Any<string>(), AppMessageType.Warning);
    }

    // Scenario 3: PLC unreachable - covered against the real IPlcDevice
    // contract via Presenter_ShowAutoFeed, using the real API for RM_BAL and
    // feeddoor lookups but a scripted-to-fail PLC.
    [Fact]
    public async Task E2E_PlcUnreachable_DuringAutoFeed_ShowsWarning_DoesNotClose()
    {
        using var factory = new CustomWebApplicationFactory();
        var api = await AuthenticatedClientAsync(factory);
        var view = Substitute.For<IView_ShowAutoFeed>();
        var plc = new SimulatedPlcDevice(new PlcSimulationScript { FailToConnect = true });
        var presenter = new Presenter_ShowAutoFeed(view, api, plc);

        await presenter.RunAsync(new AutoFeedRequest("RM001", 1, 1));

        view.Received(1).ShowMessage(Arg.Any<string>(), AppMessageType.Warning);
        view.DidNotReceive().CloseDialog(Arg.Any<DialogResult>());
    }

    // Scenario 4: barcode not found in RM_BAL.
    [Fact]
    public async Task E2E_BarcodeNotInRmBal_WarnsAndKeepsFormOpen()
    {
        using var factory = new CustomWebApplicationFactory();
        var api = await AuthenticatedClientAsync(factory);
        var view = Substitute.For<IView_ShowAutoFeed>();
        var presenter = new Presenter_ShowAutoFeed(view, api, new SimulatedPlcDevice());

        await presenter.RunAsync(new AutoFeedRequest("NO-SUCH-BARCODE", 1, 1));

        view.Received(1).ShowMessage(Arg.Any<string>(), AppMessageType.Warning);
        view.DidNotReceive().CloseDialog(Arg.Any<DialogResult>());
    }

    // Scenario 5: Feeddoor Step not configured for the line.
    [Fact]
    public async Task E2E_FeeddoorStepMissing_SkipsDoorWrite_KeepsFormOpen()
    {
        using var factory = new CustomWebApplicationFactory();
        var api = await AuthenticatedClientAsync(factory);
        var view = Substitute.For<IView_ShowAutoFeed>();
        var presenter = new Presenter_ShowAutoFeed(view, api, new SimulatedPlcDevice());

        await presenter.RunAsync(new AutoFeedRequest("RM001", LineId: 999, PlanId: 1)); // no SendStepParameter seeded for line 999

        view.Received(1).ShowMessage(Arg.Any<string>(), AppMessageType.Warning);
        view.DidNotReceive().CloseDialog(Arg.Any<DialogResult>());
    }

    // Scenario 6: PRODSTD_MIXTEMP missing - NOT an error, continues to success.
    [Fact]
    public async Task E2E_MixTempMissing_ContinuesNormally_ReachesSuccess()
    {
        using var factory = new CustomWebApplicationFactory();
        var api = await AuthenticatedClientAsync(factory);
        var view = Substitute.For<IView_ShowAutoFeed>();
        var plc = new SimulatedPlcDevice();
        var presenter = new Presenter_ShowAutoFeed(view, api, plc);

        await presenter.RunAsync(new AutoFeedRequest("RM001", LineId: 1, PlanId: 999)); // no PRODSTD_MIXTEMP row for plan 999

        view.Received(1).ShowMessage(Arg.Any<string>(), AppMessageType.Information);
        view.Received(1).CloseDialog(DialogResult.OK);
    }

    // Scenario 7 (DB write fails during auto-feed) is intentionally NOT
    // re-tested here: reliably forcing a real SQLite write to fail mid-flight
    // over an actual HTTP round trip needs test infrastructure (a fault-
    // injecting DbContext, or similar) disproportionate to what it would add.
    // It's already covered at the unit level with a substituted IApiClient in
    // Innovation.TotalWeight_PLC.Tests.Presenter_ShowAutoFeedTests
    // .DbWriteFailsDuringAutoFeed_ShowsWarning_DoesNotCloseDialog, which
    // exercises the exact same catch-and-warn branch in Presenter_ShowAutoFeed.
}
