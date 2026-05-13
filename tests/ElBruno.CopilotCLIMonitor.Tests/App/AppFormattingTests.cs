using System.Reflection;
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
}
