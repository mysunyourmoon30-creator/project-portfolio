using Innovation.Mvp.Core.Async;
using Innovation.Mvp.Core.Navigation;
using Innovation.TotalWeight_PLC.Infrastructure;
using Innovation.TotalWeight_PLC.Interfaces.Presenters;
using Innovation.TotalWeight_PLC.Interfaces.Views;
using Innovation.TotalWeight_PLC.Presenter.Implementations;
using Innovation.TotalWeight_PLC.UI.Implementations;
using Microsoft.Extensions.DependencyInjection;

namespace Innovation.TotalWeight_PLC;

internal static class Program
{
    internal static IServiceProvider ServiceProvider { get; private set; } = null!;

    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();

        ServiceProvider = BuildServiceProvider();

        var mainForm = (Form)ServiceProvider.GetRequiredService<IView_Main>();

        // Real message loop - the original app never called Application.Run()
        // at all and relied on nested ShowDialog() calls instead (see
        // Frontend ROADMAP §6). This is the Phase 0 fix.
        Application.Run(mainForm);
    }

    private static IServiceProvider BuildServiceProvider()
    {
        var services = new ServiceCollection();

        services.AddSingleton<IOperationTracer, NullOperationTracer>();
        services.AddSingleton<IAsyncOperationRunner, AsyncOperationRunner>();
        services.AddSingleton<INavigationService, NavigationService>();

        services.AddSingleton<IView_Main, frmMain>();
        services.AddSingleton<IPresenter_Main, Presenter_Main>();

        return services.BuildServiceProvider();
    }
}
