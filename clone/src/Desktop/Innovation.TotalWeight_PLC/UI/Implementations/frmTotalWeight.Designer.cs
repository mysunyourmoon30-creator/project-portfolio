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
    private Button btnSelectKanban;

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
        btnSelectKanban = new Button();

        lblBarcode.Text = "ยิงบาร์โค้ดคัมบัง";
        lblBarcode.Location = new Point(20, 24);
        lblBarcode.AutoSize = true;

        txtBarcode.Location = new Point(140, 18);
        txtBarcode.Size = new Size(360, 28);
        txtBarcode.Anchor = AnchorStyles.Top | AnchorStyles.Left;
        txtBarcode.KeyDown += txtBarcode_KeyDown;

        // Opens Presenter_SelectKB via IPresenter_TotalWeight.SelectKanbanAsync -
        // the original real system reaches kanban selection from a grid of
        // pending work, not just a direct barcode scan; this exposes that
        // path too so it isn't dead code that's only ever exercised by tests.
        btnSelectKanban.Text = "เลือกคัมบัง...";
        btnSelectKanban.Location = new Point(730, 15);
        btnSelectKanban.Size = new Size(150, 32);
        btnSelectKanban.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnSelectKanban.Click += btnSelectKanban_Click;

        gridSteps.Location = new Point(20, 60);
        gridSteps.Size = new Size(860, 394);
        gridSteps.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        gridSteps.AutoGenerateColumns = true;
        gridSteps.AllowUserToAddRows = false;
        gridSteps.CellEndEdit += gridSteps_CellEndEdit;

        btnSave.Text = "บันทึก [F5]";
        btnSave.Location = new Point(550, 464);
        btnSave.Size = new Size(130, 36);
        btnSave.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        btnSave.Click += btnSave_Click;

        btnAccept.Text = "ยืนยัน (Accept) [F2]";
        btnAccept.Location = new Point(690, 464);
        btnAccept.Size = new Size(190, 36);
        btnAccept.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        btnAccept.Click += btnAccept_Click;

        // Demo trigger for the auto-feed flow (Presenter_ShowAutoFeed) -
        // the original real system reaches this from a barcode scan during
        // weighing; this clone exposes it directly so the "must not close
        // the form on failure" behavior can be walked through by hand.
        grpAutoFeed.Text = "ทดสอบ Auto-feed";
        grpAutoFeed.Location = new Point(20, 510);
        grpAutoFeed.Size = new Size(860, 90);
        grpAutoFeed.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

        lblAutoFeedBarcode.Text = "บาร์โค้ด";
        lblAutoFeedBarcode.Location = new Point(15, 34);
        lblAutoFeedBarcode.AutoSize = true;
        txtAutoFeedBarcode.Text = "RM001";
        txtAutoFeedBarcode.Location = new Point(80, 30);
        txtAutoFeedBarcode.Size = new Size(110, 26);

        lblAutoFeedLineId.Text = "Line ID";
        lblAutoFeedLineId.Location = new Point(210, 34);
        lblAutoFeedLineId.AutoSize = true;
        txtAutoFeedLineId.Text = "1";
        txtAutoFeedLineId.Location = new Point(270, 30);
        txtAutoFeedLineId.Size = new Size(60, 26);

        lblAutoFeedPlanId.Text = "Plan ID";
        lblAutoFeedPlanId.Location = new Point(350, 34);
        lblAutoFeedPlanId.AutoSize = true;
        txtAutoFeedPlanId.Text = "1";
        txtAutoFeedPlanId.Location = new Point(410, 30);
        txtAutoFeedPlanId.Size = new Size(60, 26);

        btnAutoFeed.Text = "ป้อนวัตถุดิบอัตโนมัติ";
        btnAutoFeed.Location = new Point(660, 27);
        btnAutoFeed.Size = new Size(180, 34);
        btnAutoFeed.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnAutoFeed.Click += btnAutoFeed_Click;

        grpAutoFeed.Controls.Add(lblAutoFeedBarcode);
        grpAutoFeed.Controls.Add(txtAutoFeedBarcode);
        grpAutoFeed.Controls.Add(lblAutoFeedLineId);
        grpAutoFeed.Controls.Add(txtAutoFeedLineId);
        grpAutoFeed.Controls.Add(lblAutoFeedPlanId);
        grpAutoFeed.Controls.Add(txtAutoFeedPlanId);
        grpAutoFeed.Controls.Add(btnAutoFeed);

        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(900, 620);
        MinimumSize = new Size(760, 500);
        Controls.Add(lblBarcode);
        Controls.Add(txtBarcode);
        Controls.Add(btnSelectKanban);
        Controls.Add(gridSteps);
        Controls.Add(btnSave);
        Controls.Add(btnAccept);
        Controls.Add(grpAutoFeed);
        KeyPreview = true;
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Innovation.TotalWeight_PLC - ชั่งน้ำหนัก (portfolio clone)";
    }
}
