namespace Innovation.Mvp.Core.Contracts;

// Fixed vs. the original: no `TPresenter Presenter { set; }`.
// The original had the presenter push itself onto the view in its constructor
// (`_view.Presenter = this`), creating a reference cycle between view and
// presenter. Views are resolved with the presenter already known by the
// navigation service, so the view never needs a settable back-reference.
public interface IView<TPresenter>
{
    void Run();
}
