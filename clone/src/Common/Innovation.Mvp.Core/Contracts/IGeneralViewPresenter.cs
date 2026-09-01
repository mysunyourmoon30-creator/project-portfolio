namespace Innovation.Mvp.Core.Contracts;

public interface IGeneralViewPresenter<TView> : IPresenter<TView>
{
    void Run();
}
