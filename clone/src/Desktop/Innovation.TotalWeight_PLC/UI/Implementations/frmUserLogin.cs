using Innovation.TotalWeight_PLC.Infrastructure;
using Innovation.TotalWeight_PLC.Interfaces.Views;

namespace Innovation.TotalWeight_PLC.UI.Implementations;

public partial class frmUserLogin : Form, IView_UserLogin
{
    public frmUserLogin()
    {
        InitializeComponent();
        UiTheme.ApplyForm(this);
        UiTheme.StyleTextBox(txtUsername);
        UiTheme.StyleTextBox(txtPassword);
        UiTheme.StyleButton(btnLogin, UiTheme.ButtonKind.Primary);
        UiTheme.StyleButton(btnCancel, UiTheme.ButtonKind.Secondary);
    }

    public string Username
    {
        get => txtUsername.Text;
        set => txtUsername.Text = value;
    }

    public string Password
    {
        get => txtPassword.Text;
        set => txtPassword.Text = value;
    }

    public event EventHandler? LoginRequested;

    public void Run() => ShowDialog();

    public void ShowMessage(string message, AppMessageType type = AppMessageType.Warning) =>
        MessageBox.Show(message, type.ToString(), MessageBoxButtons.OK, Infrastructure.MessageBoxIconMapper.ToIcon(type));

    public bool ShowConfirm(string message) =>
        MessageBox.Show(message, "ยืนยัน", MessageBoxButtons.YesNo) == DialogResult.Yes;

    public void CloseDialog(DialogResult result)
    {
        DialogResult = result;
        Close();
    }

    private void btnLogin_Click(object sender, EventArgs e) => LoginRequested?.Invoke(this, EventArgs.Empty);

    private void btnCancel_Click(object sender, EventArgs e) => CloseDialog(DialogResult.Cancel);
}
