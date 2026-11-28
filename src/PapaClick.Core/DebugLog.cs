using System.IO;

namespace PapaClick.Core;

/// <summary>
/// 轻量诊断日志：写入 %AppData%\PapaClick\log.txt。发布版可关。
/// </summary>
public static class DebugLog
{
    private static readonly object Gate = new();

    public static string LogPath { get; } = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "PapaClick", "log.txt");

    public static void Write(string message)
    {
        try
        {
            lock (Gate)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(LogPath)!);
                File.AppendAllText(LogPath, $"{DateTime.Now:HH:mm:ss.fff}  {message}\r\n");
            }
        }
        catch
        {
            // 日志失败不影响主流程
        }
    }
}
