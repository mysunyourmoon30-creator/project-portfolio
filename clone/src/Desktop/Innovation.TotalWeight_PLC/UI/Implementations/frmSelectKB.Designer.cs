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

        gridKanbans.Location = new Point(10, 10);
        gridKanbans.Size = new Size(480, 260);
        gridKanbans.AutoGenerateColumns = true;
        gridKanbans.ReadOnly = true;
        gridKanbans.AllowUserToAddRows = false;
        gridKanbans.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

        btnOk.Text = "ตกลง [F5]";
        btnOk.Location = new Point(315, 280);
        btnOk.Click += btnOk_Click;

        btnCancel.Text = "ยกเลิก [Esc]";
        btnCancel.Location = new Point(415, 280);
        btnCancel.Click += btnCancel_Click;

        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(505, 330);
        Controls.Add(gridKanbans);
        Controls.Add(btnOk);
        Controls.Add(btnCancel);
        AcceptButton = btnOk;
        CancelButton = btnCancel;
        StartPosition = FormStartPosition.CenterScreen;
        Text = "เลือกคัมบัง";
    }
}
