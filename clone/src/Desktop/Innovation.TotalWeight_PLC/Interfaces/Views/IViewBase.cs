namespace Innovation.TotalWeight_PLC.Interfaces.Views;

public enum AppMessageType
{
    Information,
    Warning,
    Error,
}

// Copied from Frontend ROADMAP §3 - the one view-messaging seam the
// original TotalWeight_PLC has that other apps don't. Kept unchanged: the
// bug being fixed in this app is NotFound() conflating "report" with
// "close" (see Presenter_ShowAutoFeed), not this interface's shape.
public interface IViewBase
{
    void ShowMessage(string message, AppMessageType type = AppMessageType.Warning);
    bool ShowConfirm(string message);
    void CloseDialog(DialogResult result);
}
