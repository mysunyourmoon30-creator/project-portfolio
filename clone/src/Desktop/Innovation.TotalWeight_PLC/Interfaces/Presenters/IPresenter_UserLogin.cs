using Innovation.Mvp.Core.Contracts;
using Innovation.TotalWeight_PLC.Interfaces.Views;

namespace Innovation.TotalWeight_PLC.Interfaces.Presenters;

public interface IPresenter_UserLogin : IGeneralViewPresenter<IView_UserLogin>
{
    Task<bool> LoginAsync();
}
