namespace ElBruno.CopilotCLIMonitor.Core.Models;

public record MonitorEvent(
    EventType EventType,
    string Message,
    string? Repository = null,
    string? Branch = null,
    DateTimeOffset? Timestamp = null)
{
    // Capture UtcNow at construction so repeated access is stable.
    private readonly DateTimeOffset _capturedAt = DateTimeOffset.UtcNow;
    public DateTimeOffset OccurredAt => Timestamp ?? _capturedAt;

    public static MonitorEvent Parse(string eventTypeName, string message, string? repository = null, string? branch = null)
    {
        var eventType = eventTypeName.ToLowerInvariant() switch
        {
            "task-completed"      => EventType.TaskCompleted,
            "approval-required"   => EventType.ApprovalRequired,
            "error"               => EventType.Error,
            "warning"             => EventType.Warning,
            "build-completed"     => EventType.BuildCompleted,
            "test-completed"      => EventType.TestCompleted,
            "workflow-completed"  => EventType.WorkflowCompleted,
            "long-running-task"   => EventType.LongRunningTaskWarning,
            "hook-failed"         => EventType.HookFailed,
            _                     => EventType.Unknown
        };

        return new MonitorEvent(eventType, message, repository, branch);
    }
}
