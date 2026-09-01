using Innovation.Mvp.Core.Contracts;
using Innovation.TotalWeight_PLC.Interfaces.Views;
using Innovation.TotalWeight_PLC.ViewModel;

namespace Innovation.TotalWeight_PLC.Interfaces.Presenters;

// This is the presenter that proves the fix for Frontend ROADMAP §5b.2:
// the original's NotFound() conflated "report a problem" with "close the
// form" - RunAsync below reports every failure via View.ShowMessage but
// only ever calls View.CloseDialog on the success path.
public interface IPresenter_ShowAutoFeed : IPresenter<IView_ShowAutoFeed>
{
    Task RunAsync(AutoFeedRequest request);
}
