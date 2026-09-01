using FluentAssertions;
using Innovation.Mvp.Core.Contracts;
using NSubstitute;
using Xunit;

namespace Innovation.Mvp.Core.Tests;

public class PresenterLifecycleTests
{
    // Guards against the original back-wiring bug (`_view.Presenter = this`
    // in the presenter constructor, creating a view<->presenter reference
    // cycle) ever regressing: IView<T> must never expose a settable
    // Presenter property again.
    [Fact]
    public void IView_HasNoPresenterSetter()
    {
        var property = typeof(IView<>).GetProperty("Presenter");

        property.Should().BeNull();
    }

    public sealed class ThinPresenter : IGeneralViewPresenter<IView<object>>
    {
        public IView<object> View { get; }

        public ThinPresenter(IView<object> view) => View = view;

        public void Run() => View.Run();
    }

    [Fact]
    public void GeneralViewPresenter_Run_CallsViewRunExactlyOnce()
    {
        var view = Substitute.For<IView<object>>();
        var presenter = new ThinPresenter(view);

        presenter.Run();

        view.Received(1).Run();
    }
}
