using System.ComponentModel;
using Innovation.TotalWeight_PLC.Infrastructure;
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

        UiTheme.ApplyForm(this);
        UiTheme.StyleTextBox(txtBarcode);
        UiTheme.StyleGrid(gridSteps);
        UiTheme.StyleButton(btnSelectKanban, UiTheme.ButtonKind.Secondary);
        UiTheme.StyleButton(btnSave, UiTheme.ButtonKind.Primary);
        UiTheme.StyleButton(btnAccept, UiTheme.ButtonKind.Success);
        UiTheme.StyleButton(btnAutoFeed, UiTheme.ButtonKind.Secondary);
        UiTheme.StyleTextBox(txtAutoFeedBarcode);
        UiTheme.StyleTextBox(txtAutoFeedLineId);
        UiTheme.StyleTextBox(txtAutoFeedPlanId);
        grpAutoFeed.ForeColor = UiTheme.TextSecondary;

        // Fill mode alone left the auto-generated columns at their default
        // AutoGenerateColumns widths on first paint (no resize event to
        // trigger recalculation) - explicit FillWeights, applied once the
        // columns exist, is what actually makes "RawMaterialCode" render
        // without clipping instead of relying on Fill's own initial layout.
        gridSteps.DataBindingComplete += (_, _) =>
        {
            foreach (DataGridViewColumn column in gridSteps.Columns)
            {
                column.FillWeight = column.Name switch
                {
                    nameof(StepRowViewModel.StepNo) => 70,
                    nameof(StepRowViewModel.RawMaterialCode) => 150,
                    nameof(StepRowViewModel.Accepted) => 90,
                    _ => 100,
                };
            }
        };

        gridSteps.DataSource = _steps;

        // AutoGenerateColumns makes every column editable by default. A
        // real run of the demo script found an operator could tick the
        // Accepted checkbox directly in the grid - no API call, nothing in
        // the server log, just a UI silently lying about whether the step
        // was actually accepted. The obvious next question - "does {get;
        // init;} on StepRowViewModel's other properties protect them the
        // same way?" - turned out to be NO: DataGridView edits go through
        // PropertyDescriptor.SetValue (reflection), which happily calls an
        // init accessor's underlying setter method at runtime - `init` is
        // a C#-compiler-only restriction, not a CLR one. A live test
        // overwrote the Target column from 10.0 to 999 through the grid
        // despite it being `init`-only. So: every column is locked down
        // except Actual, the one field the operator is actually meant to
        // type into.
        gridSteps.DataBindingComplete += (_, _) =>
        {
            foreach (DataGridViewColumn column in gridSteps.Columns)
            {
                column.ReadOnly = column.Name != nameof(StepRowViewModel.Actual);
            }
        };
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
    public event EventHandler<AutoFeedRequestedEventArgs>? AutoFeedRequested;
    public event EventHandler? SelectKanbanRequested;

    public void Run() { } // main window - message loop started by Program.cs, not here

    public void ShowMessage(string message, AppMessageType type = AppMessageType.Warning) =>
        MessageBox.Show(message, type.ToString(), MessageBoxButtons.OK, Infrastructure.MessageBoxIconMapper.ToIcon(type));

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

    private void btnSelectKanban_Click(object sender, EventArgs e) => SelectKanbanRequested?.Invoke(this, EventArgs.Empty);

    private void btnAutoFeed_Click(object sender, EventArgs e)
    {
        if (!int.TryParse(txtAutoFeedLineId.Text, out var lineId) || !int.TryParse(txtAutoFeedPlanId.Text, out var planId))
        {
            ShowMessage("Line ID และ Plan ID ต้องเป็นตัวเลข", AppMessageType.Warning);
            return;
        }

        AutoFeedRequested?.Invoke(this, new AutoFeedRequestedEventArgs(new AutoFeedRequest(txtAutoFeedBarcode.Text, lineId, planId)));
    }
}
