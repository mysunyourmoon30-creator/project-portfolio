namespace Innovation.TotalWeight_PLC.UI.Implementations;

partial class frmTotalWeight
{
    private System.ComponentModel.IContainer components = null;
    private Label lblBarcode;
    private TextBox txtBarcode;
    private DataGridView gridSteps;
    private Button btnSave;
    private Button btnAccept;
    private GroupBox grpAutoFeed;
    private Label lblAutoFeedBarcode;
    private TextBox txtAutoFeedBarcode;
    private Label lblAutoFeedLineId;
    private TextBox txtAutoFeedLineId;
    private Label lblAutoFeedPlanId;
    private TextBox txtAutoFeedPlanId;
    private Button btnAutoFeed;

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
        grpAutoFeed = new GroupBox();
        lblAutoFeedBarcode = new Label();
        txtAutoFeedBarcode = new TextBox();
        lblAutoFeedLineId = new Label();
        txtAutoFeedLineId = new TextBox();
        lblAutoFeedPlanId = new Label();
        txtAutoFeedPlanId = new TextBox();
        btnAutoFeed = new Button();

        lblBarcode.Text = "ยิงบาร์โค้ดคัมบัง";
        lblBarcode.Location = new Point(10, 12);
        lblBarcode.AutoSize = true;

        txtBarcode.Location = new Point(140, 9);
        txtBarcode.Width = 220;
        txtBarcode.KeyDown += txtBarcode_KeyDown;

        gridSteps.Location = new Point(10, 45);
        gridSteps.Size = new Size(700, 220);
        gridSteps.AutoGenerateColumns = true;
        gridSteps.AllowUserToAddRows = false;
        gridSteps.CellEndEdit += gridSteps_CellEndEdit;

        btnSave.Text = "บันทึก [F5]";
        btnSave.Location = new Point(460, 275);
        btnSave.Click += btnSave_Click;

        btnAccept.Text = "ยืนยัน (Accept) [F2]";
        btnAccept.Location = new Point(570, 275);
        btnAccept.Click += btnAccept_Click;

        // Demo trigger for the auto-feed flow (Presenter_ShowAutoFeed) -
        // the original real system reaches this from a barcode scan during
        // weighing; this clone exposes it directly so the "must not close
        // the form on failure" behavior can be walked through by hand.
        grpAutoFeed.Text = "ทดสอบ Auto-feed";
        grpAutoFeed.Location = new Point(10, 310);
        grpAutoFeed.Size = new Size(700, 80);

        lblAutoFeedBarcode.Text = "บาร์โค้ด";
        lblAutoFeedBarcode.Location = new Point(10, 25);
        lblAutoFeedBarcode.AutoSize = true;
        txtAutoFeedBarcode.Text = "RM001";
        txtAutoFeedBarcode.Location = new Point(70, 22);
        txtAutoFeedBarcode.Width = 100;

        lblAutoFeedLineId.Text = "Line ID";
        lblAutoFeedLineId.Location = new Point(190, 25);
        lblAutoFeedLineId.AutoSize = true;
        txtAutoFeedLineId.Text = "1";
        txtAutoFeedLineId.Location = new Point(250, 22);
        txtAutoFeedLineId.Width = 50;

        lblAutoFeedPlanId.Text = "Plan ID";
        lblAutoFeedPlanId.Location = new Point(320, 25);
        lblAutoFeedPlanId.AutoSize = true;
        txtAutoFeedPlanId.Text = "1";
        txtAutoFeedPlanId.Location = new Point(380, 22);
        txtAutoFeedPlanId.Width = 50;

        btnAutoFeed.Text = "ป้อนวัตถุดิบอัตโนมัติ";
        btnAutoFeed.Location = new Point(460, 20);
        btnAutoFeed.Click += btnAutoFeed_Click;

        grpAutoFeed.Controls.Add(lblAutoFeedBarcode);
        grpAutoFeed.Controls.Add(txtAutoFeedBarcode);
        grpAutoFeed.Controls.Add(lblAutoFeedLineId);
        grpAutoFeed.Controls.Add(txtAutoFeedLineId);
        grpAutoFeed.Controls.Add(lblAutoFeedPlanId);
        grpAutoFeed.Controls.Add(txtAutoFeedPlanId);
        grpAutoFeed.Controls.Add(btnAutoFeed);

        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(730, 400);
        Controls.Add(lblBarcode);
        Controls.Add(txtBarcode);
        Controls.Add(gridSteps);
        Controls.Add(btnSave);
        Controls.Add(btnAccept);
        Controls.Add(grpAutoFeed);
        KeyPreview = true;
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Innovation.TotalWeight_PLC - ชั่งน้ำหนัก (portfolio clone)";
    }
}
