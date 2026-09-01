using Innovation.Mvp.Core.Contracts;
using Innovation.TotalWeight_PLC.Interfaces.Views;
using Innovation.TotalWeight_PLC.ViewModel;

namespace Innovation.TotalWeight_PLC.Interfaces.Presenters;

// Replaces the "controller writes params onto view properties, calls Run(),
// reads result back off the view" pattern (Frontend ROADMAP §5b.4:
// RunSelectKB). Request goes in and result comes out through Run() itself.
public interface IPresenter_SelectKB : IDialogPresenter<IView_SelectKB, SelectKbRequest, SelectKbResult>
{
}
