using Innovation.TotalWeight_PLC.Interfaces.Presenters;
using Innovation.TotalWeight_PLC.Interfaces.Views;

namespace Innovation.TotalWeight_PLC.UI.Implementations;

public partial class frmMain : Form, IView_Main
{
    public frmMain()
    {
        InitializeComponent();
    }

    // Main-window Run() is intentionally a no-op: the message loop is
    // started once, explicitly, by Program.cs calling
    // Application.Run(mainForm) - unlike the original app, which never
    // called Application.Run() at all and relied on nested ShowDialog()
    // calls instead. Dialog views (added in Phase 4) call ShowDialog() here.
    public void Run()
    {
    }
}
