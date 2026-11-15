namespace PapaClick.GUI;

/// <summary>
/// 主窗口：菜单栏、参数设置区、开始/停止按钮、状态栏，已接入 ClickEngine。
/// </summary>
public partial class MainForm : Form
{
    private readonly PapaClick.Core.ClickEngine _engine = new();

    /// <summary>界面计时器：定时刷新点击计数，避免后台线程直接碰控件。</summary>
    private readonly System.Windows.Forms.Timer _statusTimer = new();

    public MainForm()
    {
        InitializeComponent();
        _statusTimer.Interval = 200;
        _statusTimer.Tick += (_, _) => RefreshStatus();
        FormClosing += (_, _) =>
        {
            _statusTimer.Stop();
            _engine.Stop();
        };
    }

    private void tsmFileExit_Click(object? sender, System.EventArgs e)
    {
        Close();
    }

    private void tsmHelpAbout_Click(object? sender, System.EventArgs e)
    {
        MessageBox.Show(
            "PapaClick v0.1\n\n一个轻量的 Windows 连点器。\n本程序在 AI 协助下制作，基于 GPL-3.0 开源。",
            "关于 PapaClick",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
    }

    private void btnStart_Click(object? sender, System.EventArgs e)
    {
        if (_engine.IsRunning)
        {
            return;
        }

        _engine.IntervalMs = (int)numInterval.Value;
        _engine.Button = cboButton.SelectedIndex == 1
            ? PapaClick.Core.MouseButton.Right
            : PapaClick.Core.MouseButton.Left;
        _engine.Mode = cboClickType.SelectedIndex == 1
            ? PapaClick.Core.ClickMode.Double
            : PapaClick.Core.ClickMode.Single;

        // 界面参数在连点期间锁定，避免边点边改造成状态混乱
        numInterval.Enabled = false;
        cboButton.Enabled = false;
        cboClickType.Enabled = false;
        btnStart.Enabled = false;
        btnStop.Enabled = true;

        _engine.Start();
        _statusTimer.Start();
    }

    private void btnStop_Click(object? sender, System.EventArgs e)
    {
        if (!_engine.IsRunning)
        {
            return;
        }

        _engine.Stop();
        _statusTimer.Stop();

        numInterval.Enabled = true;
        cboButton.Enabled = true;
        cboClickType.Enabled = true;
        btnStart.Enabled = true;
        btnStop.Enabled = false;

        lblStatus.Text = $"已停止（共点击 {_engine.TotalClicks} 次）";
    }

    private void RefreshStatus()
    {
        if (_engine.IsRunning)
        {
            lblStatus.Text = $"连点中 · 已点击 {_engine.TotalClicks} 次";
        }
    }
}
