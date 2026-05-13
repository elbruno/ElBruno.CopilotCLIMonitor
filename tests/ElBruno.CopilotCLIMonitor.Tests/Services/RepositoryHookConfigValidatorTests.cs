using System.Text.Json;
using ElBruno.CopilotCLIMonitor.Core.Services;

namespace ElBruno.CopilotCLIMonitor.Tests.Services;

public class RepositoryHookConfigValidatorTests
{
    [Fact]
    public void TryValidateJson_WhenValidConfig_ReturnsTrue()
    {
        using var document = JsonDocument.Parse(
            """
            {
              "version": "1.0",
              "repository": "demo",
              "enabled": true,
              "notificationsEnabled": true,
              "events": ["task-completed", "error"],
              "quietHours": {
                "enabled": false,
                "start": "22:00",
                "end": "08:00"
              },
              "routing": {
                "sourceTagging": true
              }
            }
            """);

        var isValid = RepositoryHookConfigValidator.TryValidateJson(document.RootElement, out var error);

        Assert.True(isValid);
        Assert.Null(error);
    }

    [Fact]
    public void TryValidateJson_WhenEventsAreMissing_ReturnsFalse()
    {
        using var document = JsonDocument.Parse(
            """
            {
              "version": "1.0",
              "repository": "demo",
              "enabled": true,
              "notificationsEnabled": true,
              "quietHours": {
                "enabled": false,
                "start": "22:00",
                "end": "08:00"
              },
              "routing": {
                "sourceTagging": true
              }
            }
            """);

        var isValid = RepositoryHookConfigValidator.TryValidateJson(document.RootElement, out var error);

        Assert.False(isValid);
        Assert.Contains("events", error);
    }

    [Fact]
    public void TryValidateJson_WhenQuietHoursAreInvalid_ReturnsFalse()
    {
        using var document = JsonDocument.Parse(
            """
            {
              "version": "1.0",
              "repository": "demo",
              "enabled": true,
              "notificationsEnabled": true,
              "events": ["task-completed"],
              "quietHours": {
                "enabled": true,
                "start": "25:00",
                "end": "08:00"
              },
              "routing": {
                "sourceTagging": true
              }
            }
            """);

        var isValid = RepositoryHookConfigValidator.TryValidateJson(document.RootElement, out var error);

        Assert.False(isValid);
        Assert.Contains("start", error);
    }
}
