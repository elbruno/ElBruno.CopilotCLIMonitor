using System.Diagnostics.Tracing;

namespace ElBruno.CopilotCLIMonitor.Telemetry;

[EventSource(Name = "ElBruno-CopilotCliMonitor")]
public sealed class CopilotCliMonitorEventSource : EventSource
{
    public static CopilotCliMonitorEventSource Log { get; } = new();

    private CopilotCliMonitorEventSource() { }

    [Event(1, Level = EventLevel.Informational, Message = "Application startup")]
    public void AppStartup() => WriteEvent(1);

    [Event(2, Level = EventLevel.Informational, Message = "Event received: {0}")]
    public void EventReceived(string eventType) => WriteEvent(2, eventType);

    [Event(3, Level = EventLevel.Warning, Message = "Notification suppressed: {0}")]
    public void NotificationSuppressed(string reason) => WriteEvent(3, reason);

    [Event(4, Level = EventLevel.Informational, Message = "Notification shown: {0}")]
    public void NotificationShown(string eventType) => WriteEvent(4, eventType);
}
