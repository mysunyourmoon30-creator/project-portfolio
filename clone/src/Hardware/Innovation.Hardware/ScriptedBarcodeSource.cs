namespace Innovation.Hardware;

public sealed class ScriptedBarcodeSource : IBarcodeSource
{
    public event EventHandler<string>? BarcodeScanned;

    public void Fire(string barcode) => BarcodeScanned?.Invoke(this, barcode);
}
