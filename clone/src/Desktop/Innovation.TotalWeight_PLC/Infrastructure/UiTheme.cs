using System.Reflection;

namespace Innovation.TotalWeight_PLC.Infrastructure;

// Flat blue/white theme applied uniformly across all 4 forms - centralized
// here (alongside MessageBoxIconMapper) so no form duplicates color/font
// literals. Purely visual: nothing here touches data binding, hotkeys, or
// the Actual-column-only edit lock in frmTotalWeight.
public static class UiTheme
{
    public static readonly Color PrimaryBlue = ColorTranslator.FromHtml("#1565C0");
    public static readonly Color PrimaryBlueDark = ColorTranslator.FromHtml("#0D47A1");
    public static readonly Color Background = ColorTranslator.FromHtml("#F4F7FB");
    public static readonly Color Surface = Color.White;
    public static readonly Color Border = ColorTranslator.FromHtml("#CBD5E1");
    public static readonly Color TextPrimary = ColorTranslator.FromHtml("#1F2933");
    public static readonly Color TextSecondary = ColorTranslator.FromHtml("#5A6B7B");
    public static readonly Color SuccessGreen = ColorTranslator.FromHtml("#2E7D32");
    public static readonly Color NeutralButton = ColorTranslator.FromHtml("#E4E9F0");
    public static readonly Color NeutralButtonText = ColorTranslator.FromHtml("#37474F");
    public static readonly Color GridAltRow = ColorTranslator.FromHtml("#EAF2FB");
    public static readonly Color GridSelectionBack = ColorTranslator.FromHtml("#BBDEFB");

    public static readonly Font BaseFont = new("Segoe UI", 9.5f, FontStyle.Regular);
    public static readonly Font ButtonFont = new("Segoe UI", 9.5f, FontStyle.Bold);
    public static readonly Font GridHeaderFont = new("Segoe UI", 9.5f, FontStyle.Bold);

    public enum ButtonKind { Primary, Success, Secondary }

    private static readonly PropertyInfo? DoubleBufferedProp =
        typeof(DataGridView).GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);

    public static void ApplyForm(Form form)
    {
        form.BackColor = Background;
        form.Font = BaseFont;
    }

    public static void StyleButton(Button button, ButtonKind kind)
    {
        button.FlatStyle = FlatStyle.Flat;
        button.UseVisualStyleBackColor = false;
        button.Font = ButtonFont;
        button.Cursor = Cursors.Hand;
        button.FlatAppearance.BorderColor = Border;

        switch (kind)
        {
            case ButtonKind.Primary:
                button.BackColor = PrimaryBlue;
                button.ForeColor = Color.White;
                button.FlatAppearance.BorderSize = 0;
                button.FlatAppearance.MouseOverBackColor = PrimaryBlueDark;
                button.FlatAppearance.MouseDownBackColor = PrimaryBlueDark;
                break;
            case ButtonKind.Success:
                button.BackColor = SuccessGreen;
                button.ForeColor = Color.White;
                button.FlatAppearance.BorderSize = 0;
                button.FlatAppearance.MouseOverBackColor = ControlPaint.Dark(SuccessGreen, 0.1f);
                button.FlatAppearance.MouseDownBackColor = ControlPaint.Dark(SuccessGreen, 0.1f);
                break;
            case ButtonKind.Secondary:
                button.BackColor = NeutralButton;
                button.ForeColor = NeutralButtonText;
                button.FlatAppearance.BorderSize = 1;
                button.FlatAppearance.MouseOverBackColor = ControlPaint.Dark(NeutralButton, 0.05f);
                button.FlatAppearance.MouseDownBackColor = ControlPaint.Dark(NeutralButton, 0.1f);
                break;
        }
    }

    public static void StyleTextBox(TextBox textBox)
    {
        textBox.BorderStyle = BorderStyle.FixedSingle;
        textBox.Font = BaseFont;
        textBox.BackColor = Surface;
        textBox.ForeColor = TextPrimary;
    }

    public static void StyleGrid(DataGridView grid)
    {
        grid.EnableHeadersVisualStyles = false;
        grid.BorderStyle = BorderStyle.None;
        grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
        grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
        grid.BackgroundColor = Surface;
        grid.GridColor = Border;
        grid.RowHeadersVisible = false;
        grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
        grid.ColumnHeadersHeight = 34;
        grid.RowTemplate.Height = 30;
        grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

        grid.ColumnHeadersDefaultCellStyle.BackColor = PrimaryBlue;
        grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
        grid.ColumnHeadersDefaultCellStyle.Font = GridHeaderFont;
        grid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
        grid.ColumnHeadersDefaultCellStyle.Padding = new Padding(6, 0, 0, 0);

        grid.DefaultCellStyle.BackColor = Surface;
        grid.DefaultCellStyle.ForeColor = TextPrimary;
        grid.DefaultCellStyle.Font = BaseFont;
        grid.DefaultCellStyle.SelectionBackColor = GridSelectionBack;
        grid.DefaultCellStyle.SelectionForeColor = PrimaryBlueDark;
        grid.DefaultCellStyle.Padding = new Padding(4, 0, 0, 0);

        grid.AlternatingRowsDefaultCellStyle.BackColor = GridAltRow;
        grid.AlternatingRowsDefaultCellStyle.SelectionBackColor = GridSelectionBack;
        grid.AlternatingRowsDefaultCellStyle.SelectionForeColor = PrimaryBlueDark;

        EnableDoubleBuffering(grid);
    }

    public static void EnableDoubleBuffering(DataGridView grid) =>
        DoubleBufferedProp?.SetValue(grid, true);
}
