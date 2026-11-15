namespace PapaClick.Core;

/// <summary>
/// 基础连点引擎：按设定间隔向当前光标位置发送鼠标点击。
/// Start/Stop 可反复调用；参数（间隔、按键、方式）在 Start 时生效。
/// Clicked 事件在后台线程触发，界面订阅时需自行切换到 UI 线程。
/// </summary>
public sealed class ClickEngine
{
    /// <summary>双击时两次单击之间的间隔（毫秒）。</summary>
    private const int DoubleClickGapMs = 40;

    private System.Threading.Timer? _timer;

    private long _totalClicks;

    /// <summary>点击间隔（毫秒），最小 10。</summary>
    public int IntervalMs { get; set; } = 100;

    /// <summary>模拟的鼠标按键。</summary>
    public MouseButton Button { get; set; } = MouseButton.Left;

    /// <summary>单击或双击。</summary>
    public ClickMode Mode { get; set; } = ClickMode.Single;

    public bool IsRunning { get; private set; }

    /// <summary>累计点击次数（本次进程生命周期内，不因 Stop 重置）。</summary>
    public long TotalClicks => Interlocked.Read(ref _totalClicks);

    /// <summary>每完成一次点击（单击算一次、双击算一次）触发一次。</summary>
    public event Action<long>? Clicked;

    public void Start()
    {
        if (IsRunning)
        {
            return;
        }

        var interval = Math.Clamp(IntervalMs, 10, int.MaxValue);
        IsRunning = true;
        _timer = new System.Threading.Timer(OnTick, null, dueTime: 0, period: interval);
    }

    public void Stop()
    {
        if (!IsRunning)
        {
            return;
        }

        IsRunning = false;

        using var done = new ManualResetEvent(false);
        _timer?.Dispose(done);
        done.WaitOne(TimeSpan.FromSeconds(1));
        _timer = null;
    }

    private void OnTick(object? state)
    {
        if (!IsRunning)
        {
            return;
        }

        try
        {
            var (down, up) = Button == MouseButton.Left
                ? (NativeMethods.MOUSEEVENTF_LEFTDOWN, NativeMethods.MOUSEEVENTF_LEFTUP)
                : (NativeMethods.MOUSEEVENTF_RIGHTDOWN, NativeMethods.MOUSEEVENTF_RIGHTUP);

            NativeMethods.SendMouseFlag(down);
            NativeMethods.SendMouseFlag(up);

            if (Mode == ClickMode.Double)
            {
                Thread.Sleep(DoubleClickGapMs);
                NativeMethods.SendMouseFlag(down);
                NativeMethods.SendMouseFlag(up);
            }

            var count = Interlocked.Increment(ref _totalClicks);
            Clicked?.Invoke(count);
        }
        catch
        {
            // 定时器线程上不能抛异常（会拖垮整个进程），
            // 单次失败（如被系统拦截）直接跳过，下个周期继续。
        }
    }
}
