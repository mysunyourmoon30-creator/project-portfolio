namespace Innovation.Hardware;

// Replaces the original's direct use of the concrete MX Component COM type
// `ActUtlType`, which the original leaked all the way onto a view interface
// (Frontend ROADMAP §9.2: `ActUtlType ActFXCPU1 { get; }`) - meaning there
// was no seam anywhere to substitute a simulator. Address is a string
// (e.g. "D70") to match Mitsubishi device notation.
public interface IPlcDevice
{
    bool IsConnected { get; }
    Task OpenAsync(CancellationToken cancellationToken = default);
    Task CloseAsync(CancellationToken cancellationToken = default);
    Task<int> ReadDeviceAsync(string address, CancellationToken cancellationToken = default);
    Task WriteDeviceAsync(string address, int value, CancellationToken cancellationToken = default);
}
