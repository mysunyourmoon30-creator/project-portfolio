using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Innovation.TotalWeight_PLC.ViewModel;

// Bound via BindingList<StepRowViewModel> + DataGridView (Phase 0 fix #4:
// "BindingSource + INotifyPropertyChanged" replacing the original's
// presenter pushing values into control.Text one property at a time).
public sealed class StepRowViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    public int StepNo { get; init; }
    public string RawMaterialCode { get; init; } = string.Empty;
    public decimal Target { get; init; }
    public decimal Min { get; init; }
    public decimal Max { get; init; }

    private decimal? _actual;
    public decimal? Actual
    {
        get => _actual;
        set => SetField(ref _actual, value);
    }

    private bool _accepted;
    public bool Accepted
    {
        get => _accepted;
        set => SetField(ref _accepted, value);
    }

    private void SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (Equals(field, value))
        {
            return;
        }

        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
