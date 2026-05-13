using System.Drawing;
using System.Globalization;
using System.Diagnostics;
using System.Windows;
using System.Windows.Forms;
using ElBruno.CopilotCLIMonitor.Core.Models;
using ElBruno.CopilotCLIMonitor.Models;
using ElBruno.CopilotCLIMonitor.Core.Services;
using ElBruno.CopilotCLIMonitor.Services;
using ElBruno.CopilotCLIMonitor.Telemetry;
using Microsoft.Extensions.Logging;

namespace ElBruno.CopilotCLIMonitor;

public partial class App : System.Windows.Application
{
    private NotifyIcon? _trayIcon;
    private IpcServer? _ipcServer;
    private DashboardWindow? _dashboard;
    private ToolStripMenuItem? _pauseNotificationsMenuItem;
    private readonly EventStore _eventStore = new();
    private readonly UserPreferencesStore _preferencesStore = new();
    private readonly ProtectedTokenStore _tokenStore = new();
    private readonly WindowsStartupManager _startupManager = new();
    private ILoggerFactory? _loggerFactory;
    private ILogger<App>? _logger;
    private bool _notificationsPaused;
    private UserPreferences _preferences = new();
    private UserTelemetryClient? _telemetryClient;

    protected override void OnStartup(StartupEventArgs e)
    {
        var startupSw = Stopwatch.StartNew();
        base.OnStartup(e);
        _loggerFactory = LoggingFactoryBuilder.Create();
        _logger = _loggerFactory.CreateLogger<App>();
        _preferences = _preferencesStore.Load();
        ApplyStartupPreference();
        LoadStoredTokenIfMissing();
        CopilotCliMonitorEventSource.Log.AppStartup();
        if (_preferences.TelemetryOptIn && !string.IsNullOrWhiteSpace(_preferences.TelemetryInstallationId))
        {
            _telemetryClient = new UserTelemetryClient(_preferences.TelemetryInstallationId);
            _telemetryClient.TrackEvent("app_start");
        }
        _logger.LogInformation("Application startup.");

        InitializeTrayIcon();
        Dispatcher.BeginInvoke(StartIpcServer, System.Windows.Threading.DispatcherPriority.Background);
        startupSw.Stop();
        _logger.LogInformation("Startup pipeline completed in {ElapsedMs} ms.", startupSw.ElapsedMilliseconds);
    }

    private void InitializeTrayIcon()
    {
        var icon = CreateDefaultIcon();

        _trayIcon = new NotifyIcon
        {
            Icon = icon,
            Visible = true,
            Text = UiResources.Get("TrayAppName")
        };

        var menu = new ContextMenuStrip();
        menu.Items.Add(UiResources.Get("MenuOpenDashboard"), null, (_, _) => OpenDashboard());
        menu.Items.Add("-");
        menu.Items.Add(UiResources.Get("MenuRecentEvents"), null, (_, _) => OpenDashboard());
        menu.Items.Add(UiResources.Get("MenuSettings"), null, (_, _) => ShowSettings());
        menu.Items.Add("-");
        _pauseNotificationsMenuItem = new ToolStripMenuItem(UiResources.Get("MenuPauseNotifications"), null, (_, _) => ToggleNotificationsPause());
        menu.Items.Add(_pauseNotificationsMenuItem);
        menu.Items.Add(UiResources.Get("MenuAbout"), null, (_, _) => ShowAbout());
        menu.Items.Add("-");
        menu.Items.Add(UiResources.Get("MenuExit"), null, (_, _) => ExitApp());

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
        _telemetryClient?.TrackEvent("event_received", monitorEvent.EventType.ToString(), monitorEvent.Repository);
        CopilotCliMonitorEventSource.Log.EventReceived(monitorEvent.EventType.ToString());

        Dispatcher.BeginInvoke(() =>
        {
            _eventStore.Add(monitorEvent);
            if (ShouldDisplayNotification(_preferences, DateTime.Now) && !_notificationsPaused)
            {
                ShowNotification(monitorEvent);
                if (_preferences.SoundEnabled)
                {
                    System.Media.SystemSounds.Asterisk.Play();
                }
            }
            else
            {
                CopilotCliMonitorEventSource.Log.NotificationSuppressed(_notificationsPaused ? "Paused" : "Preferences");
            }
            _dashboard?.RefreshEvents(_eventStore.Recent);
        });
    }

    private void ShowNotification(MonitorEvent monitorEvent)
    {
        var title = BuildNotificationTitle(monitorEvent);
        var details = BuildNotificationDetails(monitorEvent);

        _trayIcon?.ShowBalloonTip(
            timeout: GetNotificationTimeout(monitorEvent.EventType),
            tipTitle: title,
            tipText: details,
            tipIcon: GetNotificationIcon(monitorEvent.EventType));
        CopilotCliMonitorEventSource.Log.NotificationShown(monitorEvent.EventType.ToString());
    }

    private static string BuildNotificationTitle(MonitorEvent monitorEvent) =>
        monitorEvent.Repository is { Length: > 0 } repo
            ? $"[{repo}] {FormatEventType(monitorEvent.EventType)}"
            : FormatEventType(monitorEvent.EventType);

    private static string BuildNotificationDetails(MonitorEvent monitorEvent) =>
        monitorEvent.Branch is { Length: > 0 } branch
            ? $"{monitorEvent.Message} ({branch})"
            : monitorEvent.Message;

    private static string FormatEventType(EventType t, CultureInfo? culture = null) =>
        t == EventType.Unknown ? LocalizedText.Get("Notification", culture) : LocalizedText.GetEventTypeLabel(t, culture);

    private static int GetNotificationTimeout(EventType t) => t switch
    {
        EventType.Error or EventType.HookFailed => 10000,
        EventType.ApprovalRequired or EventType.Warning or EventType.LongRunningTaskWarning => 8000,
        _ => 5000
    };

    private static ToolTipIcon GetNotificationIcon(EventType t) => t switch
    {
        EventType.Error or EventType.HookFailed => ToolTipIcon.Error,
        EventType.ApprovalRequired or EventType.Warning or EventType.LongRunningTaskWarning => ToolTipIcon.Warning,
        _ => ToolTipIcon.Info
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
            UiResources.Get("AboutMessage", Environment.NewLine),
            UiResources.Get("AboutTitle"),
            MessageBoxButton.OK,
            MessageBoxImage.Information);
    }

    private void ShowSettings()
    {
        var settingsWindow = new SettingsWindow(_preferences);
        var saved = settingsWindow.ShowDialog();
        if (saved != true || settingsWindow.UpdatedPreferences is null)
        {
            return;
        }

        _preferences = settingsWindow.UpdatedPreferences;
        if (!string.IsNullOrWhiteSpace(settingsWindow.UpdatedToken))
        {
            _tokenStore.SaveToken(settingsWindow.UpdatedToken);
            Environment.SetEnvironmentVariable(IpcConstants.AuthTokenEnvVar, settingsWindow.UpdatedToken);
        }
        if (_preferences.TelemetryOptIn && string.IsNullOrWhiteSpace(_preferences.TelemetryInstallationId))
        {
            _preferences.TelemetryInstallationId = Guid.NewGuid().ToString("N");
        }
        if (_preferences.TelemetryOptIn && !string.IsNullOrWhiteSpace(_preferences.TelemetryInstallationId))
        {
            _telemetryClient ??= new UserTelemetryClient(_preferences.TelemetryInstallationId);
            _telemetryClient.TrackEvent("telemetry_enabled");
        }
        else
        {
            _telemetryClient = null;
        }
        _preferencesStore.Save(_preferences);
        Environment.SetEnvironmentVariable("COPILOTCLIMON_LOG_LEVEL", _preferences.LogLevel);
        ApplyStartupPreference();

        var tokenConfigured = !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable(IpcConstants.AuthTokenEnvVar));
        System.Windows.MessageBox.Show(
            BuildSettingsSummary(IpcConstants.DefaultPort, tokenConfigured, _preferences),
            UiResources.Get("SettingsTitle"),
            MessageBoxButton.OK,
            MessageBoxImage.Information);
    }

    private void ToggleNotificationsPause()
    {
        _notificationsPaused = !_notificationsPaused;
        if (_pauseNotificationsMenuItem is not null)
        {
            _pauseNotificationsMenuItem.Text = _notificationsPaused ? UiResources.Get("MenuResumeNotifications") : UiResources.Get("MenuPauseNotifications");
        }
    }

    private static string BuildSettingsSummary(int ipcPort, bool tokenConfigured, UserPreferences preferences) =>
        UiResources.Get("SettingsSummaryTemplate",
            ipcPort,
            tokenConfigured ? UiResources.Get("TokenConfigured") : UiResources.Get("TokenNotConfigured"),
            preferences.NotificationsEnabled,
            preferences.QuietHoursEnabled ? $"{preferences.QuietHoursStart}:00-{preferences.QuietHoursEnd}:00" : UiResources.Get("QuietHoursDisabled"),
            preferences.LogLevel,
            Environment.NewLine,
            preferences.TelemetryOptIn ? UiResources.Get("TelemetryEnabled") : UiResources.Get("TelemetryDisabled"));

    private static bool ShouldDisplayNotification(UserPreferences preferences, DateTime localNow)
    {
        if (!preferences.NotificationsEnabled)
        {
            return false;
        }

        if (!preferences.QuietHoursEnabled)
        {
            return true;
        }

        return !IsWithinQuietHours(localNow.Hour, preferences.QuietHoursStart, preferences.QuietHoursEnd);
    }

    private static bool IsWithinQuietHours(int currentHour, int quietStartHour, int quietEndHour)
    {
        if (quietStartHour == quietEndHour)
        {
            return true;
        }

        if (quietStartHour < quietEndHour)
        {
            return currentHour >= quietStartHour && currentHour < quietEndHour;
        }

        return currentHour >= quietStartHour || currentHour < quietEndHour;
    }

    private void LoadStoredTokenIfMissing()
    {
        if (!string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable(IpcConstants.AuthTokenEnvVar)))
        {
            return;
        }

        var storedToken = _tokenStore.LoadToken();
        if (!string.IsNullOrWhiteSpace(storedToken))
        {
            Environment.SetEnvironmentVariable(IpcConstants.AuthTokenEnvVar, storedToken);
        }
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

    private void ApplyStartupPreference()
    {
        var processPath = Environment.ProcessPath;
        if (string.IsNullOrWhiteSpace(processPath))
        {
            return;
        }

        _startupManager.SetEnabled(_preferences.StartWithWindows, processPath);
    }
}
