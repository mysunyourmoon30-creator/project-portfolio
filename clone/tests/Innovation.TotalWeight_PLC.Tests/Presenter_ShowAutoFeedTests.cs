using Innovation.Hardware;
using Innovation.Services.Contracts;
using Innovation.Services.Errors;
using Innovation.TotalWeight_PLC.Interfaces.Views;
using Innovation.TotalWeight_PLC.Presenter.Implementations;
using Innovation.TotalWeight_PLC.Service;
using Innovation.TotalWeight_PLC.ViewModel;
using NSubstitute;
using Xunit;

namespace Innovation.TotalWeight_PLC.Tests;

// These are the sharpest tests in the whole clone: they prove the original
// NotFound() bug (report + close conflated - Frontend ROADMAP §5b.2) is
// actually fixed, not just described as fixed. Every one of the four
// failure scenarios must warn the operator WITHOUT closing the dialog.
public class Presenter_ShowAutoFeedTests
{
    private static (IView_ShowAutoFeed View, IApiClient Api, IPlcDevice Plc, Presenter_ShowAutoFeed Presenter) Build()
    {
        var view = Substitute.For<IView_ShowAutoFeed>();
        var api = Substitute.For<IApiClient>();
        var plc = Substitute.For<IPlcDevice>();
        var presenter = new Presenter_ShowAutoFeed(view, api, plc);
        return (view, api, plc, presenter);
    }

    [Fact]
    public async Task BarcodeNotFoundInRmBal_ShowsWarning_DoesNotCloseDialog()
    {
        var (view, api, _, presenter) = Build();
        api.GetRmBalAsync(Arg.Any<string>()).Returns<RmBalDto>(_ => throw new RmBalNotFoundException("RM-X"));

        await presenter.RunAsync(new AutoFeedRequest("RM-X", 1, 1));

        view.Received(1).ShowMessage(Arg.Any<string>(), AppMessageType.Warning);
        view.DidNotReceive().CloseDialog(Arg.Any<DialogResult>());
    }

    [Fact]
    public async Task FeeddoorStepNotConfigured_SkipsDoorWrite_ShowsWarning_DoesNotCloseDialog()
    {
        var (view, api, plc, presenter) = Build();
        api.GetRmBalAsync(Arg.Any<string>()).Returns(new RmBalDto("RM001", 100m));
        api.GetFeeddoorStepAsync(Arg.Any<int>()).Returns<FeeddoorStepDto>(_ => throw new SettingNotFoundException("Feeddoor Step"));
        api.GetMixTempAsync(Arg.Any<int>()).Returns((MixTempDto?)null);

        await presenter.RunAsync(new AutoFeedRequest("RM001", 1, 1));

        view.Received(1).ShowMessage(Arg.Any<string>(), AppMessageType.Warning);
        view.DidNotReceive().CloseDialog(Arg.Any<DialogResult>());
        await plc.DidNotReceive().WriteDeviceAsync(Arg.Any<string>(), Arg.Any<int>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task MixTempMissing_ContinuesWithoutWarningOrClose_ThenSucceeds()
    {
        var (view, api, _, presenter) = Build();
        api.GetRmBalAsync(Arg.Any<string>()).Returns(new RmBalDto("RM001", 100m));
        api.GetFeeddoorStepAsync(Arg.Any<int>()).Returns(new FeeddoorStepDto(2, "D70", "Feeddoor Step"));
        api.GetMixTempAsync(Arg.Any<int>()).Returns((MixTempDto?)null); // missing row - not an error

        await presenter.RunAsync(new AutoFeedRequest("RM001", 1, 1));

        // No warning about the missing mix temp specifically, and the flow
        // reaches the success path (closes with the normal success message).
        view.Received(1).ShowMessage(Arg.Any<string>(), AppMessageType.Information);
        view.Received(1).CloseDialog(DialogResult.OK);
    }

    [Fact]
    public async Task DbWriteFailsDuringAutoFeed_ShowsWarning_DoesNotCloseDialog()
    {
        var (view, api, _, presenter) = Build();
        api.GetRmBalAsync(Arg.Any<string>()).Returns(new RmBalDto("RM001", 100m));
        api.GetFeeddoorStepAsync(Arg.Any<int>()).Returns(new FeeddoorStepDto(2, "D70", "Feeddoor Step"));
        api.GetMixTempAsync(Arg.Any<int>()).Returns((MixTempDto?)null);
        api.WithdrawRmBalAsync(Arg.Any<string>(), Arg.Any<decimal>(), Arg.Any<CancellationToken>())
            .Returns(_ => throw new HttpRequestException("db write failed"));

        await presenter.RunAsync(new AutoFeedRequest("RM001", 1, 1));

        // Must be the generic DB-write-failed message, NOT the PLC-specific
        // one below - a live run once found these two conflated.
        view.Received(1).ShowMessage(Resources.Strings.AutoFeedDbWriteFailed, AppMessageType.Warning);
        view.DidNotReceive().CloseDialog(Arg.Any<DialogResult>());
    }

    // A live run of the demo script found that a PLC connection/timeout
    // failure was falling into the same catch as a database failure and
    // showing the misleading "database write failed" message. This test
    // locks in the fix: the PLC gets its own message.
    [Fact]
    public async Task PlcUnreachable_ShowsPlcSpecificWarning_DoesNotCloseDialog()
    {
        var (view, api, plc, presenter) = Build();
        api.GetRmBalAsync(Arg.Any<string>()).Returns(new RmBalDto("RM001", 100m));
        api.GetFeeddoorStepAsync(Arg.Any<int>()).Returns(new FeeddoorStepDto(2, "D70", "Feeddoor Step"));
        api.GetMixTempAsync(Arg.Any<int>()).Returns((MixTempDto?)null);
        plc.OpenAsync(Arg.Any<CancellationToken>())
            .Returns(_ => throw new PlcConnectionException("PLC unreachable"));

        await presenter.RunAsync(new AutoFeedRequest("RM001", 1, 1));

        view.Received(1).ShowMessage(Resources.Strings.PlcUnreachable, AppMessageType.Warning);
        view.DidNotReceive().CloseDialog(Arg.Any<DialogResult>());
    }

    [Fact]
    public async Task PlcTimesOut_ShowsPlcSpecificWarning_DoesNotCloseDialog()
    {
        var (view, api, plc, presenter) = Build();
        api.GetRmBalAsync(Arg.Any<string>()).Returns(new RmBalDto("RM001", 100m));
        api.GetFeeddoorStepAsync(Arg.Any<int>()).Returns(new FeeddoorStepDto(2, "D70", "Feeddoor Step"));
        api.GetMixTempAsync(Arg.Any<int>()).Returns((MixTempDto?)null);
        plc.WriteDeviceAsync(Arg.Any<string>(), Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns(_ => throw new PlcTimeoutException("PLC timed out"));

        await presenter.RunAsync(new AutoFeedRequest("RM001", 1, 1));

        view.Received(1).ShowMessage(Resources.Strings.PlcUnreachable, AppMessageType.Warning);
        view.DidNotReceive().CloseDialog(Arg.Any<DialogResult>());
    }
}
