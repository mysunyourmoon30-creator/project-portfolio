using Innovation.Mvp.Core.Contracts;

namespace Innovation.Mvp.Core.Navigation;

// Replaces the original IApplicationController: a flat list of ~40 RunXxx()
// methods, one per screen, growing forever as the app grows. Screens are
// resolved generically here instead of each getting its own named method.
public interface INavigationService
{
    void RunMain<TPresenter, TView>() where TPresenter : IGeneralViewPresenter<TView>;

    TResult ShowDialog<TPresenter, TView, TRequest, TResult>(TRequest request)
        where TPresenter : IDialogPresenter<TView, TRequest, TResult>;
}
