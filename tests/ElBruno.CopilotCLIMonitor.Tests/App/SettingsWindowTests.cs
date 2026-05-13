namespace ElBruno.CopilotCLIMonitor.Tests.App;

public sealed class SettingsWindowTests
{
    [Fact]
    public void BuildUpdatedPreferences_AppliesProvidedValues()
    {
        var preferences = SettingsWindow.BuildUpdatedPreferences(
            notificationsEnabled: false,
            soundEnabled: true,
            quietHoursEnabled: true,
            quietHoursStart: 21,
            quietHoursEnd: 6,
            logLevel: "Debug",
            startWithWindows: true,
            telemetryOptIn: true,
            telemetryInstallationId: "telemetry-id");

        Assert.False(preferences.NotificationsEnabled);
        Assert.True(preferences.SoundEnabled);
        Assert.True(preferences.QuietHoursEnabled);
        Assert.Equal(21, preferences.QuietHoursStart);
        Assert.Equal(6, preferences.QuietHoursEnd);
        Assert.Equal("Debug", preferences.LogLevel);
        Assert.True(preferences.StartWithWindows);
        Assert.True(preferences.TelemetryOptIn);
        Assert.Equal("telemetry-id", preferences.TelemetryInstallationId);
    }

    [Fact]
    public void BuildUpdatedPreferences_UsesDefaultsWhenNullProvided()
    {
        var preferences = SettingsWindow.BuildUpdatedPreferences(
            notificationsEnabled: true,
            soundEnabled: false,
            quietHoursEnabled: false,
            quietHoursStart: null,
            quietHoursEnd: null,
            logLevel: null,
            startWithWindows: false,
            telemetryOptIn: false,
            telemetryInstallationId: null);

        Assert.True(preferences.NotificationsEnabled);
        Assert.False(preferences.SoundEnabled);
        Assert.Equal(22, preferences.QuietHoursStart);
        Assert.Equal(7, preferences.QuietHoursEnd);
        Assert.Equal("Information", preferences.LogLevel);
        Assert.False(preferences.StartWithWindows);
        Assert.False(preferences.TelemetryOptIn);
    }
}
