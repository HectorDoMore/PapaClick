using System.Windows;
using PapaClick.Core;

namespace PapaClick.GUI;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        DispatcherUnhandledException += (_, args) =>
        {
            DebugLog.Write($"UI 线程异常: {args.Exception}");
            MessageBox.Show($"程序遇到错误：{args.Exception.Message}\n\n详情见日志 {DebugLog.LogPath}",
                "PapaClick", MessageBoxButton.OK, MessageBoxImage.Error);
            args.Handled = true;
        };
        AppDomain.CurrentDomain.UnhandledException += (_, args) =>
            DebugLog.Write($"致命异常: {args.ExceptionObject}");

        DebugLog.Write("App 启动");
        base.OnStartup(e);
    }
}
