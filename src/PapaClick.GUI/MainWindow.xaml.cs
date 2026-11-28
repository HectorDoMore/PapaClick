using System.Globalization;
using System.Windows;
using System.Windows.Interop;
using PapaClick.Core;

namespace PapaClick.GUI;

/// <summary>
/// 主窗口：连点参数设置、开始/停止、全局热键启停、防检测抖动、当次/总计计数。
/// 偏好设置从 %AppData%\PapaClick\settings.json 加载，退出与停止时持久化。
/// </summary>
public partial class MainWindow : Window
{
    private const int HotkeyIdToggle = 1;

    private readonly ClickEngine _engine = new();
    private readonly System.Windows.Threading.DispatcherTimer _statusTimer = new();
    private HwndSource? _hwndSource;

    private AppSettings _settings = new();
    private long _baseTotalClicks;

    public MainWindow()
    {
        InitializeComponent();
        LoadSettings();
        ApplyWindowFromSettings();

        _statusTimer.Interval = TimeSpan.FromMilliseconds(200);
        _statusTimer.Tick += (_, _) => RefreshStatus();
        Loaded += (_, _) => RegisterHotkey();
        Closing += (_, _) => OnWindowClosing();
    }

    private void LoadSettings()
    {
        _settings = SettingsStore.Load();
        _baseTotalClicks = _settings.TotalClicks;
        ThemeService.Apply(_settings.Theme);
    }

    private void ApplyWindowFromSettings()
    {
        IntervalBox.Text = _settings.IntervalSeconds.ToString(CultureInfo.InvariantCulture);
        ButtonBox.SelectedIndex = _settings.MouseButton == "right" ? 1 : 0;
        ModeBox.SelectedIndex = _settings.ClickMode == "double" ? 1 : 0;
        JitterCheck.IsChecked = _settings.JitterEnabled;
        UpdateCounterText();
    }

    private void RegisterHotkey()
    {
        var handle = new WindowInteropHelper(this).Handle;
        _hwndSource = HwndSource.FromHwnd(handle);
        _hwndSource?.AddHook(WndProc);
        ReRegisterHotkey(handle);
    }

    private void ReRegisterHotkey(IntPtr handle)
    {
        HotkeyApi.UnregisterHotKey(handle, HotkeyIdToggle);
        if (!HotkeyApi.RegisterHotKey(handle, HotkeyIdToggle, (uint)_settings.HotkeyMods, (uint)_settings.HotkeyVk))
        {
            StatusText.Text = "全局热键注册失败（可能被其他程序占用），可在设置中更换热键";
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

        if (!double.TryParse(IntervalBox.Text.Trim(), CultureInfo.InvariantCulture, out var seconds)
            || seconds is < 0.001 or > 60)
        {
            StatusText.Text = "点击间隔无效：请输入 0.001 ~ 60 之间的数字（秒）";
            Core.DebugLog.Write($"GUI StartClicking 参数无效: \"{IntervalBox.Text}\"");
            return;
        }

        _engine.IntervalMs = Math.Max(1, (int)Math.Round(seconds * 1000));
        _engine.Button = ButtonBox.SelectedIndex == 1 ? MouseButton.Right : MouseButton.Left;
        _engine.Mode = ModeBox.SelectedIndex == 1 ? ClickMode.Double : ClickMode.Single;
        _engine.JitterEnabled = JitterCheck.IsChecked == true;
        _engine.JitterRangePx = Math.Clamp(_settings.JitterRangePx, 1, 50);

        Core.DebugLog.Write($"GUI StartClicking: seconds={seconds}");

        IntervalBox.IsEnabled = false;
        ButtonBox.IsEnabled = false;
        ModeBox.IsEnabled = false;
        JitterCheck.IsEnabled = false;
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
        PersistTotalClicks();

        IntervalBox.IsEnabled = true;
        ButtonBox.IsEnabled = true;
        ModeBox.IsEnabled = true;
        JitterCheck.IsEnabled = true;
        BtnStart.IsEnabled = true;
        BtnStop.IsEnabled = false;

        StatusText.Text = "已停止";
        UpdateCounterText();
    }

    private void RefreshStatus()
    {
        if (_engine.IsRunning)
        {
            StatusText.Text = "连点中";
            UpdateCounterText();
        }
    }

    private void UpdateCounterText()
    {
        CounterText.Text = $"当次 {_engine.SessionClicks} · 总计 {_baseTotalClicks + _engine.TotalClicks}";
    }

    /// <summary>把本进程的点击量并入历史总计并落盘。</summary>
    private void PersistTotalClicks()
    {
        _settings.TotalClicks = _baseTotalClicks + _engine.TotalClicks;
        _settings.IntervalSeconds = ParseIntervalOrDefault();
        _settings.MouseButton = ButtonBox.SelectedIndex == 1 ? "right" : "left";
        _settings.ClickMode = ModeBox.SelectedIndex == 1 ? "double" : "single";
        _settings.JitterEnabled = JitterCheck.IsChecked == true;
        SettingsStore.Save(_settings);
    }

    private double ParseIntervalOrDefault()
        => double.TryParse(IntervalBox.Text.Trim(), CultureInfo.InvariantCulture, out var v) && v is >= 0.001 and <= 60
            ? v
            : _settings.IntervalSeconds;

    private void OnWindowClosing()
    {
        _statusTimer.Stop();
        _engine.Stop();
        PersistTotalClicks();
        UnregisterHotkey();
    }

    /// <summary>设置窗口保存/导入后回调：应用主题、热键、并持久化。</summary>
    public void ApplySettings(AppSettings settings)
    {
        _settings = settings;
        _baseTotalClicks = settings.TotalClicks;
        ThemeService.Apply(settings.Theme);
        ApplyWindowFromSettings();

        var handle = new WindowInteropHelper(this).Handle;
        ReRegisterHotkey(handle);

        StatusText.Text = "设置已应用";
        UpdateCounterText();
    }

    private void OpenSettings_Click(object sender, RoutedEventArgs e)
    {
        var window = new SettingsWindow(_settings) { Owner = this };
        window.ShowDialog();
    }

    private void Exit_Click(object sender, RoutedEventArgs e) => Close();

    private void About_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show(
            "PapaClick v0.2\n\n一个轻量的 Windows 自动化工具。\n全局热键默认 F7 开始/停止，可在设置中自定义。\n本程序在 AI 协助下制作，基于 GPL-3.0 开源。",
            "关于 PapaClick",
            MessageBoxButton.OK,
            MessageBoxImage.Information);
    }
}
