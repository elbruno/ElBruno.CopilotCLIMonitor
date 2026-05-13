namespace ElBruno.CopilotCLIMonitor.Core.Models;

/// <summary>Conversion helpers between <see cref="EventType"/> and the hyphenated string form used in IPC payloads.</summary>
public static class EventTypeExtensions
{
    /// <summary>
    /// Returns the canonical hyphenated string representation used in <c>NotifyRequest.Event</c>.
    /// Must stay in sync with the switch in <see cref="MonitorEvent.Parse"/>.
    /// </summary>
    public static string ToEventString(this EventType eventType) => eventType switch
    {
        EventType.TaskCompleted         => "task-completed",
        EventType.ApprovalRequired      => "approval-required",
        EventType.Error                 => "error",
        EventType.Warning               => "warning",
        EventType.BuildCompleted        => "build-completed",
        EventType.TestCompleted         => "test-completed",
        EventType.WorkflowCompleted     => "workflow-completed",
        EventType.LongRunningTaskWarning => "long-running-task",
        EventType.HookFailed            => "hook-failed",
        _                               => "unknown"
    };
}
