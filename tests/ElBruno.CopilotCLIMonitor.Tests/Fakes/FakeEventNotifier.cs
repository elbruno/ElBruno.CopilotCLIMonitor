using ElBruno.CopilotCLIMonitor.Core.Interfaces;
using ElBruno.CopilotCLIMonitor.Core.Models;

namespace ElBruno.CopilotCLIMonitor.Tests.Fakes;

public sealed class FakeEventNotifier : IEventNotifier
{
    public List<MonitorEvent> ReceivedEvents { get; } = [];

    public Task NotifyAsync(MonitorEvent monitorEvent, CancellationToken cancellationToken = default)
    {
        ReceivedEvents.Add(monitorEvent);
        return Task.CompletedTask;
    }
}
