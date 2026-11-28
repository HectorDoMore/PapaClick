using System.Windows;
using System.Windows.Interop;
using PapaClick.Core;

namespace PapaClick.GUI;

/// <summary>
/// 主窗口：连点参数设置、开始/停止、全局热键 F7 启停。
/// </summary>
public partial class MainWindow : Window
{
    private const int HotkeyIdToggle = 1;

    private readonly ClickEngine _engine = new();
    private readonly System.Windows.Threading.DispatcherTimer _statusTimer = new();
    private HwndSource? _hwndSource;

    public MainWindow()
    {
        InitializeComponent();
        _statusTimer.Interval = TimeSpan.FromMilliseconds(200);
        _statusTimer.Tick += (_, _) => RefreshStatus();
        Loaded += (_, _) => RegisterHotkey();
        Closing += (_, _) =>
        {
            _statusTimer.Stop();
            _engine.Stop();
            UnregisterHotkey();
        };
    }

    private void RegisterHotkey()
    {
        var handle = new WindowInteropHelper(this).Handle;
        _hwndSource = HwndSource.FromHwnd(handle);
        _hwndSource?.AddHook(WndProc);

        if (!HotkeyApi.RegisterHotKey(handle, HotkeyIdToggle, HotkeyApi.MOD_NONE, HotkeyApi.VK_F7))
        {
            StatusText.Text = "全局热键 F7 注册失败（可能被其他程序占用）";
        }
    }

    private void UnregisterHotkey()
    {
        var handle = new WindowInteropHelper(this).Handle;
        HotkeyApi.UnregisterHotKey(handle, HotkeyIdToggle);
    }

    private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        if (msg == HotkeyApi.WM_HOTKEY && wParam.ToInt32() == HotkeyIdToggle)
        {
            Toggle();
            handled = true;
        }
        return IntPtr.Zero;
    }

    private void Toggle()
    {
        if (_engine.IsRunning)
        {
            StopClicking();
        }
        else
        {
            StartClicking();
        }
    }

    private void BtnStart_Click(object sender, RoutedEventArgs e) => StartClicking();

    private void BtnStop_Click(object sender, RoutedEventArgs e) => StopClicking();

    private void StartClicking()
    {
        if (_engine.IsRunning)
        {
            return;
        }

        if (!double.TryParse(IntervalBox.Text.Trim(), System.Globalization.CultureInfo.InvariantCulture, out var seconds)
            || seconds is < 0.001 or > 60)
        {
            StatusText.Text = "点击间隔无效：请输入 0.001 ~ 60 之间的数字（秒）";
            return;
        }

        _engine.IntervalMs = Math.Max(1, (int)Math.Round(seconds * 1000));
        _engine.Button = ButtonBox.SelectedIndex == 1 ? MouseButton.Right : MouseButton.Left;
        _engine.Mode = ModeBox.SelectedIndex == 1 ? ClickMode.Double : ClickMode.Single;

        IntervalBox.IsEnabled = false;
        ButtonBox.IsEnabled = false;
        ModeBox.IsEnabled = false;
        BtnStart.IsEnabled = false;
        BtnStop.IsEnabled = true;

        _engine.Start();
        _statusTimer.Start();
    }

    private void StopClicking()
    {
        if (!_engine.IsRunning)
        {
            return;
        }

        _engine.Stop();
        _statusTimer.Stop();

        IntervalBox.IsEnabled = true;
        ButtonBox.IsEnabled = true;
        ModeBox.IsEnabled = true;
        BtnStart.IsEnabled = true;
        BtnStop.IsEnabled = false;

        StatusText.Text = $"已停止 · 共点击 {_engine.TotalClicks} 次";
    }

    private void RefreshStatus()
    {
        if (_engine.IsRunning)
        {
            StatusText.Text = $"连点中 · 已点击 {_engine.TotalClicks} 次";
        }
    }

    private void OpenSettings_Click(object sender, RoutedEventArgs e)
    {
        var window = new SettingsWindow { Owner = this };
        window.ShowDialog();
    }

    private void Exit_Click(object sender, RoutedEventArgs e) => Close();

    private void About_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show(
            "PapaClick v0.2\n\n一个轻量的 Windows 自动化工具。\n全局热键：F7 开始/停止切换。\n本程序在 AI 协助下制作，基于 GPL-3.0 开源。",
            "关于 PapaClick",
            MessageBoxButton.OK,
            MessageBoxImage.Information);
    }
}
