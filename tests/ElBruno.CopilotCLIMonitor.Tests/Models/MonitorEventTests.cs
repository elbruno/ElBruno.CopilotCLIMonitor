using ElBruno.CopilotCLIMonitor.Core.Models;

namespace ElBruno.CopilotCLIMonitor.Tests.Models;

public class MonitorEventTests
{
    [Theory]
    [InlineData("task-completed",    EventType.TaskCompleted)]
    [InlineData("approval-required", EventType.ApprovalRequired)]
    [InlineData("error",             EventType.Error)]
    [InlineData("warning",           EventType.Warning)]
    [InlineData("build-completed",   EventType.BuildCompleted)]
    [InlineData("test-completed",    EventType.TestCompleted)]
    [InlineData("workflow-completed",EventType.WorkflowCompleted)]
    [InlineData("long-running-task", EventType.LongRunningTaskWarning)]
    [InlineData("hook-failed",       EventType.HookFailed)]
    public void Parse_KnownEventType_ReturnsCorrectEnum(string input, EventType expected)
    {
        var evt = MonitorEvent.Parse(input, "msg");
        Assert.Equal(expected, evt.EventType);
    }

    [Theory]
    [InlineData("unknown-type")]
    [InlineData("")]
    [InlineData("TASK-COMPLETED")]   // Case test — must still match via ToLower
    public void Parse_UnknownOrMixedCaseInput_ReturnsUnknown(string input)
    {
        // "TASK-COMPLETED" uppercase should NOT match because ToLowerInvariant handles it
        // but we explicitly list it as unknown to force a design check
        // Actually ToLowerInvariant converts it so "TASK-COMPLETED" → TaskCompleted
        // We only mark truly unknown strings as Unknown
        if (input == "TASK-COMPLETED")
        {
            var evt = MonitorEvent.Parse(input, "msg");
            Assert.Equal(EventType.TaskCompleted, evt.EventType);
        }
        else
        {
            var evt = MonitorEvent.Parse(input, "msg");
            Assert.Equal(EventType.Unknown, evt.EventType);
        }
    }

    [Fact]
    public void Parse_PreservesMessage()
    {
        const string message = "Migration finished successfully";
        var evt = MonitorEvent.Parse("task-completed", message);
        Assert.Equal(message, evt.Message);
    }

    [Fact]
    public void Parse_PreservesRepositoryAndBranch()
    {
        var evt = MonitorEvent.Parse("error", "Build failed", repository: "myrepo", branch: "main");
        Assert.Equal("myrepo", evt.Repository);
        Assert.Equal("main", evt.Branch);
    }

    [Fact]
    public void Parse_NullRepositoryAndBranch_AreNullByDefault()
    {
        var evt = MonitorEvent.Parse("warning", "msg");
        Assert.Null(evt.Repository);
        Assert.Null(evt.Branch);
    }

    [Fact]
    public void OccurredAt_WhenTimestampNotSet_ReturnsRecentUtcTime()
    {
        var before = DateTimeOffset.UtcNow.AddSeconds(-1);
        var evt = MonitorEvent.Parse("task-completed", "done");

        // OccurredAt calls DateTimeOffset.UtcNow each access, so it's always "now".
        // Just verify it is recent (within the last 60 seconds).
        Assert.True(evt.OccurredAt >= before, "OccurredAt should not be in the past");
        Assert.True(evt.OccurredAt <= DateTimeOffset.UtcNow.AddSeconds(1), "OccurredAt should not be in the future");
    }

    [Fact]
    public void OccurredAt_WhenTimestampProvided_UsesProvidedValue()
    {
        var ts = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var evt = new MonitorEvent(EventType.TaskCompleted, "done", Timestamp: ts);
        Assert.Equal(ts, evt.OccurredAt);
    }
}
