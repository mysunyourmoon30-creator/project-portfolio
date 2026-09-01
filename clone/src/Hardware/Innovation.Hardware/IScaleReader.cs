namespace Innovation.Hardware;

// Replaces the original's direct `SerialPort` read inside frmMain.cs
// (Frontend ROADMAP §9.4).
public interface IScaleReader
{
    decimal CurrentWeight { get; }
    event EventHandler<decimal>? WeightChanged;
    Task OpenAsync(CancellationToken cancellationToken = default);
    Task CloseAsync(CancellationToken cancellationToken = default);
}
