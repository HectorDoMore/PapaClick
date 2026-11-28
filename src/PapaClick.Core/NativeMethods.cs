using System.Runtime.InteropServices;

namespace PapaClick.Core;

/// <summary>
/// Windows 鼠标输入 API（user32.SendInput）的 P/Invoke 封装。
/// </summary>
internal static class NativeMethods
{
    private const uint INPUT_MOUSE = 0;
    public const uint MOUSEEVENTF_LEFTDOWN = 0x0002;
    public const uint MOUSEEVENTF_LEFTUP = 0x0004;
    public const uint MOUSEEVENTF_RIGHTDOWN = 0x0008;
    public const uint MOUSEEVENTF_RIGHTUP = 0x0010;

    [StructLayout(LayoutKind.Sequential)]
    private struct MOUSEINPUT
    {
        public int dx;
        public int dy;
        public uint mouseData;
        public uint dwFlags;
        public uint time;
        public UIntPtr dwExtraInfo;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct KEYBDINPUT
    {
        public ushort wVk;
        public ushort wScan;
        public uint dwFlags;
        public uint time;
        public UIntPtr dwExtraInfo;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct HARDWAREINPUT
    {
        public uint uMsg;
        public ushort wParamL;
        public ushort wParamH;
    }

    [StructLayout(LayoutKind.Explicit)]
    private struct InputUnion
    {
        [FieldOffset(0)] public MOUSEINPUT mi;
        [FieldOffset(0)] public KEYBDINPUT ki;
        [FieldOffset(0)] public HARDWAREINPUT hi;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct INPUT
    {
        public uint type;
        public InputUnion U;
    }

    [DllImport("user32.dll", SetLastError = true)]
    private static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);

    [DllImport("winmm.dll")]
    private static extern uint timeBeginPeriod(uint uPeriod);

    [DllImport("winmm.dll")]
    private static extern uint timeEndPeriod(uint uPeriod);

    /// <summary>
    /// 把系统计时精度从默认 ~15.6ms 提升到 1ms，连点期间调用，停止后释放。
    /// </summary>
    public static void EnableHighResolutionTimer() => timeBeginPeriod(1);

    public static void DisableHighResolutionTimer() => timeEndPeriod(1);

    /// <summary>发送一个鼠标事件标志（按下或抬起）。失败返回 false 并带 Win32 错误码。</summary>
    public static bool TrySendMouseFlag(uint flag, out int lastError)
    {
        var input = new INPUT
        {
            type = INPUT_MOUSE,
            U = new InputUnion
            {
                mi = new MOUSEINPUT
                {
                    dx = 0,
                    dy = 0,
                    mouseData = 0,
                    dwFlags = flag,
                    time = 0,
                    dwExtraInfo = UIntPtr.Zero,
                },
            },
        };
        var inputs = new[] { input };
        var sent = SendInput((uint)inputs.Length, inputs, Marshal.SizeOf<INPUT>());
        if (sent != 1)
        {
            lastError = Marshal.GetLastWin32Error();
            return false;
        }
        lastError = 0;
        return true;
    }

    [DllImport("user32.dll")]
    private static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern int GetWindowText(IntPtr hWnd, System.Text.StringBuilder text, int count);

    /// <summary>取前台窗口标题（诊断用）。</summary>
    public static string GetForegroundWindowTitle()
    {
        try
        {
            var sb = new System.Text.StringBuilder(256);
            _ = GetWindowText(GetForegroundWindow(), sb, 256);
            return sb.ToString();
        }
        catch
        {
            return "";
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int X;
        public int Y;
    }

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetCursorPos(out POINT lpPoint);

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool SetCursorPos(int x, int y);

    /// <summary>取当前光标位置，失败返回 null。</summary>
    public static POINT? TryGetCursorPos()
        => GetCursorPos(out var p) ? p : null;

    /// <summary>移动光标到绝对屏幕坐标。</summary>
    public static bool TrySetCursorPos(int x, int y) => SetCursorPos(x, y);
}
