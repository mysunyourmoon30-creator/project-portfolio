namespace Innovation.Hardware;

public sealed class SimulatedScaleReader : IScaleReader
{
    private bool _isOpen;

    public decimal CurrentWeight { get; private set; }

    public event EventHandler<decimal>? WeightChanged;

    public Task OpenAsync(CancellationToken cancellationToken = default)
    {
        _isOpen = true;
        return Task.CompletedTask;
    }

    public Task CloseAsync(CancellationToken cancellationToken = default)
    {
        _isOpen = false;
        return Task.CompletedTask;
    }

    // Test/simulation hook standing in for the real scale's serial stream.
    public void PushWeight(decimal weightKg)
    {
        if (!_isOpen)
        {
            throw new InvalidOperationException("Scale reader is not open.");
        }

        CurrentWeight = weightKg;
        WeightChanged?.Invoke(this, weightKg);
    }
}
