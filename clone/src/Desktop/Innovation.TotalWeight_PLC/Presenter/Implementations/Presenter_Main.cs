using Innovation.TotalWeight_PLC.Interfaces.Presenters;
using Innovation.TotalWeight_PLC.Interfaces.Views;

namespace Innovation.TotalWeight_PLC.Presenter.Implementations;

// Real screens (frmTotalWeight etc.) are built in Phase 4. This presenter
// only exists so Phase 0 can prove the composition root resolves a
// view+presenter pair via DI and starts a real message loop.
public sealed class Presenter_Main : IPresenter_Main
{
    public IView_Main View { get; }

    public Presenter_Main(IView_Main view)
    {
        View = view;
    }

    public void Run() => View.Run();
}
