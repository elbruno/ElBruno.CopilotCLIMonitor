using ElBruno.CopilotCLIMonitor.Core.Models;

namespace ElBruno.CopilotCLIMonitor.Core.Interfaces;

public interface IEventNotifier
{
    Task NotifyAsync(MonitorEvent monitorEvent, CancellationToken cancellationToken = default);
}
