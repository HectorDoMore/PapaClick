using System.Globalization;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;

namespace PapaClick.GUI;

/// <summary>
/// 设置窗口：主题预览、热键捕获、抖动幅度、配置导入导出与持久化。
/// </summary>
public partial class SettingsWindow : Window
{
    // RegisterHotKey 修饰键位
    private const int ModFlagAlt = 0x1;
    private const int ModFlagControl = 0x2;
    private const int ModFlagShift = 0x4;

    private static readonly HashSet<Key> ModifierOnlyKeys =
        new()
        {
            Key.LeftCtrl, Key.RightCtrl, Key.LeftAlt, Key.RightAlt,
            Key.LeftShift, Key.RightShift, Key.LWin, Key.RWin, Key.Escape,
        };

    private AppSettings _working;
    private int _hotkeyVk;
    private int _hotkeyMods;

    public SettingsWindow(AppSettings current)
    {
        InitializeComponent();
        _working = CloneSettings(current);
        _hotkeyVk = _working.HotkeyVk;
        _hotkeyMods = _working.HotkeyMods;
        LoadToUi();
    }

    private static AppSettings CloneSettings(AppSettings s)
        => new()
        {
            Theme = s.Theme,
            HotkeyVk = s.HotkeyVk,
            HotkeyMods = s.HotkeyMods,
            IntervalSeconds = s.IntervalSeconds,
            MouseButton = s.MouseButton,
            ClickMode = s.ClickMode,
            JitterEnabled = s.JitterEnabled,
            JitterRangePx = s.JitterRangePx,
            TotalClicks = s.TotalClicks,
        };

    private void LoadToUi()
    {
        switch (_working.Theme)
        {
            case "Light": ThemeLight.IsChecked = true; break;
            case "Dark": ThemeDark.IsChecked = true; break;
            default: ThemeSystem.IsChecked = true; break;
        }

        ModCtrl.IsChecked = (_hotkeyMods & ModFlagControl) != 0;
        ModAlt.IsChecked = (_hotkeyMods & ModFlagAlt) != 0;
        ModShift.IsChecked = (_hotkeyMods & ModFlagShift) != 0;
        RefreshHotkeyDisplay();

        JitterRangeBox.Text = _working.JitterRangePx.ToString(CultureInfo.InvariantCulture);
        TotalClicksText.Text = $"历史累计点击：{_working.TotalClicks}";
    }

    private void RefreshHotkeyDisplay()
    {
        var mods = new List<string>();
        if ((_hotkeyMods & ModFlagControl) != 0) mods.Add("Ctrl");
        if ((_hotkeyMods & ModFlagAlt) != 0) mods.Add("Alt");
        if ((_hotkeyMods & ModFlagShift) != 0) mods.Add("Shift");
        var keyName = ((Key)KeyInterop.KeyFromVirtualKey(_hotkeyVk)).ToString();
        HotkeyBox.Text = mods.Count > 0 ? $"{string.Join("+", mods)}+{keyName}" : keyName;
    }

    private void Theme_Changed(object sender, RoutedEventArgs e)
    {
        // 即时预览（不落盘）
        var theme = ThemeLight.IsChecked == true ? "Light"
            : ThemeDark.IsChecked == true ? "Dark"
            : "System";
        _working.Theme = theme;
        ThemeService.Apply(theme);
    }

    private void HotkeyBox_GotFocus(object sender, RoutedEventArgs e)
    {
        HotkeyBox.Background = System.Windows.Media.Brushes.Transparent;
        HotkeyBox.Text = "按下任意键…";
    }

    private void HotkeyBox_LostFocus(object sender, RoutedEventArgs e) => RefreshHotkeyDisplay();

    private void HotkeyBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape)
        {
            RefreshHotkeyDisplay();
            HotkeyBox.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            e.Handled = true;
            return;
        }

        var key = e.Key == Key.System ? e.SystemKey : e.Key;
        if (ModifierOnlyKeys.Contains(key))
        {
            e.Handled = true;
            return;
        }

        _hotkeyVk = KeyInterop.VirtualKeyFromKey(key);
        _hotkeyMods =
            (ModCtrl.IsChecked == true ? ModFlagControl : 0) |
            (ModAlt.IsChecked == true ? ModFlagAlt : 0) |
            (ModShift.IsChecked == true ? ModFlagShift : 0);
        RefreshHotkeyDisplay();
        e.Handled = true;
    }

    private bool ValidateAndCollect()
    {
        if (!int.TryParse(JitterRangeBox.Text.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var range)
            || range is < 1 or > 50)
        {
            MessageBox.Show("抖动幅度需为 1 ~ 50 的整数（像素）。", "提示",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        if (_hotkeyVk <= 0)
        {
            MessageBox.Show("热键无效，请重新设置。", "提示",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        _working.HotkeyVk = _hotkeyVk;
        _working.HotkeyMods =
            (ModCtrl.IsChecked == true ? ModFlagControl : 0) |
            (ModAlt.IsChecked == true ? ModFlagAlt : 0) |
            (ModShift.IsChecked == true ? ModFlagShift : 0);
        _working.JitterRangePx = range;
        return true;
    }

    private void Save_Click(object sender, RoutedEventArgs e)
    {
        if (!ValidateAndCollect())
        {
            return;
        }

        SettingsStore.Save(_working);
        if (Owner is MainWindow main)
        {
            main.ApplySettings(_working);
        }
        Close();
    }

    private void Import_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFileDialog
        {
            Title = "导入 PapaClick 配置",
            Filter = "JSON 文件 (*.json)|*.json",
        };
        if (dialog.ShowDialog(this) != true)
        {
            return;
        }

        var imported = SettingsStore.TryImport(dialog.FileName);
        if (imported == null)
        {
            MessageBox.Show("导入失败：文件不存在或格式不正确。", "错误",
                MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        _working = imported;
        _hotkeyVk = _working.HotkeyVk;
        _hotkeyMods = _working.HotkeyMods;
        LoadToUi();
        if (Owner is MainWindow main)
        {
            main.ApplySettings(_working);
        }
        MessageBox.Show("导入成功，配置已应用。", "提示",
            MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void Export_Click(object sender, RoutedEventArgs e)
    {
        if (!ValidateAndCollect())
        {
            return;
        }

        var dialog = new SaveFileDialog
        {
            Title = "导出 PapaClick 配置",
            Filter = "JSON 文件 (*.json)|*.json",
            FileName = "papaclick-settings.json",
        };
        if (dialog.ShowDialog(this) != true)
        {
            return;
        }

        SettingsStore.Export(_working, dialog.FileName);
        MessageBox.Show($"已导出到：{dialog.FileName}", "提示",
            MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void Close_Click(object sender, RoutedEventArgs e)
    {
        // 关闭不做半套保存；主题预览还原为主窗口当前生效主题
        ThemeService.Apply(_working.Theme);
        Close();
    }
}
