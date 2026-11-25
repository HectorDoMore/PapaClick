using System.Globalization;
using PapaClick.Core;

namespace PapaClick.CLI;

/// <summary>
/// PapaClick 命令行版：按参数启动连点引擎，按 Enter 停止。
/// </summary>
internal static class Program
{
    public static int Main(string[] args)
    {
        if (args.Length == 0 || args.Contains("-h") || args.Contains("--help"))
        {
            PrintUsage();
            return 0;
        }

        double intervalSeconds;
        MouseButton button;
        ClickMode mode;

        try
        {
            intervalSeconds = ReadOption(args, "--interval", "0.1", s =>
            {
                var v = double.Parse(s, CultureInfo.InvariantCulture);
                if (v is < 0.001 or > 60)
                {
                    throw new ArgumentException("--interval 取值范围 0.001 ~ 60（秒）");
                }
                return v;
            });

            var buttonStr = ReadOption(args, "--button", "left", s => s.ToLowerInvariant() switch
            {
                "left" or "l" => "left",
                "right" or "r" => "right",
                _ => throw new ArgumentException("--button 只支持 left / right"),
            });
            button = buttonStr == "left" ? MouseButton.Left : MouseButton.Right;

            var modeStr = ReadOption(args, "--mode", "single", s => s.ToLowerInvariant() switch
            {
                "single" or "s" => "single",
                "double" or "d" => "double",
                _ => throw new ArgumentException("--mode 只支持 single / double"),
            });
            mode = modeStr == "single" ? ClickMode.Single : ClickMode.Double;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"参数错误：{ex.Message}");
            PrintUsage();
            return 1;
        }

        Run(intervalSeconds, button, mode);
        return 0;
    }

    private static void Run(double intervalSeconds, MouseButton button, ClickMode mode)
    {
        var engine = new ClickEngine
        {
            IntervalMs = Math.Max(1, (int)Math.Round(intervalSeconds * 1000)),
            Button = button,
            Mode = mode,
        };

        Console.WriteLine($"PapaClick CLI · 间隔 {intervalSeconds} 秒 · 按键 {button} · 方式 {mode}");
        Console.WriteLine("点击发生在当前光标位置。按 Enter 停止，Ctrl+C 退出程序。");
        Console.WriteLine();

        engine.Start();

        using var cancel = new CancellationTokenSource();
        Console.CancelKeyPress += (_, e) =>
        {
            e.Cancel = true;
            cancel.Cancel();
        };

        var reporter = Task.Run(async () =>
        {
            while (!cancel.IsCancellationRequested)
            {
                Console.Write($"\r连点中 · 已点击 {engine.TotalClicks} 次    ");
                try
                {
                    await Task.Delay(500, cancel.Token);
                }
                catch (TaskCanceledException)
                {
                    return;
                }
            }
        });

        try
        {
            Console.ReadLine();
        }
        catch (ObjectDisposedException) { }

        engine.Stop();
        cancel.Cancel();
        try
        {
            reporter.Wait(600);
        }
        catch { }

        Console.WriteLine($"\r已停止 · 共点击 {engine.TotalClicks} 次    ");
    }

    private static T ReadOption<T>(string[] args, string name, string defaultValue, Func<string, T> parse)
    {
        for (int i = 0; i < args.Length - 1; i++)
        {
            if (string.Equals(args[i], name, StringComparison.OrdinalIgnoreCase))
            {
                return parse(args[i + 1]);
            }
        }
        return parse(defaultValue);
    }

    private static void PrintUsage()
    {
        Console.WriteLine("""
            PapaClick CLI —— 命令行连点器

            用法：PapaClick.CLI [选项]

            选项：
              --interval <秒>    点击间隔，0.001 ~ 60，默认 0.1
              --button <键>      left / right，默认 left
              --mode <方式>      single / double，默认 single
              -h, --help         显示本帮助

            示例：
              PapaClick.CLI --interval 0.05 --button right
              PapaClick.CLI --interval 1 --mode double
            """);
    }
}
