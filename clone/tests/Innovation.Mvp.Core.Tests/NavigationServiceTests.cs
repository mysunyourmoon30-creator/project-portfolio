using FluentAssertions;
using Innovation.Mvp.Core.Contracts;
using Innovation.Mvp.Core.Navigation;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Innovation.Mvp.Core.Tests;

public class NavigationServiceTests
{
    public interface IFakeMainView : IView<FakeMainPresenter> { }

    public class FakeMainView : IFakeMainView
    {
        public int RunCount { get; private set; }
        public void Run() => RunCount++;
    }

    public class FakeMainPresenter : IGeneralViewPresenter<IFakeMainView>
    {
        public IFakeMainView View { get; }
        public int RunCount { get; private set; }

        public FakeMainPresenter(IFakeMainView view) => View = view;

        public void Run() => RunCount++;
    }

    public record FakeRequest(string Value);
    public record FakeResult(string Echo);

    public class FakeDialogPresenter : IDialogPresenter<IFakeMainView, FakeRequest, FakeResult>
    {
        public IFakeMainView View { get; }

        public FakeDialogPresenter(IFakeMainView view) => View = view;

        public FakeResult Run(FakeRequest request) => new(request.Value);
    }

    public class ThrowingDialogPresenter : IDialogPresenter<IFakeMainView, FakeRequest, FakeResult>
    {
        public IFakeMainView View { get; }

        public ThrowingDialogPresenter(IFakeMainView view) => View = view;

        public FakeResult Run(FakeRequest request) => throw new InvalidOperationException("boom");
    }

    private static IServiceProvider BuildContainer(Action<IServiceCollection> configure)
    {
        var services = new ServiceCollection();
        services.AddSingleton<IFakeMainView, FakeMainView>();
        services.AddSingleton<INavigationService, NavigationService>();
        configure(services);
        return services.BuildServiceProvider();
    }

    [Fact]
    public void RunMain_ResolvesPresenterFromContainer_AndCallsRunExactlyOnce()
    {
        var provider = BuildContainer(s => s.AddSingleton<FakeMainPresenter>());
        var nav = provider.GetRequiredService<INavigationService>();

        nav.RunMain<FakeMainPresenter, IFakeMainView>();

        provider.GetRequiredService<FakeMainPresenter>().RunCount.Should().Be(1);
    }

    [Fact]
    public void ShowDialog_ResolvesPresenterFromContainer_PassesRequestAndReturnsResult()
    {
        var provider = BuildContainer(s => s.AddTransient<FakeDialogPresenter>());
        var nav = provider.GetRequiredService<INavigationService>();

        var result = nav.ShowDialog<FakeDialogPresenter, IFakeMainView, FakeRequest, FakeResult>(new FakeRequest("hello"));

        result.Echo.Should().Be("hello");
    }

    [Fact]
    public void ShowDialog_WhenPresenterThrows_ExceptionPropagates()
    {
        var provider = BuildContainer(s => s.AddTransient<ThrowingDialogPresenter>());
        var nav = provider.GetRequiredService<INavigationService>();

        var act = () => nav.ShowDialog<ThrowingDialogPresenter, IFakeMainView, FakeRequest, FakeResult>(new FakeRequest("x"));

        act.Should().Throw<InvalidOperationException>().WithMessage("boom");
    }
}
