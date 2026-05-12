using ElBruno.CopilotCLIMonitor.Core.Interfaces;
using ElBruno.CopilotCLIMonitor.Core.Models;
using ElBruno.CopilotCLIMonitor.Core.Services;

namespace ElBruno.CopilotCLIMonitor.Cli;

/// <summary>
/// Sends notification events to the running systray app via the local HTTP IPC endpoint.
/// Falls back to console output if the monitor is not running.
/// </summary>
public sealed class HttpEventNotifier : IEventNotifier
{
    private readonly HttpIpcClient _client = new();

    public async Task NotifyAsync(MonitorEvent monitorEvent, CancellationToken cancellationToken = default)
    {
        var request = new NotifyRequest(
            monitorEvent.EventType.ToString().ToLowerInvariant().Replace("unknown", "custom"),
            monitorEvent.Message,
            monitorEvent.Repository,
            monitorEvent.Branch);

        var sent = await _client.SendNotifyAsync(request, cancellationToken);

        if (sent)
        {
            Console.WriteLine($"✓ Event '{monitorEvent.EventType}' sent to monitor.");
        }
        else
        {
            Console.Error.WriteLine($"✗ Monitor not running. Event: {monitorEvent.EventType} — {monitorEvent.Message}");
            Console.Error.WriteLine($"  Start the monitor with: copilotmon");
        }
    }
}
