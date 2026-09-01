using System.ComponentModel;
using FluentAssertions;
using Innovation.Mvp.Core.Async;
using Microsoft.Extensions.DependencyInjection;
using Innovation.Services.Contracts;
using Innovation.Services.Errors;
using Innovation.TotalWeight_PLC.Interfaces.Views;
using Innovation.TotalWeight_PLC.Presenter.Implementations;
using Innovation.TotalWeight_PLC.Service;
using Innovation.TotalWeight_PLC.ViewModel;
using NSubstitute;
using Xunit;

namespace Innovation.TotalWeight_PLC.Tests;

public class Presenter_TotalWeightTests
{
    private static IView_TotalWeight FakeView()
    {
        var view = Substitute.For<IView_TotalWeight>();
        view.Steps.Returns(new BindingList<StepRowViewModel>());
        return view;
    }

    // Tests call presenter methods directly rather than raising view events,
    // so the async runner is never actually invoked - a substitute is all
    // that's needed to satisfy the constructor.
    private static IAsyncOperationRunner FakeRunner() => Substitute.For<IAsyncOperationRunner>();

    // Auto-feed is orchestrated through a DI scope (RunAutoFeedAsync), not
    // exercised by these tests - Presenter_ShowAutoFeedTests covers that
    // logic directly. A substitute is enough to satisfy the constructor.
    private static IServiceScopeFactory FakeScopeFactory() => Substitute.For<IServiceScopeFactory>();

    [Fact]
    public async Task LoadKanban_ValidBarcode_PopulatesStepsOnView()
    {
        var view = FakeView();
        var api = Substitute.For<IApiClient>();
        api.GetKanbanAsync("KB0000001").Returns(new KanbanDetailDto(1, "KB0000001", 1, 1, "Pending",
            new List<KanbanStepDto> { new(1, "RM001", 10m, 9.5m, 10.5m, null, false) }));
        var presenter = new Presenter_TotalWeight(view, api, FakeRunner(), FakeScopeFactory());

        await presenter.LoadKanbanAsync("KB0000001");

        view.Steps.Should().ContainSingle(s => s.StepNo == 1 && s.Target == 10m);
        view.KbTogetherId.Should().Be(1);
    }

    [Fact]
    public async Task LoadKanban_UnknownBarcode_ShowsWarning()
    {
        var view = FakeView();
        var api = Substitute.For<IApiClient>();
        api.GetKanbanAsync("BAD").Returns<KanbanDetailDto>(_ => throw new BarcodeNotFoundException("BAD"));
        var presenter = new Presenter_TotalWeight(view, api, FakeRunner(), FakeScopeFactory());

        await presenter.LoadKanbanAsync("BAD");

        view.Received(1).ShowMessage(Arg.Any<string>(), AppMessageType.Warning);
    }

    [Fact]
    public async Task SubmitStepWeight_WithinTolerance_UpdatesActualOnRow()
    {
        var view = FakeView();
        view.Steps.Add(new StepRowViewModel { StepNo = 1, Target = 10m, Min = 9.5m, Max = 10.5m });
        var presenter = new Presenter_TotalWeight(view, Substitute.For<IApiClient>(), FakeRunner(), FakeScopeFactory());

        await presenter.SubmitStepWeightAsync(1, 10.05m);

        view.Steps.Single().Actual.Should().Be(10.05m);
    }

    [Fact]
    public async Task SubmitStepWeight_OutsideTolerance_ShowsWarning_DoesNotSetActual()
    {
        var view = FakeView();
        view.Steps.Add(new StepRowViewModel { StepNo = 1, Target = 10m, Min = 9.5m, Max = 10.5m });
        var presenter = new Presenter_TotalWeight(view, Substitute.For<IApiClient>(), FakeRunner(), FakeScopeFactory());

        await presenter.SubmitStepWeightAsync(1, 11.2m);

        view.Steps.Single().Actual.Should().BeNull();
        view.Received(1).ShowMessage(Arg.Any<string>(), AppMessageType.Warning);
    }

    // Reproduces a real bug found by running the app: WinForms' DataGridView
    // commits an edited cell into its bound object BEFORE raising
    // CellEndEdit, so by the time frmTotalWeight's handler reads row.Actual
    // and calls this method, the model ALREADY holds the rejected value -
    // unlike the test above, where row.Actual starts null and the candidate
    // weight arrives purely as a parameter. This test starts row.Actual
    // already set to the out-of-range value to match what really happens.
    [Fact]
    public async Task SubmitStepWeight_OutsideTolerance_RevertsActualThatGridAlreadyCommitted()
    {
        var view = FakeView();
        view.Steps.Add(new StepRowViewModel { StepNo = 1, Target = 10m, Min = 9.5m, Max = 10.5m, Actual = 12.00m });
        var presenter = new Presenter_TotalWeight(view, Substitute.For<IApiClient>(), FakeRunner(), FakeScopeFactory());

        await presenter.SubmitStepWeightAsync(1, 12.00m);

        view.Steps.Single().Actual.Should().BeNull();
    }

    [Fact]
    public async Task Save_HappyPath_CallsApiThenShowsConfirmation()
    {
        var view = FakeView();
        view.Steps.Add(new StepRowViewModel { StepNo = 1, Target = 10m, Min = 9.5m, Max = 10.5m, Actual = 10m });
        var api = Substitute.For<IApiClient>();
        var presenter = new Presenter_TotalWeight(view, api, FakeRunner(), FakeScopeFactory());

        await presenter.SaveAsync();

        await api.Received(1).SaveTotalWeightAsync(
            Arg.Is<SaveTotalWeightRequestDto>(r => r.Steps.Count == 1 && r.Steps[0].ActualWeight == 10m),
            Arg.Any<CancellationToken>());
        view.Received(1).ShowMessage(Arg.Any<string>(), AppMessageType.Information);
    }

    [Fact]
    public async Task Save_ApiReturnsConflict_ShowsWarning_DoesNotThrow()
    {
        var view = FakeView();
        view.Steps.Add(new StepRowViewModel { StepNo = 1, Target = 10m, Min = 9.5m, Max = 10.5m, Actual = 10m });
        var api = Substitute.For<IApiClient>();
        api.SaveTotalWeightAsync(Arg.Any<SaveTotalWeightRequestDto>(), Arg.Any<CancellationToken>())
            .Returns<SaveTotalWeightResultDto>(_ => throw new TotalWeightAlreadyExistsException(1));
        var presenter = new Presenter_TotalWeight(view, api, FakeRunner(), FakeScopeFactory());

        var act = () => presenter.SaveAsync();

        await act.Should().NotThrowAsync();
        view.Received(1).ShowMessage(Arg.Any<string>(), AppMessageType.Warning);
    }

    [Fact]
    public async Task Accept_WithoutSubmittingCurrentWeight_ShowsWarning()
    {
        var view = FakeView();
        view.Steps.Add(new StepRowViewModel { StepNo = 1, Target = 10m, Min = 9.5m, Max = 10.5m });
        var api = Substitute.For<IApiClient>();
        api.AcceptAsync(Arg.Any<AcceptStepRequestDto>(), Arg.Any<CancellationToken>())
            .Returns(_ => throw new StepNotAcceptedException(1));
        var presenter = new Presenter_TotalWeight(view, api, FakeRunner(), FakeScopeFactory());

        await presenter.AcceptStepAsync(1);

        view.Steps.Single().Accepted.Should().BeFalse();
        view.Received(1).ShowMessage(Arg.Any<string>(), AppMessageType.Warning);
    }
}
