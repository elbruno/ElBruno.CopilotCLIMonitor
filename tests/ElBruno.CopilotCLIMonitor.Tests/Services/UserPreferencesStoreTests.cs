using ElBruno.CopilotCLIMonitor.Models;
using ElBruno.CopilotCLIMonitor.Services;

namespace ElBruno.CopilotCLIMonitor.Tests.Services;

public sealed class UserPreferencesStoreTests : IDisposable
{
    private readonly string _tempUserFile = Path.Combine(Path.GetTempPath(), $"copilotclimon-user-preferences-{Guid.NewGuid():N}.json");
    private readonly string _tempSystemFile = Path.Combine(Path.GetTempPath(), $"copilotclimon-system-preferences-{Guid.NewGuid():N}.json");
    private static readonly string[] EnvironmentVariablesToReset =
    [
        "COPILOTCLIMON_PREFERENCES_PATH",
        "COPILOTCLIMON_SYSTEM_PREFERENCES_PATH",
        "COPILOTCLIMON_NOTIFICATIONS_ENABLED",
        "COPILOTCLIMON_SOUND_ENABLED",
        "COPILOTCLIMON_QUIET_HOURS_ENABLED",
        "COPILOTCLIMON_QUIET_HOURS_START",
        "COPILOTCLIMON_QUIET_HOURS_END",
        "COPILOTCLIMON_LOG_LEVEL",
        "COPILOTCLIMON_START_WITH_WINDOWS",
        "COPILOTCLIMON_TELEMETRY_OPT_IN",
        "COPILOTCLIMON_TELEMETRY_INSTALLATION_ID"
    ];

    [Fact]
    public void Load_WhenFileDoesNotExist_ReturnsDefaults()
    {
        var store = new UserPreferencesStore(_tempUserFile, _tempSystemFile);

        var preferences = store.Load();

        Assert.True(preferences.NotificationsEnabled);
        Assert.Equal("Information", preferences.LogLevel);
    }

    [Fact]
    public void SaveAndLoad_RoundTripsPreferences()
    {
        var store = new UserPreferencesStore(_tempUserFile, _tempSystemFile);
        var expected = new UserPreferences
        {
            NotificationsEnabled = false,
            SoundEnabled = true,
            QuietHoursEnabled = true,
            QuietHoursStart = 21,
            QuietHoursEnd = 6,
            LogLevel = "Debug",
            StartWithWindows = true,
            TelemetryOptIn = true,
            TelemetryInstallationId = "abc123"
        };

        store.Save(expected);
        var loaded = store.Load();

        Assert.False(loaded.NotificationsEnabled);
        Assert.True(loaded.SoundEnabled);
        Assert.True(loaded.QuietHoursEnabled);
        Assert.Equal(21, loaded.QuietHoursStart);
        Assert.Equal(6, loaded.QuietHoursEnd);
        Assert.Equal("Debug", loaded.LogLevel);
        Assert.True(loaded.StartWithWindows);
        Assert.True(loaded.TelemetryOptIn);
        Assert.Equal("abc123", loaded.TelemetryInstallationId);
    }

    [Fact]
    public void Load_WhenSystemAndUserFilesExist_AppliesSystemThenUserOverrides()
    {
        File.WriteAllText(
            _tempSystemFile,
            """
            {
              "NotificationsEnabled": true,
              "SoundEnabled": true,
              "QuietHoursEnabled": true,
              "QuietHoursStart": 20,
              "QuietHoursEnd": 6,
              "LogLevel": "Warning",
              "StartWithWindows": true
            }
            """);

        File.WriteAllText(
            _tempUserFile,
            """
            {
              "NotificationsEnabled": false,
              "LogLevel": "Debug",
              "TelemetryOptIn": true
            }
            """);

        var store = new UserPreferencesStore(_tempUserFile, _tempSystemFile);
        var loaded = store.Load();

        Assert.False(loaded.NotificationsEnabled);
        Assert.True(loaded.SoundEnabled);
        Assert.True(loaded.QuietHoursEnabled);
        Assert.Equal(20, loaded.QuietHoursStart);
        Assert.Equal(6, loaded.QuietHoursEnd);
        Assert.Equal("Debug", loaded.LogLevel);
        Assert.True(loaded.StartWithWindows);
        Assert.True(loaded.TelemetryOptIn);
    }

    [Fact]
    public void Load_WhenEnvironmentVariablesAreSet_OverridesFileValues()
    {
        var store = new UserPreferencesStore(_tempUserFile, _tempSystemFile);
        store.Save(new UserPreferences
        {
            NotificationsEnabled = true,
            SoundEnabled = false,
            QuietHoursEnabled = false,
            QuietHoursStart = 22,
            QuietHoursEnd = 7,
            LogLevel = "Information",
            StartWithWindows = false,
            TelemetryOptIn = false,
            TelemetryInstallationId = "file-id"
        });

        Environment.SetEnvironmentVariable("COPILOTCLIMON_NOTIFICATIONS_ENABLED", "false");
        Environment.SetEnvironmentVariable("COPILOTCLIMON_SOUND_ENABLED", "true");
        Environment.SetEnvironmentVariable("COPILOTCLIMON_QUIET_HOURS_ENABLED", "true");
        Environment.SetEnvironmentVariable("COPILOTCLIMON_QUIET_HOURS_START", "19");
        Environment.SetEnvironmentVariable("COPILOTCLIMON_QUIET_HOURS_END", "5");
        Environment.SetEnvironmentVariable("COPILOTCLIMON_LOG_LEVEL", "Debug");
        Environment.SetEnvironmentVariable("COPILOTCLIMON_START_WITH_WINDOWS", "true");
        Environment.SetEnvironmentVariable("COPILOTCLIMON_TELEMETRY_OPT_IN", "true");
        Environment.SetEnvironmentVariable("COPILOTCLIMON_TELEMETRY_INSTALLATION_ID", "env-id");

        var loaded = store.Load();

        Assert.False(loaded.NotificationsEnabled);
        Assert.True(loaded.SoundEnabled);
        Assert.True(loaded.QuietHoursEnabled);
        Assert.Equal(19, loaded.QuietHoursStart);
        Assert.Equal(5, loaded.QuietHoursEnd);
        Assert.Equal("Debug", loaded.LogLevel);
        Assert.True(loaded.StartWithWindows);
        Assert.True(loaded.TelemetryOptIn);
        Assert.Equal("env-id", loaded.TelemetryInstallationId);
    }

    [Fact]
    public void Save_WhenPreferencePathEnvironmentVariableIsSet_UsesOverriddenPath()
    {
        var envPath = Path.Combine(Path.GetTempPath(), $"copilotclimon-env-user-{Guid.NewGuid():N}.json");
        var envSystemPath = Path.Combine(Path.GetTempPath(), $"copilotclimon-env-system-{Guid.NewGuid():N}.json");

        Environment.SetEnvironmentVariable("COPILOTCLIMON_PREFERENCES_PATH", envPath);
        Environment.SetEnvironmentVariable("COPILOTCLIMON_SYSTEM_PREFERENCES_PATH", envSystemPath);

        var store = new UserPreferencesStore();
        store.Save(new UserPreferences { LogLevel = "Trace" });

        Assert.True(File.Exists(envPath));

        if (File.Exists(envPath))
        {
            File.Delete(envPath);
        }

        if (File.Exists(envSystemPath))
        {
            File.Delete(envSystemPath);
        }
    }

    public void Dispose()
    {
        foreach (var envVar in EnvironmentVariablesToReset)
        {
            Environment.SetEnvironmentVariable(envVar, null);
        }

        if (File.Exists(_tempUserFile))
        {
            File.Delete(_tempUserFile);
        }

        if (File.Exists(_tempSystemFile))
        {
            File.Delete(_tempSystemFile);
        }
    }
}
