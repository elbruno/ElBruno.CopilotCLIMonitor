using ElBruno.CopilotCLIMonitor.Models;
using ElBruno.CopilotCLIMonitor.Services;

namespace ElBruno.CopilotCLIMonitor.Tests.Services;

public sealed class UserPreferencesStoreTests : IDisposable
{
    private readonly string _tempUserFile = Path.Combine(Path.GetTempPath(), $"copilotclimon-user-preferences-{Guid.NewGuid():N}.json");
    private readonly string _tempSystemFile = Path.Combine(Path.GetTempPath(), $"copilotclimon-system-preferences-{Guid.NewGuid():N}.json");

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

    public void Dispose()
    {
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
