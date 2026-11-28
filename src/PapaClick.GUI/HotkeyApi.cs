using System.Runtime.InteropServices;

namespace PapaClick.GUI;

/// <summary>
/// 全局热键注册 API（user32.RegisterHotKey）。
/// </summary>
internal static class HotkeyApi
{
    public const int WM_HOTKEY = 0x0312;
    public const int MOD_NONE = 0x0000;
    public const uint VK_F7 = 0x76;

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool UnregisterHotKey(IntPtr hWnd, int id);
}
