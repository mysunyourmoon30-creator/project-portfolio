namespace Innovation.TotalWeight_PLC.UI.Implementations;

partial class frmUserLogin
{
    private System.ComponentModel.IContainer components = null;
    private Label lblUsername;
    private Label lblPassword;
    private TextBox txtUsername;
    private TextBox txtPassword;
    private Button btnLogin;
    private Button btnCancel;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        lblUsername = new Label();
        lblPassword = new Label();
        txtUsername = new TextBox();
        txtPassword = new TextBox();
        btnLogin = new Button();
        btnCancel = new Button();

        lblUsername.Text = "ชื่อผู้ใช้";
        lblUsername.Location = new Point(30, 30);
        lblUsername.AutoSize = true;

        txtUsername.Location = new Point(140, 26);
        txtUsername.Size = new Size(250, 28);

        lblPassword.Text = "รหัสผ่าน";
        lblPassword.Location = new Point(30, 70);
        lblPassword.AutoSize = true;

        txtPassword.Location = new Point(140, 66);
        txtPassword.Size = new Size(250, 28);
        txtPassword.UseSystemPasswordChar = true;

        // Button widths are generous relative to their measured text width -
        // TextRenderer (GDI) sizes Thai + bracket-hint text wider than
        // Graphics.MeasureString (GDI+) predicts, and a too-tight width
        // silently clips the trailing "[Enter]"/"[Esc]" hotkey hint with no
        // visual warning (no ellipsis) - found by screenshotting the actual
        // rendered button, not by trusting measured string width alone.
        btnLogin.Text = "เข้าสู่ระบบ [Enter]";
        btnLogin.Location = new Point(70, 116);
        btnLogin.Size = new Size(160, 36);
        btnLogin.Click += btnLogin_Click;

        btnCancel.Text = "ยกเลิก [Esc]";
        btnCancel.Location = new Point(240, 116);
        btnCancel.Size = new Size(110, 36);
        btnCancel.Click += btnCancel_Click;

        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(420, 172);
        Controls.Add(lblUsername);
        Controls.Add(txtUsername);
        Controls.Add(lblPassword);
        Controls.Add(txtPassword);
        Controls.Add(btnLogin);
        Controls.Add(btnCancel);
        AcceptButton = btnLogin;
        CancelButton = btnCancel;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        StartPosition = FormStartPosition.CenterScreen;
        Text = "เข้าสู่ระบบ - Innovation.TotalWeight_PLC";
    }
}
