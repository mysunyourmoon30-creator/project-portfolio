namespace Innovation.Mvp.Core.Contracts;

// Replaces the original pattern where a controller wrote request parameters
// onto public mutable view properties, called Run() (which blocked on
// ShowDialog()), then read the result back off another view property. That
// made the view's state part of the public contract and impossible to unit
// test in isolation. Here the request goes in and the result comes out
// through the method signature instead.
public interface IDialogPresenter<TView, TRequest, TResult> : IPresenter<TView>
{
    TResult Run(TRequest request);
}
