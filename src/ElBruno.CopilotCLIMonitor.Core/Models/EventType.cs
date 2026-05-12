namespace ElBruno.CopilotCLIMonitor.Core.Models;

public enum EventType
{
    TaskCompleted,
    ApprovalRequired,
    Error,
    Warning,
    BuildCompleted,
    TestCompleted,
    WorkflowCompleted,
    LongRunningTaskWarning,
    HookFailed,
    Unknown
}
