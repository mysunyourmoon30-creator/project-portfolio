using Innovation.Hardware;
using Innovation.Services.Errors;
using Innovation.TotalWeight_PLC.Interfaces.Presenters;
using Innovation.TotalWeight_PLC.Interfaces.Views;
using Innovation.TotalWeight_PLC.Service;
using Innovation.TotalWeight_PLC.ViewModel;

namespace Innovation.TotalWeight_PLC.Presenter.Implementations;

// The original's NotFound() helper did two things at once: show a message
// AND close the dialog (Frontend ROADMAP §5b.2), which is why
// RUNTIME_TEST_CHECKLIST.md §H/§I/§J exist as a list of bugs. Every failure
// branch below calls View.ShowMessage but explicitly does NOT call
// View.CloseDialog - only the single success path at the bottom does.
public sealed class Presenter_ShowAutoFeed : IPresenter_ShowAutoFeed
{
    private readonly IApiClient _api;
    private readonly IPlcDevice _plc;

    public IView_ShowAutoFeed View { get; }

    public Presenter_ShowAutoFeed(IView_ShowAutoFeed view, IApiClient api, IPlcDevice plc)
    {
        View = view;
        _api = api;
        _plc = plc;
    }

    public async Task RunAsync(AutoFeedRequest request)
    {
        try
        {
            await _api.GetRmBalAsync(request.Barcode);
        }
        catch (RmBalNotFoundException)
        {
            View.ShowMessage(Resources.Strings.RmBalNotFound(request.Barcode), AppMessageType.Warning);
            return; // form must stay open
        }

        Innovation.Services.Contracts.FeeddoorStepDto feeddoorStep;
        try
        {
            feeddoorStep = await _api.GetFeeddoorStepAsync(request.LineId);
        }
        catch (SettingNotFoundException)
        {
            View.ShowMessage(Resources.Strings.FeeddoorStepNotConfigured, AppMessageType.Warning);
            return; // door write (and the rest of auto-feed) skipped - form must stay open
        }

        // A missing PRODSTD_MIXTEMP row is NOT an error (README §8, Phase 3
        // scenario 6) - weighing continues silently, no warning, no close.
        _ = await _api.GetMixTempAsync(request.PlanId);

        try
        {
            await _api.WithdrawRmBalAsync(request.Barcode, amount: 1m);
            await _plc.OpenAsync();
            await _plc.WriteDeviceAsync(feeddoorStep.PlcAddress, 1);
        }
        catch (Exception ex) when (ex is not RmBalNotFoundException and not SettingNotFoundException)
        {
            View.ShowMessage(Resources.Strings.AutoFeedDbWriteFailed, AppMessageType.Warning);
            return; // form must stay open
        }

        View.ShowMessage(Resources.Strings.AutoFeedSuccess, AppMessageType.Information);
        View.CloseDialog(DialogResult.OK); // the ONLY path that closes the form
    }
}
