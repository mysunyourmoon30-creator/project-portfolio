namespace Innovation.TotalWeight_PLC.UI.Implementations;

partial class frmTotalWeight
{
    private System.ComponentModel.IContainer components = null;
    private Label lblBarcode;
    private TextBox txtBarcode;
    private DataGridView gridSteps;
    private Button btnSave;
    private Button btnAccept;

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
        lblBarcode = new Label();
        txtBarcode = new TextBox();
        gridSteps = new DataGridView();
        btnSave = new Button();
        btnAccept = new Button();

        lblBarcode.Text = "ยิงบาร์โค้ดคัมบัง";
        lblBarcode.Location = new Point(10, 12);
        lblBarcode.AutoSize = true;

        txtBarcode.Location = new Point(140, 9);
        txtBarcode.Width = 220;
        txtBarcode.KeyDown += txtBarcode_KeyDown;

        gridSteps.Location = new Point(10, 45);
        gridSteps.Size = new Size(700, 300);
        gridSteps.AutoGenerateColumns = true;
        gridSteps.AllowUserToAddRows = false;
        gridSteps.CellEndEdit += gridSteps_CellEndEdit;

        btnSave.Text = "บันทึก [F5]";
        btnSave.Location = new Point(460, 355);
        btnSave.Click += btnSave_Click;

        btnAccept.Text = "ยืนยัน (Accept) [F2]";
        btnAccept.Location = new Point(570, 355);
        btnAccept.Click += btnAccept_Click;

        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(730, 400);
        Controls.Add(lblBarcode);
        Controls.Add(txtBarcode);
        Controls.Add(gridSteps);
        Controls.Add(btnSave);
        Controls.Add(btnAccept);
        KeyPreview = true;
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Innovation.TotalWeight_PLC - ชั่งน้ำหนัก (portfolio clone)";
    }
}
