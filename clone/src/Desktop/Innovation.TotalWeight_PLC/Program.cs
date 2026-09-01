using Innovation.Hardware;
using Innovation.Mvp.Core.Async;
using Innovation.Mvp.Core.Navigation;
using Innovation.TotalWeight_PLC.Infrastructure;
using Innovation.TotalWeight_PLC.Interfaces.Presenters;
using Innovation.TotalWeight_PLC.Interfaces.Views;
using Innovation.TotalWeight_PLC.Presenter.Implementations;
using Innovation.TotalWeight_PLC.Service;
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
        RegisterActivityListener();

        ServiceProvider = BuildServiceProvider();

        using var loginScope = ServiceProvider.CreateScope();
        var loginPresenter = loginScope.ServiceProvider.GetRequiredService<IPresenter_UserLogin>();
        loginPresenter.Run();
        if (((Form)loginPresenter.View).DialogResult != DialogResult.OK)
        {
            return; // operator cancelled login - nothing to run
        }

        using var mainScope = ServiceProvider.CreateScope();
        var mainForm = (Form)mainScope.ServiceProvider.GetRequiredService<IView_TotalWeight>();

        // Real message loop - the original app never called Application.Run()
        // at all and relied on nested ShowDialog() calls instead (Frontend
        // ROADMAP §6).
        Application.Run(mainForm);
    }

    private static void RegisterActivityListener()
    {
        System.Diagnostics.ActivitySource.AddActivityListener(new System.Diagnostics.ActivityListener
        {
            ShouldListenTo = source => source.Name == "Innovation.TotalWeight_PLC",
            Sample = (ref System.Diagnostics.ActivityCreationOptions<System.Diagnostics.ActivityContext> _) =>
                System.Diagnostics.ActivitySamplingResult.AllData,
        });
    }

    private static IServiceProvider BuildServiceProvider()
    {
        var services = new ServiceCollection();

        services.AddSingleton<IOperationTracer, OperationTracer>();
        services.AddSingleton<IAsyncOperationRunner, AsyncOperationRunner>();
        services.AddSingleton<INavigationService, NavigationService>();

        services.AddSingleton<IAuthSession, AuthSession>();
        services.AddTransient<AuthHeaderHandler>();
        services.AddHttpClient<IApiClient, ApiClient>(client =>
        {
            client.BaseAddress = new Uri("http://localhost:5299/");
        }).AddHttpMessageHandler<AuthHeaderHandler>();

        // Demo hardware: simulators only. Swapping to real MX
        // Component/serial hardware is a one-line change here - see
        // docs/hardware/connecting-real-plc.md.
        services.AddSingleton<IPlcDevice>(new SimulatedPlcDevice());
        services.AddSingleton<IScaleReader, SimulatedScaleReader>();
        services.AddSingleton<IBarcodeSource, ScriptedBarcodeSource>();

        services.AddScoped<IView_UserLogin, frmUserLogin>();
        services.AddScoped<IPresenter_UserLogin, Presenter_UserLogin>();

        services.AddScoped<IView_SelectKB, frmSelectKB>();
        services.AddScoped<IPresenter_SelectKB, Presenter_SelectKB>();

        services.AddScoped<IView_TotalWeight, frmTotalWeight>();
        services.AddScoped<IPresenter_TotalWeight, Presenter_TotalWeight>();

        services.AddScoped<IView_ShowAutoFeed, frmShowAutoFeed>();
        services.AddScoped<IPresenter_ShowAutoFeed, Presenter_ShowAutoFeed>();

        return services.BuildServiceProvider();
    }
}
