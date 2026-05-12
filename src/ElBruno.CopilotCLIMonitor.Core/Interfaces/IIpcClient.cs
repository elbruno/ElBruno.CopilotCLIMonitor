using ElBruno.CopilotCLIMonitor.Core.Models;

namespace ElBruno.CopilotCLIMonitor.Core.Interfaces;

/// <summary>Sends a notification event to the running systray app via IPC.</summary>
public interface IIpcClient
{
    Task<bool> SendNotifyAsync(NotifyRequest request, CancellationToken cancellationToken = default);
    Task<bool> IsRunningAsync(CancellationToken cancellationToken = default);
}
