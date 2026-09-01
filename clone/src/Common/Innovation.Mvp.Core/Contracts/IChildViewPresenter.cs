namespace Innovation.Mvp.Core.Contracts;

public interface IChildViewPresenter<TView, TParentPresenter> : IPresenter<TView>
{
    void Run(IView<TParentPresenter> parentView);
}
