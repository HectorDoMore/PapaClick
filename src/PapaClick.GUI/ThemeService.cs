using System.Windows;
using Microsoft.Win32;

namespace PapaClick.GUI;

/// <summary>
/// 主题服务：在 Light / Dark / 跟随系统之间切换（替换应用级画刷资源字典）。
/// </summary>
public static class ThemeService
{
    public static string Current { get; private set; } = "System";

    static ThemeService()
    {
        // 跟随系统模式下，操作系统切换深浅色时实时跟随
        SystemEvents.UserPreferenceChanged += (_, e) =>
        {
            if (e.Category == UserPreferenceCategory.General && Current == "System")
            {
                Apply("System");
            }
        };
    }

    public static void Apply(string theme)
    {
        Current = theme;
        var source = theme switch
        {
            "Light" => "Themes/Light.xaml",
            "Dark" => "Themes/Dark.xaml",
            _ => IsSystemLight() ? "Themes/Light.xaml" : "Themes/Dark.xaml",
        };

        var dict = new ResourceDictionary { Source = new Uri(source, UriKind.Relative) };
        var merged = Application.Current.Resources.MergedDictionaries;
        if (merged.Count > 0 && merged[0].Source?.OriginalString.Contains("Themes/") == true)
        {
            merged[0] = dict;
        }
        else
        {
            merged.Insert(0, dict);
        }
    }

    private static bool IsSystemLight()
    {
        using var key = Registry.CurrentUser.OpenSubKey(
            @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize");
        var value = key?.GetValue("AppsUseLightTheme");
        return value is null || Convert.ToInt32(value) == 1;
    }
}
