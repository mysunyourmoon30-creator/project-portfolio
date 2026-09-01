using System.ComponentModel;
using Innovation.TotalWeight_PLC.Interfaces.Views;
using Innovation.TotalWeight_PLC.ViewModel;

namespace Innovation.TotalWeight_PLC.UI.Implementations;

// The main operator screen. Hotkeys [F5] Save / [F2] Accept / [Esc] Close
// are preserved from the original (Frontend ROADMAP §8.5: operators wear
// gloves and rely on the keyboard, not the mouse) via ProcessCmdKey -
// simply putting "[F5]" in a button's label does not itself bind the key in
// standard WinForms the way it silently appeared to in the DevExpress
// original.
public partial class frmTotalWeight : Form, IView_TotalWeight
{
    private readonly BindingList<StepRowViewModel> _steps = new();

    public frmTotalWeight()
    {
        InitializeComponent();
        gridSteps.DataSource = _steps;
    }

    public string Barcode
    {
        get => txtBarcode.Text;
        set => txtBarcode.Text = value;
    }

    public int KbTogetherId { get; set; }

    public BindingList<StepRowViewModel> Steps => _steps;

    public event EventHandler<string>? BarcodeScanned;
    public event EventHandler<StepWeightEnteredEventArgs>? StepWeightEntered;
    public event EventHandler? SaveRequested;
    public event EventHandler<int>? AcceptRequested;

    public void Run() { } // main window - message loop started by Program.cs, not here

    public void ShowMessage(string message, AppMessageType type = AppMessageType.Warning) =>
        MessageBox.Show(message, type.ToString(), MessageBoxButtons.OK,
            type == AppMessageType.Error ? MessageBoxIcon.Error : MessageBoxIcon.Warning);

    public bool ShowConfirm(string message) =>
        MessageBox.Show(message, "ยืนยัน", MessageBoxButtons.YesNo) == DialogResult.Yes;

    public void CloseDialog(DialogResult result) { } // no-op on the main window

    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
        switch (keyData)
        {
            case Keys.F5:
                btnSave.PerformClick();
                return true;
            case Keys.F2:
                RaiseAcceptForSelectedRow();
                return true;
            case Keys.Escape:
                Close();
                return true;
            default:
                return base.ProcessCmdKey(ref msg, keyData);
        }
    }

    private void txtBarcode_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode != Keys.Enter)
        {
            return;
        }

        BarcodeScanned?.Invoke(this, Barcode);
    }

    private void gridSteps_CellEndEdit(object sender, DataGridViewCellEventArgs e)
    {
        if (gridSteps.Columns[e.ColumnIndex].Name != nameof(StepRowViewModel.Actual))
        {
            return;
        }

        var row = _steps[e.RowIndex];
        if (row.Actual is { } weight)
        {
            StepWeightEntered?.Invoke(this, new StepWeightEnteredEventArgs(row.StepNo, weight));
        }
    }

    private void btnSave_Click(object sender, EventArgs e) => SaveRequested?.Invoke(this, EventArgs.Empty);

    private void btnAccept_Click(object sender, EventArgs e) => RaiseAcceptForSelectedRow();

    private void RaiseAcceptForSelectedRow()
    {
        if (gridSteps.CurrentRow?.DataBoundItem is StepRowViewModel row)
        {
            AcceptRequested?.Invoke(this, row.StepNo);
        }
    }
}
