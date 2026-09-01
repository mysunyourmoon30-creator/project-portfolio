namespace Innovation.Hardware;

// Replaces the original's keyboard-wedge barcode reader firing straight
// into a TextBox's KeyPress handler (Frontend ROADMAP §9.4).
public interface IBarcodeSource
{
    event EventHandler<string>? BarcodeScanned;
}
