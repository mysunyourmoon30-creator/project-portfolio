using Innovation.TotalWeight_PLC.Infrastructure;
using Innovation.TotalWeight_PLC.Interfaces.Views;

namespace Innovation.TotalWeight_PLC.UI.Implementations;

public partial class frmShowAutoFeed : Form, IView_ShowAutoFeed
{
    public frmShowAutoFeed()
    {
        InitializeComponent();
        UiTheme.ApplyForm(this);
        UiTheme.StyleButton(btnClose, UiTheme.ButtonKind.Secondary);
    }

    public void Run() => ShowDialog();

    public void ShowMessage(string message, AppMessageType type = AppMessageType.Warning) =>
        MessageBox.Show(message, type.ToString(), MessageBoxButtons.OK, Infrastructure.MessageBoxIconMapper.ToIcon(type));

    public bool ShowConfirm(string message) => MessageBox.Show(message, "ยืนยัน", MessageBoxButtons.YesNo) == DialogResult.Yes;

    // Only ever called by Presenter_ShowAutoFeed's single success path -
    // see that class's remarks for why every failure branch deliberately
    // does NOT call this.
    public void CloseDialog(DialogResult result)
    {
        DialogResult = result;
        Close();
    }
}
