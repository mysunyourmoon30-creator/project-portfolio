namespace Innovation.TotalWeight_PLC.UI.Implementations;

partial class frmSelectKB
{
    private System.ComponentModel.IContainer components = null;
    private DataGridView gridKanbans;
    private Button btnOk;
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
        gridKanbans = new DataGridView();
        btnOk = new Button();
        btnCancel = new Button();

        gridKanbans.Location = new Point(20, 20);
        gridKanbans.Size = new Size(520, 318);
        gridKanbans.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        gridKanbans.AutoGenerateColumns = true;
        gridKanbans.ReadOnly = true;
        gridKanbans.AllowUserToAddRows = false;
        gridKanbans.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

        btnOk.Text = "ตกลง [F5]";
        btnOk.Location = new Point(290, 348);
        btnOk.Size = new Size(120, 36);
        btnOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        btnOk.Click += btnOk_Click;

        btnCancel.Text = "ยกเลิก [Esc]";
        btnCancel.Location = new Point(420, 348);
        btnCancel.Size = new Size(120, 36);
        btnCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        btnCancel.Click += btnCancel_Click;

        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(560, 400);
        Controls.Add(gridKanbans);
        Controls.Add(btnOk);
        Controls.Add(btnCancel);
        AcceptButton = btnOk;
        CancelButton = btnCancel;
        MinimumSize = new Size(420, 320);
        StartPosition = FormStartPosition.CenterScreen;
        Text = "เลือกคัมบัง";
    }
}
