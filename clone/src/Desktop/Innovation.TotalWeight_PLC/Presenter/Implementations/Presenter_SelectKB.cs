using Innovation.TotalWeight_PLC.Interfaces.Presenters;
using Innovation.TotalWeight_PLC.Interfaces.Views;
using Innovation.TotalWeight_PLC.ViewModel;

namespace Innovation.TotalWeight_PLC.Presenter.Implementations;

public sealed class Presenter_SelectKB : IPresenter_SelectKB
{
    public IView_SelectKB View { get; }

    public Presenter_SelectKB(IView_SelectKB view) => View = view;

    public SelectKbResult Run(SelectKbRequest request)
    {
        View.AvailableKanbans = request.Candidates;
        View.Run(); // blocks on ShowDialog()
        return new SelectKbResult(View.SelectedKanban);
    }
}
