using System.ComponentModel;
using Innovation.Mvp.Core.Contracts;
using Innovation.TotalWeight_PLC.Interfaces.Presenters;
using Innovation.TotalWeight_PLC.ViewModel;

namespace Innovation.TotalWeight_PLC.Interfaces.Views;

public sealed class StepWeightEnteredEventArgs(int stepNo, decimal weight) : EventArgs
{
    public int StepNo { get; } = stepNo;
    public decimal Weight { get; } = weight;
}

public sealed class AutoFeedRequestedEventArgs(AutoFeedRequest request) : EventArgs
{
    public AutoFeedRequest Request { get; } = request;
}

// Exposes events instead of taking IPresenter_TotalWeight in its
// constructor - see IView_UserLogin.LoginRequested for why (avoids a
// view<->presenter DI cycle). The presenter subscribes to all four events
// and wraps each handler in IAsyncOperationRunner itself.
public interface IView_TotalWeight : IViewBase, IView<IPresenter_TotalWeight>
{
    string Barcode { get; set; }
    int KbTogetherId { get; set; }
    BindingList<StepRowViewModel> Steps { get; }

    event EventHandler<string>? BarcodeScanned;
    event EventHandler<StepWeightEnteredEventArgs>? StepWeightEntered;
    event EventHandler? SaveRequested;
    event EventHandler<int>? AcceptRequested;
    event EventHandler<AutoFeedRequestedEventArgs>? AutoFeedRequested;
}
