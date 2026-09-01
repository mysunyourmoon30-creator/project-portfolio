namespace Innovation.Mvp.Core.Contracts;

public interface IPresenter<TView>
{
    TView View { get; }
}
