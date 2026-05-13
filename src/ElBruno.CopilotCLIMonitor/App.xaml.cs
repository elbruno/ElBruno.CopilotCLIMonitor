using System.Drawing;
using System.Windows;
using System.Windows.Forms;
using ElBruno.CopilotCLIMonitor.Core.Models;
using ElBruno.CopilotCLIMonitor.Core.Services;
using ElBruno.CopilotCLIMonitor.Services;
using Microsoft.Extensions.Logging;

namespace ElBruno.CopilotCLIMonitor;

public partial class App : System.Windows.Application
{
    private NotifyIcon? _trayIcon;
    private IpcServer? _ipcServer;
    private DashboardWindow? _dashboard;
    private readonly EventStore _eventStore = new();
    private ILoggerFactory? _loggerFactory;
    private ILogger<App>? _logger;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        _loggerFactory = LoggingFactoryBuilder.Create();
        _logger = _loggerFactory.CreateLogger<App>();
        _logger.LogInformation("Application startup.");

        InitializeTrayIcon();
        StartIpcServer();
    }

    private void InitializeTrayIcon()
    {
        var icon = CreateDefaultIcon();

        _trayIcon = new NotifyIcon
        {
            Icon = icon,
            Visible = true,
            Text = "CopilotCLI Monitor"
        };

        var menu = new ContextMenuStrip();
        menu.Items.Add("Open Dashboard", null, (_, _) => OpenDashboard());
        menu.Items.Add("-");
        menu.Items.Add("Recent Events", null, (_, _) => OpenDashboard());
        menu.Items.Add("-");
        menu.Items.Add("About", null, (_, _) => ShowAbout());
        menu.Items.Add("-");
        menu.Items.Add("Exit", null, (_, _) => ExitApp());

        _trayIcon.ContextMenuStrip = menu;
        _trayIcon.DoubleClick += (_, _) => OpenDashboard();
    }

    private static Icon CreateDefaultIcon()
    {
        // Create a simple 16x16 icon programmatically (green circle)
        var bmp = new Bitmap(16, 16);
        using (var g = Graphics.FromImage(bmp))
        {
            g.Clear(Color.Transparent);
            g.FillEllipse(Brushes.DodgerBlue, 1, 1, 14, 14);
            g.DrawString("C", new Font("Arial", 7, GraphicsUnit.Pixel), Brushes.White, 3, 3);
        }
        return Icon.FromHandle(bmp.GetHicon());
    }

    private void StartIpcServer()
    {
        _ipcServer = new IpcServer(IpcConstants.DefaultPort, _loggerFactory?.CreateLogger<IpcServer>());
        _ipcServer.EventReceived += OnEventReceived;
        _ipcServer.Start();
    }

    private void OnEventReceived(MonitorEvent monitorEvent)
    {
        _logger?.LogDebug(
            "Dispatching notification event type={EventType} repository={Repository} branch={Branch}.",
            monitorEvent.EventType, monitorEvent.Repository, monitorEvent.Branch);

        Dispatcher.BeginInvoke(() =>
        {
            _eventStore.Add(monitorEvent);
            ShowNotification(monitorEvent);
            _dashboard?.RefreshEvents(_eventStore.Recent);
        });
    }

    private void ShowNotification(MonitorEvent monitorEvent)
    {
        var title = monitorEvent.Repository is { Length: > 0 } repo
            ? $"[{repo}] {FormatEventType(monitorEvent.EventType)}"
            : FormatEventType(monitorEvent.EventType);

        _trayIcon?.ShowBalloonTip(
            timeout: 5000,
            tipTitle: title,
            tipText: monitorEvent.Message,
            tipIcon: monitorEvent.EventType switch
            {
                EventType.Error or EventType.HookFailed => ToolTipIcon.Error,
                EventType.ApprovalRequired or EventType.Warning or EventType.LongRunningTaskWarning => ToolTipIcon.Warning,
                _ => ToolTipIcon.Info
            });
    }

    private static string FormatEventType(EventType t) => t switch
    {
        EventType.TaskCompleted => "Task Completed",
        EventType.ApprovalRequired => "Approval Required",
        EventType.Error => "Error",
        EventType.Warning => "Warning",
        EventType.BuildCompleted => "Build Completed",
        EventType.TestCompleted => "Test Completed",
        EventType.WorkflowCompleted => "Workflow Completed",
        EventType.LongRunningTaskWarning => "Long-Running Task",
        EventType.HookFailed => "Hook Failed",
        _ => "Notification"
    };

    private void OpenDashboard()
    {
        if (_dashboard is null || !_dashboard.IsLoaded)
        {
            _dashboard = new DashboardWindow(_eventStore);
        }
        _dashboard.Show();
        _dashboard.Activate();
        _dashboard.RefreshEvents(_eventStore.Recent);
    }

    private void ShowAbout()
    {
        System.Windows.MessageBox.Show(
            "CopilotCLI Monitor v0.1.0\n\nMonitors GitHub Copilot CLI tasks and shows\nWindows desktop notifications.\n\nCLI: copilotclimon notify --event task-completed --message \"Done\"",
            "About CopilotCLI Monitor",
            MessageBoxButton.OK,
            MessageBoxImage.Information);
    }

    private void ExitApp()
    {
        _trayIcon?.Dispose();
        _ipcServer?.Stop();
        Shutdown();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _logger?.LogInformation("Application exit.");
        _trayIcon?.Dispose();
        _ipcServer?.Stop();
        _loggerFactory?.Dispose();
        base.OnExit(e);
    }
}
