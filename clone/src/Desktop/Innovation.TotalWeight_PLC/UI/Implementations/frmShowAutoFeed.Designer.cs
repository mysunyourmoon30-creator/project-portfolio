namespace Innovation.TotalWeight_PLC.UI.Implementations;

partial class frmShowAutoFeed
{
    private System.ComponentModel.IContainer components = null;
    private Label lblStatus;

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

        lblStatus.Text = "กำลังป้อนวัตถุดิบอัตโนมัติ...";
        lblStatus.Location = new Point(20, 20);
        lblStatus.AutoSize = true;

        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(320, 100);
        Controls.Add(lblStatus);
        StartPosition = FormStartPosition.CenterScreen;
        Text = "ป้อนวัตถุดิบอัตโนมัติ";
    }
}
