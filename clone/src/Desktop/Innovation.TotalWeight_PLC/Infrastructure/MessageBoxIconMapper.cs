using Innovation.TotalWeight_PLC.Interfaces.Views;

namespace Innovation.TotalWeight_PLC.Infrastructure;

// Shared by every IViewBase.ShowMessage implementation to avoid duplicating
// (and re-introducing bugs into) the same three-way icon mapping per form.
public static class MessageBoxIconMapper
{
    public static MessageBoxIcon ToIcon(AppMessageType type) => type switch
    {
        AppMessageType.Information => MessageBoxIcon.Information,
        AppMessageType.Error => MessageBoxIcon.Error,
        _ => MessageBoxIcon.Warning,
    };
}
