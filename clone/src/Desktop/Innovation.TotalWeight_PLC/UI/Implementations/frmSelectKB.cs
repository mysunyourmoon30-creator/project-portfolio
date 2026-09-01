using Innovation.TotalWeight_PLC.Interfaces.Presenters;
using Innovation.TotalWeight_PLC.Interfaces.Views;
using Innovation.TotalWeight_PLC.ViewModel;

namespace Innovation.TotalWeight_PLC.UI.Implementations;

public partial class frmSelectKB : Form, IView_SelectKB
{
    public frmSelectKB()
    {
        InitializeComponent();
    }

    public List<KanbanSummary> AvailableKanbans
    {
        get => (List<KanbanSummary>)gridKanbans.DataSource;
        set => gridKanbans.DataSource = value;
    }

    public KanbanSummary? SelectedKanban { get; set; }

    public void Run() => ShowDialog();

    public void ShowMessage(string message, AppMessageType type = AppMessageType.Warning) =>
        MessageBox.Show(message, type.ToString(), MessageBoxButtons.OK, Infrastructure.MessageBoxIconMapper.ToIcon(type));

    public bool ShowConfirm(string message) => MessageBox.Show(message, "ยืนยัน", MessageBoxButtons.YesNo) == DialogResult.Yes;

    public void CloseDialog(DialogResult result)
    {
        DialogResult = result;
        Close();
    }

    private void btnOk_Click(object sender, EventArgs e)
    {
        if (gridKanbans.CurrentRow?.DataBoundItem is KanbanSummary selected)
        {
            SelectedKanban = selected;
            CloseDialog(DialogResult.OK);
        }
    }

    private void btnCancel_Click(object sender, EventArgs e)
    {
        SelectedKanban = null;
        CloseDialog(DialogResult.Cancel);
    }
}
