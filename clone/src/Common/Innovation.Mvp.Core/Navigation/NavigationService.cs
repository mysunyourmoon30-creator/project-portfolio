using Innovation.Mvp.Core.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace Innovation.Mvp.Core.Navigation;

public sealed class NavigationService : INavigationService
{
    private readonly IServiceProvider _serviceProvider;

    public NavigationService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public void RunMain<TPresenter, TView>() where TPresenter : IGeneralViewPresenter<TView>
    {
        var presenter = _serviceProvider.GetRequiredService<TPresenter>();
        presenter.Run();
    }

    public TResult ShowDialog<TPresenter, TView, TRequest, TResult>(TRequest request)
        where TPresenter : IDialogPresenter<TView, TRequest, TResult>
    {
        var presenter = _serviceProvider.GetRequiredService<TPresenter>();
        return presenter.Run(request);
    }
}
