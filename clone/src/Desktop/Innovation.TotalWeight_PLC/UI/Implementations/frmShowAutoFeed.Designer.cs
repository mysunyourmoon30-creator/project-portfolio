namespace Innovation.TotalWeight_PLC.UI.Implementations;

partial class frmShowAutoFeed
{
    private System.ComponentModel.IContainer components = null;
    private Label lblStatus;
    private Button btnClose;

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
        lblStatus = new Label();
        btnClose = new Button();

        lblStatus.Text = "กำลังป้อนวัตถุดิบอัตโนมัติ...";
        lblStatus.Location = new Point(20, 20);
        lblStatus.AutoSize = true;

        // Only needed for the failure paths - success closes the dialog
        // itself via CloseDialog(). Without this button, a failed auto-feed
        // would leave the operator with no way to dismiss the dialog at
        // all, which is its own kind of bug even though "must stay open"
        // is the correct behavior on failure.
        btnClose.Text = "ปิด [Esc]";
        btnClose.Location = new Point(20, 55);
        btnClose.Click += (_, _) => CloseDialog(DialogResult.Cancel);

        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(320, 100);
        Controls.Add(lblStatus);
        Controls.Add(btnClose);
        CancelButton = btnClose;
        StartPosition = FormStartPosition.CenterScreen;
        Text = "ป้อนวัตถุดิบอัตโนมัติ";
    }
}
