using ElBruno.CopilotCLIMonitor.Core.Models;

namespace ElBruno.CopilotCLIMonitor.Tests.Models;

public class EventTypeExtensionsTests
{
    [Theory]
    [InlineData(EventType.TaskCompleted,          "task-completed")]
    [InlineData(EventType.ApprovalRequired,       "approval-required")]
    [InlineData(EventType.Error,                  "error")]
    [InlineData(EventType.Warning,                "warning")]
    [InlineData(EventType.BuildCompleted,         "build-completed")]
    [InlineData(EventType.TestCompleted,          "test-completed")]
    [InlineData(EventType.WorkflowCompleted,      "workflow-completed")]
    [InlineData(EventType.LongRunningTaskWarning, "long-running-task")]
    [InlineData(EventType.HookFailed,             "hook-failed")]
    [InlineData(EventType.Unknown,                "unknown")]
    public void ToEventString_ReturnsHyphenatedString(EventType input, string expected)
    {
        Assert.Equal(expected, input.ToEventString());
    }

    [Theory]
    [InlineData(EventType.TaskCompleted)]
    [InlineData(EventType.ApprovalRequired)]
    [InlineData(EventType.Error)]
    [InlineData(EventType.Warning)]
    [InlineData(EventType.BuildCompleted)]
    [InlineData(EventType.TestCompleted)]
    [InlineData(EventType.WorkflowCompleted)]
    [InlineData(EventType.LongRunningTaskWarning)]
    [InlineData(EventType.HookFailed)]
    public void ToEventString_ThenParse_RoundTrips(EventType original)
    {
        // This is the IPC round-trip: CLI serialises the enum → string → WPF parses back.
        var eventString = original.ToEventString();
        var parsed = MonitorEvent.Parse(eventString, "msg").EventType;
        Assert.Equal(original, parsed);
    }
}
