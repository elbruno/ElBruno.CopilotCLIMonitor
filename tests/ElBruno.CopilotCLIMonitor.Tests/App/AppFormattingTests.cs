using System.Reflection;
using System.Windows.Forms;
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
        var summary = method!.Invoke(null, [41234, true]) as string;

        Assert.NotNull(summary);
        Assert.Contains("IPC Port: 41234", summary);
        Assert.Contains("Authentication Token: Configured", summary);
    }
}
