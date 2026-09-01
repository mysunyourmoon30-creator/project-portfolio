namespace Innovation.Hardware;

public sealed class PlcSimulationScript
{
    public bool FailToConnect { get; set; }
    public TimeSpan? ResponseDelay { get; set; }
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(5);
}

// The demo default IPlcDevice - an in-memory register dictionary driven by
// a PlcSimulationScript, so tests can force "PLC unreachable" or "PLC slow
// to respond" scenarios without any real hardware or COM dependency.
public sealed class SimulatedPlcDevice : IPlcDevice
{
    private readonly PlcSimulationScript _script;
    private readonly Dictionary<string, int> _registers = new();

    public bool IsConnected { get; private set; }

    public SimulatedPlcDevice(PlcSimulationScript? script = null)
    {
        _script = script ?? new PlcSimulationScript();
    }

    public Task OpenAsync(CancellationToken cancellationToken = default)
    {
        if (_script.FailToConnect)
        {
            throw new PlcConnectionException("Simulated PLC refused connection.");
        }

        // ResponseDelay/Timeout simulate a slow device READ/WRITE, not a
        // slow connection handshake - deliberately not applied here.
        IsConnected = true;
        return Task.CompletedTask;
    }

    public Task CloseAsync(CancellationToken cancellationToken = default)
    {
        IsConnected = false;
        return Task.CompletedTask;
    }

    public async Task<int> ReadDeviceAsync(string address, CancellationToken cancellationToken = default)
    {
        EnsureConnected();
        await SimulateDelayAsync(cancellationToken);
        return _registers.GetValueOrDefault(address, 0);
    }

    public async Task WriteDeviceAsync(string address, int value, CancellationToken cancellationToken = default)
    {
        EnsureConnected();
        await SimulateDelayAsync(cancellationToken);
        _registers[address] = value;
    }

    private void EnsureConnected()
    {
        if (!IsConnected)
        {
            throw new PlcConnectionException("PLC device is not open.");
        }
    }

    private async Task SimulateDelayAsync(CancellationToken cancellationToken)
    {
        if (_script.ResponseDelay is not { } delay)
        {
            return;
        }

        if (delay > _script.Timeout)
        {
            try
            {
                await Task.Delay(_script.Timeout, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                // fall through to the timeout exception below regardless of
                // whether the caller's own token or ours elapsed first
            }

            throw new PlcTimeoutException($"PLC did not respond within {_script.Timeout}.");
        }

        await Task.Delay(delay, cancellationToken);
    }
}
