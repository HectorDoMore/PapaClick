using System.Runtime.InteropServices;

namespace PapaClick.GUI;

/// <summary>
/// 全局热键注册 API（user32.RegisterHotKey）。
/// 热键注册到指定窗口句柄，触发时以 WM_HOTKEY 消息送达该窗口。
/// </summary>
internal static class HotkeyApi
{
    public const int WM_HOTKEY = 0x0312;

    public const int MOD_NONE = 0x0000;
    public const uint VK_F5 = 0x74;
    public const uint VK_F6 = 0x75;

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool UnregisterHotKey(IntPtr hWnd, int id);
}
