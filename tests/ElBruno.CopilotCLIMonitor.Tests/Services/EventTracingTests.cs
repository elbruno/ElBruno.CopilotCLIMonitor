using ElBruno.CopilotCLIMonitor.Telemetry;

namespace ElBruno.CopilotCLIMonitor.Tests.Services;

public sealed class EventTracingTests
{
    [Fact]
    public void EventSource_HasExpectedName()
    {
        Assert.Equal("ElBruno-CopilotCliMonitor", CopilotCliMonitorEventSource.Log.Name);
    }

    [Fact]
    public void EventSource_MethodsCanBeInvoked()
    {
        CopilotCliMonitorEventSource.Log.AppStartup();
        CopilotCliMonitorEventSource.Log.EventReceived("TaskCompleted");
        CopilotCliMonitorEventSource.Log.NotificationShown("TaskCompleted");
        CopilotCliMonitorEventSource.Log.NotificationSuppressed("Paused");
    }
}
