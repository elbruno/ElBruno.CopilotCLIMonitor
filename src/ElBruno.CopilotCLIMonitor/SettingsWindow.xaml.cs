using System.Windows;
using ElBruno.CopilotCLIMonitor.Models;
using ElBruno.CopilotCLIMonitor.Services;

namespace ElBruno.CopilotCLIMonitor;

public partial class SettingsWindow : Window
{
    private readonly UserPreferences _initialPreferences;

    public SettingsWindow(UserPreferences preferences)
    {
        _initialPreferences = preferences;
        InitializeComponent();
        UiCultureSupport.ApplyFlowDirection(this);
        QuietHoursStartComboBox.ItemsSource = Enumerable.Range(0, 24).ToList();
        QuietHoursEndComboBox.ItemsSource = Enumerable.Range(0, 24).ToList();
        LogLevelComboBox.ItemsSource = new[] { "Trace", "Debug", "Information", "Warning", "Error", "Critical" };
        ApplyPreferences(preferences);
    }

    public UserPreferences? UpdatedPreferences { get; private set; }
    public string? UpdatedToken { get; private set; }

    private void ApplyPreferences(UserPreferences preferences)
    {
        NotificationsEnabledCheckBox.IsChecked = preferences.NotificationsEnabled;
        SoundEnabledCheckBox.IsChecked = preferences.SoundEnabled;
        QuietHoursEnabledCheckBox.IsChecked = preferences.QuietHoursEnabled;
        QuietHoursStartComboBox.SelectedItem = preferences.QuietHoursStart;
        QuietHoursEndComboBox.SelectedItem = preferences.QuietHoursEnd;
        LogLevelComboBox.SelectedItem = preferences.LogLevel;
        StartWithWindowsCheckBox.IsChecked = preferences.StartWithWindows;
        TelemetryOptInCheckBox.IsChecked = preferences.TelemetryOptIn;
    }

    private void Save_Click(object sender, RoutedEventArgs e)
    {
        UpdatedPreferences = BuildUpdatedPreferences(
            notificationsEnabled: NotificationsEnabledCheckBox.IsChecked ?? true,
            soundEnabled: SoundEnabledCheckBox.IsChecked ?? false,
            quietHoursEnabled: QuietHoursEnabledCheckBox.IsChecked ?? false,
            quietHoursStart: QuietHoursStartComboBox.SelectedItem as int?,
            quietHoursEnd: QuietHoursEndComboBox.SelectedItem as int?,
            logLevel: LogLevelComboBox.SelectedItem as string,
            startWithWindows: StartWithWindowsCheckBox.IsChecked ?? false,
            telemetryOptIn: TelemetryOptInCheckBox.IsChecked ?? false,
            telemetryInstallationId: _initialPreferences.TelemetryInstallationId);
        UpdatedToken = string.IsNullOrWhiteSpace(IpcTokenPasswordBox.Password) ? null : IpcTokenPasswordBox.Password;

        DialogResult = true;
        Close();
    }

    public static UserPreferences BuildUpdatedPreferences(
        bool notificationsEnabled,
        bool soundEnabled,
        bool quietHoursEnabled,
        int? quietHoursStart,
        int? quietHoursEnd,
        string? logLevel,
        bool startWithWindows,
        bool telemetryOptIn,
        string? telemetryInstallationId) =>
        new()
        {
            NotificationsEnabled = notificationsEnabled,
            SoundEnabled = soundEnabled,
            QuietHoursEnabled = quietHoursEnabled,
            QuietHoursStart = quietHoursStart ?? 22,
            QuietHoursEnd = quietHoursEnd ?? 7,
            LogLevel = logLevel ?? "Information",
            StartWithWindows = startWithWindows,
            TelemetryOptIn = telemetryOptIn,
            TelemetryInstallationId = telemetryInstallationId
        };

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}
