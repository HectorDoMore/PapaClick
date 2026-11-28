using System.Windows;

namespace PapaClick.GUI;

/// <summary>
/// 设置窗口骨架。实际读写 %AppData%\PapaClick\settings.json 于阶段 1 实现。
/// </summary>
public partial class SettingsWindow : Window
{
    public SettingsWindow()
    {
        InitializeComponent();
    }

    private void Close_Click(object sender, RoutedEventArgs e) => Close();
}
