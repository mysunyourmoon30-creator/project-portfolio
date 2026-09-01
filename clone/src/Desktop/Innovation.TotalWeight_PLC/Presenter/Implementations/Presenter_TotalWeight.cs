using Innovation.Mvp.Core.Async;
using Innovation.Services.Contracts;
using Innovation.Services.Errors;
using Innovation.TotalWeight_PLC.Interfaces.Presenters;
using Innovation.TotalWeight_PLC.Interfaces.Views;
using Innovation.TotalWeight_PLC.Service;
using Innovation.TotalWeight_PLC.ViewModel;

namespace Innovation.TotalWeight_PLC.Presenter.Implementations;

public sealed class Presenter_TotalWeight : IPresenter_TotalWeight
{
    private readonly IApiClient _api;
    private readonly IAsyncOperationRunner _asyncRunner;

    public IView_TotalWeight View { get; }

    // Constructor injection only - no `View.Presenter = this` back-wiring
    // (the original's reference-cycle bug, README §8.4 Phase 0 fix #1). The
    // view exposes events instead of taking this presenter as a dependency,
    // so the relationship stays one-directional (presenter -> view) - see
    // IView_TotalWeight's event members. Each event handler is wrapped in
    // IAsyncOperationRunner here, replacing the original's
    // BaseForm.RunSafeAsync being called from inside the view/code-behind.
    public Presenter_TotalWeight(IView_TotalWeight view, IApiClient api, IAsyncOperationRunner asyncRunner)
    {
        View = view;
        _api = api;
        _asyncRunner = asyncRunner;

        View.BarcodeScanned += (_, barcode) =>
            _ = _asyncRunner.RunAsync(nameof(View.BarcodeScanned), () => LoadKanbanAsync(barcode));
        View.StepWeightEntered += (_, e) =>
            _ = _asyncRunner.RunAsync(nameof(View.StepWeightEntered), () => SubmitStepWeightAsync(e.StepNo, e.Weight));
        View.SaveRequested += (_, _) =>
            _ = _asyncRunner.RunAsync(nameof(View.SaveRequested), SaveAsync);
        View.AcceptRequested += (_, stepNo) =>
            _ = _asyncRunner.RunAsync(nameof(View.AcceptRequested), () => AcceptStepAsync(stepNo));
    }

    public void Run() => View.Run();

    public async Task LoadKanbanAsync(string barcode)
    {
        KanbanDetailDto kanban;
        try
        {
            kanban = await _api.GetKanbanAsync(barcode);
        }
        catch (BarcodeNotFoundException)
        {
            View.ShowMessage(Resources.Strings.BarcodeNotFound(barcode), AppMessageType.Warning);
            return;
        }

        View.KbTogetherId = kanban.KbTogetherId;
        View.Barcode = kanban.Barcode;
        View.Steps.Clear();
        foreach (var step in kanban.Steps)
        {
            View.Steps.Add(new StepRowViewModel
            {
                StepNo = step.StepNo,
                RawMaterialCode = step.RawMaterialCode,
                Target = step.Target,
                Min = step.Min,
                Max = step.Max,
                Actual = step.Actual,
                Accepted = step.Accepted,
            });
        }
    }

    public Task SubmitStepWeightAsync(int stepNo, decimal actualWeight)
    {
        var row = View.Steps.FirstOrDefault(s => s.StepNo == stepNo);
        if (row is null)
        {
            return Task.CompletedTask;
        }

        // Weight tolerance was already computed server-side (WeightToleranceCalculator)
        // and returned on the step DTO - re-validated here so the operator gets
        // instant feedback without a round-trip.
        if (actualWeight < row.Min || actualWeight > row.Max)
        {
            View.ShowMessage(Resources.Strings.WeightOutOfRange(row.Min, row.Max), AppMessageType.Warning);
            return Task.CompletedTask;
        }

        row.Actual = actualWeight;
        return Task.CompletedTask;
    }

    public async Task SaveAsync()
    {
        var steps = View.Steps
            .Where(s => s.Actual.HasValue)
            .Select(s => new StepWeightDto(s.StepNo, s.Actual!.Value))
            .ToList();

        if (steps.Count == 0)
        {
            View.ShowMessage(Resources.Strings.NoStepsWeighed, AppMessageType.Warning);
            return;
        }

        try
        {
            await _api.SaveTotalWeightAsync(new SaveTotalWeightRequestDto(View.KbTogetherId, steps));
            View.ShowMessage(Resources.Strings.SaveSuccess, AppMessageType.Information);
        }
        catch (TotalWeightAlreadyExistsException)
        {
            View.ShowMessage(Resources.Strings.TotalWeightAlreadyExists, AppMessageType.Warning);
        }
    }

    public async Task AcceptStepAsync(int stepNo)
    {
        try
        {
            await _api.AcceptAsync(new AcceptStepRequestDto(View.KbTogetherId, stepNo));
            var row = View.Steps.FirstOrDefault(s => s.StepNo == stepNo);
            if (row is not null)
            {
                row.Accepted = true;
            }
        }
        catch (StepNotAcceptedException)
        {
            View.ShowMessage(Resources.Strings.StepNotAccepted, AppMessageType.Warning);
        }
    }
}
