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
        UpdatedPreferences = new UserPreferences
        {
            NotificationsEnabled = NotificationsEnabledCheckBox.IsChecked ?? true,
            SoundEnabled = SoundEnabledCheckBox.IsChecked ?? false,
            QuietHoursEnabled = QuietHoursEnabledCheckBox.IsChecked ?? false,
            QuietHoursStart = QuietHoursStartComboBox.SelectedItem is int start ? start : 22,
            QuietHoursEnd = QuietHoursEndComboBox.SelectedItem is int end ? end : 7,
            LogLevel = LogLevelComboBox.SelectedItem as string ?? "Information",
            StartWithWindows = StartWithWindowsCheckBox.IsChecked ?? false,
            TelemetryOptIn = TelemetryOptInCheckBox.IsChecked ?? false,
            TelemetryInstallationId = _initialPreferences.TelemetryInstallationId
        };

        DialogResult = true;
        Close();
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}
