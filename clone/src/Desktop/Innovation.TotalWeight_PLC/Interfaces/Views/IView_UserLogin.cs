using Innovation.Mvp.Core.Contracts;
using Innovation.TotalWeight_PLC.Interfaces.Presenters;

namespace Innovation.TotalWeight_PLC.Interfaces.Views;

public interface IView_UserLogin : IViewBase, IView<IPresenter_UserLogin>
{
    string Username { get; set; }
    string Password { get; set; }

    // The presenter subscribes to this instead of being injected into the
    // view's constructor - injecting IPresenter_UserLogin into frmUserLogin
    // while Presenter_UserLogin also depends on IView_UserLogin creates an
    // unresolvable DI cycle (view -> presenter -> view). Events keep the
    // dependency one-directional: presenter depends on view, never the
    // reverse.
    event EventHandler? LoginRequested;
}
