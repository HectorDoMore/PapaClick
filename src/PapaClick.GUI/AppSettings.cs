using System.IO;
using System.Text.Json;

namespace PapaClick.GUI;

/// <summary>
/// 应用偏好设置。持久化于 %AppData%\PapaClick\settings.json，支持导入导出。
/// </summary>
public sealed class AppSettings
{
    public string Theme { get; set; } = "System";          // Light / Dark / System
    public int HotkeyVk { get; set; } = 0x76;              // 默认 F7
    public int HotkeyMods { get; set; } = 0;               // MOD_NONE
    public double IntervalSeconds { get; set; } = 0.001;
    public string MouseButton { get; set; } = "left";
    public string ClickMode { get; set; } = "single";
    public bool JitterEnabled { get; set; }
    public int JitterRangePx { get; set; } = 3;
    public long TotalClicks { get; set; }                  // 历史累计点击（跨会话持久化）
}

/// <summary>
/// 设置存取：默认文件 %AppData%\PapaClick\settings.json。
/// </summary>
public static class SettingsStore
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
    };

    public static string DefaultPath
    {
        get
        {
            var dir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "PapaClick");
            Directory.CreateDirectory(dir);
            return Path.Combine(dir, "settings.json");
        }
    }

    public static AppSettings Load()
    {
        try
        {
            if (File.Exists(DefaultPath))
            {
                var json = File.ReadAllText(DefaultPath);
                var settings = JsonSerializer.Deserialize<AppSettings>(json, JsonOptions);
                if (settings != null)
                {
                    return settings;
                }
            }
        }
        catch
        {
            // 配置损坏时回退到默认值，不阻塞启动
        }
        return new AppSettings();
    }

    public static void Save(AppSettings settings)
        => File.WriteAllText(DefaultPath, JsonSerializer.Serialize(settings, JsonOptions));

    public static void Export(AppSettings settings, string path)
        => File.WriteAllText(path, JsonSerializer.Serialize(settings, JsonOptions));

    /// <summary>从外部 JSON 导入；格式非法或缺少文件时返回 null。</summary>
    public static AppSettings? TryImport(string path)
    {
        try
        {
            if (!File.Exists(path))
            {
                return null;
            }
            var settings = JsonSerializer.Deserialize<AppSettings>(File.ReadAllText(path), JsonOptions);
            if (settings != null)
            {
                Save(settings);
            }
            return settings;
        }
        catch
        {
            return null;
        }
    }
}
