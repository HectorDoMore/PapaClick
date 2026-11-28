namespace PapaClick.Core;

/// <summary>
/// 连点引擎：按设定间隔向目标位置发送鼠标点击。
/// 抖动模式：开启后点击位置在启动时光标位置附近随机偏移，间隔亦随机波动，
/// 用于模拟人工操作特征。关闭时点击发生在当前光标位置，间隔精确固定。
/// 定时器采用"每次触发后按下一次的（可能带抖动的）延迟重新武装"模式。
/// </summary>
public sealed class ClickEngine
{
    /// <summary>双击时两次单击之间的间隔（毫秒）。</summary>
    private const int DoubleClickGapMs = 40;

    /// <summary>间隔抖动幅度（比例），±15%。</summary>
    private const double IntervalJitterRatio = 0.15;

    private System.Threading.Timer? _timer;
    private readonly Random _random = new();
    private NativeMethods.POINT? _anchor;
    private long _sessionClicks;
    private long _totalClicks;

    /// <summary>点击间隔（毫秒），最小 1。</summary>
    public int IntervalMs { get; set; } = 100;

    /// <summary>模拟的鼠标按键。</summary>
    public MouseButton Button { get; set; } = MouseButton.Left;

    /// <summary>单击或双击。</summary>
    public ClickMode Mode { get; set; } = ClickMode.Single;

    /// <summary>是否启用防检测抖动。</summary>
    public bool JitterEnabled { get; set; }

    /// <summary>抖动幅度（像素）：点击位置在锚点 ±N px 内随机偏移。</summary>
    public int JitterRangePx { get; set; } = 3;

    public bool IsRunning { get; private set; }

    /// <summary>本次运行的点击次数（Start 时清零）。</summary>
    public long SessionClicks => Interlocked.Read(ref _sessionClicks);

    /// <summary>本次进程生命周期内的累计点击次数（跨 Start/Stop 不清零）。</summary>
    public long TotalClicks => Interlocked.Read(ref _totalClicks);

    /// <summary>每完成一次点击触发（当次, 累计）。后台线程触发。</summary>
    public event Action<long, long>? Clicked;

    public void Start()
    {
        if (IsRunning)
        {
            return;
        }

        Interlocked.Exchange(ref _sessionClicks, 0);
        _anchor = JitterEnabled ? NativeMethods.TryGetCursorPos() : null;
        IsRunning = true;
        NativeMethods.EnableHighResolutionTimer();
        _timer = new System.Threading.Timer(OnTick, null, dueTime: 0, period: Timeout.Infinite);
    }

    public void Stop()
    {
        if (!IsRunning)
        {
            return;
        }

        IsRunning = false;
        NativeMethods.DisableHighResolutionTimer();

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
            DoClick();
        }
        catch
        {
            // 定时器线程上不能抛异常（会拖垮整个进程），
            // 单次失败（如被系统拦截）直接跳过，下个周期继续。
        }

        if (!IsRunning)
        {
            return;
        }

        _timer?.Change(NextDelay(), Timeout.Infinite);
    }

    /// <summary>计算下一次触发延迟：固定模式精确等间隔，抖动模式 ±15% 随机波动。</summary>
    private int NextDelay()
    {
        var baseMs = Math.Clamp(IntervalMs, 1, int.MaxValue);
        if (!JitterEnabled)
        {
            return baseMs;
        }

        var spread = Math.Max(1, (int)Math.Round(baseMs * IntervalJitterRatio));
        return Math.Max(1, baseMs + _random.Next(-spread, spread + 1));
    }

    private void DoClick()
    {
        if (JitterEnabled && _anchor is { } anchor)
        {
            var x = anchor.X + _random.Next(-JitterRangePx, JitterRangePx + 1);
            var y = anchor.Y + _random.Next(-JitterRangePx, JitterRangePx + 1);
            NativeMethods.TrySetCursorPos(x, y);
        }

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

        var session = Interlocked.Increment(ref _sessionClicks);
        var total = Interlocked.Increment(ref _totalClicks);
        Clicked?.Invoke(session, total);
    }
}
