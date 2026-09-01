namespace Innovation.Mvp.Core.Contracts;

// ParentView setter is legitimate owner-window wiring (needed for modal
// centering / MDI-style relationships), unlike the presenter back-reference
// removed from IView<T> — kept as-is.
public interface IChildView<TPresenter, TParentPresenter> : IView<TPresenter>
{
    IView<TParentPresenter> ParentView { set; }
}
