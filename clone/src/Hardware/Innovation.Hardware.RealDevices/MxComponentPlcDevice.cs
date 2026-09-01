namespace Innovation.Hardware.RealDevices;

// Real-hardware IPlcDevice, built against Mitsubishi's MX Component COM
// library (Frontend ROADMAP §9.1: `ActUtlTypeLib.ActUtlType`).
//
// This class is a documented placeholder, not a working implementation:
// MX Component is a licensed, Windows-only COM component that must be
// installed and registered on the target machine, and no such environment
// is available where this clone was built. Wiring up the real thing means:
//   1. Install MX Component and register ActUtlTypeLib.
//   2. Add <COMReference Include="ActUtlTypeLib" .../> to this project.
//   3. Replace the NotSupportedExceptions below with calls to
//      ActUtlType.Open()/Close()/GetDevice()/SetDevice(), converting
//      between this interface's string addresses (e.g. "D70") and the
//      logical station number configured for the target PLC.
// See docs/hardware/connecting-real-plc.md for the full procedure.
public sealed class MxComponentPlcDevice : IPlcDevice
{
    public bool IsConnected { get; private set; }

    public Task OpenAsync(CancellationToken cancellationToken = default) =>
        throw new NotSupportedException("MX Component is not available in this environment. See class remarks.");

    public Task CloseAsync(CancellationToken cancellationToken = default)
    {
        IsConnected = false;
        return Task.CompletedTask;
    }

    public Task<int> ReadDeviceAsync(string address, CancellationToken cancellationToken = default) =>
        throw new NotSupportedException("MX Component is not available in this environment. See class remarks.");

    public Task WriteDeviceAsync(string address, int value, CancellationToken cancellationToken = default) =>
        throw new NotSupportedException("MX Component is not available in this environment. See class remarks.");
}
