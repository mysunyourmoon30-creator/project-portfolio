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
        lblUsername.Location = new Point(20, 20);
        lblUsername.AutoSize = true;

        txtUsername.Location = new Point(120, 17);
        txtUsername.Width = 180;

        lblPassword.Text = "รหัสผ่าน";
        lblPassword.Location = new Point(20, 55);
        lblPassword.AutoSize = true;

        txtPassword.Location = new Point(120, 52);
        txtPassword.Width = 180;
        txtPassword.UseSystemPasswordChar = true;

        btnLogin.Text = "เข้าสู่ระบบ [Enter]";
        btnLogin.Location = new Point(120, 90);
        btnLogin.Click += btnLogin_Click;

        btnCancel.Text = "ยกเลิก [Esc]";
        btnCancel.Location = new Point(225, 90);
        btnCancel.Click += btnCancel_Click;

        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(340, 140);
        Controls.Add(lblUsername);
        Controls.Add(txtUsername);
        Controls.Add(lblPassword);
        Controls.Add(txtPassword);
        Controls.Add(btnLogin);
        Controls.Add(btnCancel);
        AcceptButton = btnLogin;
        CancelButton = btnCancel;
        StartPosition = FormStartPosition.CenterScreen;
        Text = "เข้าสู่ระบบ - Innovation.TotalWeight_PLC";
    }
}
