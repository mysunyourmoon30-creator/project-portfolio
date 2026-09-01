using System.IO.Ports;

namespace Innovation.Hardware.RealDevices;

// Real-hardware IScaleReader over a serial port (Frontend ROADMAP §9.4).
// Unlike MxComponentPlcDevice, System.IO.Ports.SerialPort ships in the BCL,
// so this one is fully wired up - it just has never been run against a
// physical scale in this environment. Port name/baud rate/parsing format
// are placeholders; see docs/hardware/connecting-real-plc.md.
public sealed class SerialScaleReader : IScaleReader, IDisposable
{
    private readonly SerialPort _port;

    public decimal CurrentWeight { get; private set; }

    public event EventHandler<decimal>? WeightChanged;

    public SerialScaleReader(string portName, int baudRate = 9600)
    {
        _port = new SerialPort(portName, baudRate);
        _port.DataReceived += OnDataReceived;
    }

    public Task OpenAsync(CancellationToken cancellationToken = default)
    {
        _port.Open();
        return Task.CompletedTask;
    }

    public Task CloseAsync(CancellationToken cancellationToken = default)
    {
        _port.Close();
        return Task.CompletedTask;
    }

    private void OnDataReceived(object sender, SerialDataReceivedEventArgs e)
    {
        var line = _port.ReadLine();
        if (decimal.TryParse(line, out var weight))
        {
            CurrentWeight = weight;
            WeightChanged?.Invoke(this, weight);
        }
    }

    public void Dispose() => _port.Dispose();
}
