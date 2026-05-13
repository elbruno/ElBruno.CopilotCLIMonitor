using System.Reflection;
using System.Windows.Forms;
using ElBruno.CopilotCLIMonitor.Models;
using ElBruno.CopilotCLIMonitor.Core.Models;

namespace ElBruno.CopilotCLIMonitor.Tests.App;

public class AppFormattingTests
{
    [Theory]
    [InlineData(EventType.TaskCompleted, "Task Completed")]
    [InlineData(EventType.Error, "Error")]
    [InlineData(EventType.LongRunningTaskWarning, "Long-Running Task")]
    [InlineData(EventType.Unknown, "Notification")]
    public void FormatEventType_ReturnsExpectedLabel(EventType eventType, string expected)
    {
        var method = typeof(ElBruno.CopilotCLIMonitor.App)
            .GetMethod("FormatEventType", BindingFlags.NonPublic | BindingFlags.Static);

        Assert.NotNull(method);
        var label = method!.Invoke(null, [eventType]) as string;
        Assert.Equal(expected, label);
    }

    [Theory]
    [InlineData(EventType.Error, 10000)]
    [InlineData(EventType.HookFailed, 10000)]
    [InlineData(EventType.Warning, 8000)]
    [InlineData(EventType.ApprovalRequired, 8000)]
    [InlineData(EventType.TaskCompleted, 5000)]
    public void GetNotificationTimeout_ReturnsExpectedDuration(EventType eventType, int expected)
    {
        var method = typeof(ElBruno.CopilotCLIMonitor.App)
            .GetMethod("GetNotificationTimeout", BindingFlags.NonPublic | BindingFlags.Static);

        Assert.NotNull(method);
        var timeout = method!.Invoke(null, [eventType]);
        Assert.Equal(expected, timeout);
    }

    [Theory]
    [InlineData(EventType.Error, ToolTipIcon.Error)]
    [InlineData(EventType.HookFailed, ToolTipIcon.Error)]
    [InlineData(EventType.Warning, ToolTipIcon.Warning)]
    [InlineData(EventType.ApprovalRequired, ToolTipIcon.Warning)]
    [InlineData(EventType.TaskCompleted, ToolTipIcon.Info)]
    public void GetNotificationIcon_ReturnsExpectedIcon(EventType eventType, ToolTipIcon expected)
    {
        var method = typeof(ElBruno.CopilotCLIMonitor.App)
            .GetMethod("GetNotificationIcon", BindingFlags.NonPublic | BindingFlags.Static);

        Assert.NotNull(method);
        var icon = method!.Invoke(null, [eventType]);
        Assert.Equal(expected, icon);
    }

    [Fact]
    public void BuildSettingsSummary_ContainsPortAndTokenStatus()
    {
        var method = typeof(ElBruno.CopilotCLIMonitor.App)
            .GetMethod("BuildSettingsSummary", BindingFlags.NonPublic | BindingFlags.Static);

        Assert.NotNull(method);
        var preferences = new UserPreferences
        {
            NotificationsEnabled = true,
            QuietHoursEnabled = true,
            QuietHoursStart = 22,
            QuietHoursEnd = 7,
            LogLevel = "Debug"
        };
        var summary = method!.Invoke(null, [41234, true, preferences]) as string;

        Assert.NotNull(summary);
        Assert.Contains("IPC Port: 41234", summary);
        Assert.Contains("Authentication Token: Configured", summary);
        Assert.Contains("Notifications Enabled: True", summary);
        Assert.Contains("Quiet Hours: 22:00-7:00", summary);
    }

    [Theory]
    [InlineData(23, 22, 7, true)]
    [InlineData(6, 22, 7, true)]
    [InlineData(12, 22, 7, false)]
    [InlineData(9, 9, 17, true)]
    [InlineData(18, 9, 17, false)]
    public void IsWithinQuietHours_ReturnsExpectedValue(int current, int start, int end, bool expected)
    {
        var method = typeof(ElBruno.CopilotCLIMonitor.App)
            .GetMethod("IsWithinQuietHours", BindingFlags.NonPublic | BindingFlags.Static);

        Assert.NotNull(method);
        var isQuiet = method!.Invoke(null, [current, start, end]);
        Assert.Equal(expected, isQuiet);
    }
}
