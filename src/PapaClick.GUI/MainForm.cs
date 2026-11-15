namespace PapaClick.GUI;

/// <summary>
/// 主窗口。目前只有界面骨架：菜单栏、参数设置区、开始/停止按钮、状态栏。
/// 连点功能待后续版本接入 ClickEngine 后启用。
/// </summary>
public partial class MainForm : Form
{
    public MainForm()
    {
        InitializeComponent();
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
        lblStatus.Text = "连点功能开发中，敬请期待";
    }

    private void btnStop_Click(object? sender, System.EventArgs e)
    {
        lblStatus.Text = "就绪";
    }
}
