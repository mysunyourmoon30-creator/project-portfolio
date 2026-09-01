using Innovation.Services.Errors;
using Innovation.TotalWeight_PLC.Infrastructure;
using Innovation.TotalWeight_PLC.Interfaces.Presenters;
using Innovation.TotalWeight_PLC.Interfaces.Views;
using Innovation.TotalWeight_PLC.Service;

namespace Innovation.TotalWeight_PLC.Presenter.Implementations;

public sealed class Presenter_UserLogin : IPresenter_UserLogin
{
    private readonly IApiClient _api;
    private readonly IAuthSession _authSession;

    public IView_UserLogin View { get; }

    // Constructor injection only - no `View.Presenter = this` back-wiring.
    // The view has no dependency on the presenter at all (see
    // IView_UserLogin.LoginRequested); this class is the only one that
    // knows both sides exist.
    public Presenter_UserLogin(IView_UserLogin view, IApiClient api, IAuthSession authSession)
    {
        View = view;
        _api = api;
        _authSession = authSession;
        View.LoginRequested += async (_, _) => await LoginAsync();
    }

    public void Run() => View.Run();

    public async Task<bool> LoginAsync()
    {
        try
        {
            var result = await _api.LoginAsync(View.Username, View.Password);
            _authSession.Token = result.Token;
            View.CloseDialog(DialogResult.OK);
            return true;
        }
        catch (InvalidCredentialsException)
        {
            View.ShowMessage(Resources.Strings.InvalidCredentials, AppMessageType.Warning);
            return false;
        }
    }
}
