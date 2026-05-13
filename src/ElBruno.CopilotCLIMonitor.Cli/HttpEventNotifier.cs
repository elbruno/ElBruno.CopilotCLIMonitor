using ElBruno.CopilotCLIMonitor.Core.Interfaces;
using ElBruno.CopilotCLIMonitor.Core.Models;
using ElBruno.CopilotCLIMonitor.Core.Services;
using Microsoft.Extensions.Logging;

namespace ElBruno.CopilotCLIMonitor.Cli;

/// <summary>
/// Sends notification events to the running systray app via the local HTTP IPC endpoint.
/// Falls back to logging a warning if the monitor is not running.
/// </summary>
public sealed class HttpEventNotifier : IEventNotifier
{
    private readonly IIpcClient _client;
    private readonly ILogger<HttpEventNotifier> _logger;

    public HttpEventNotifier(IIpcClient? client = null, ILogger<HttpEventNotifier>? logger = null)
    {
        _client = client ?? new HttpIpcClient();
        _logger = logger ?? Microsoft.Extensions.Logging.Abstractions.NullLogger<HttpEventNotifier>.Instance;
        _logger.LogInformation("HttpEventNotifier initialised.");
    }

    public async Task NotifyAsync(MonitorEvent monitorEvent, CancellationToken cancellationToken = default)
    {
        var request = new NotifyRequest(
            monitorEvent.EventType.ToEventString(),
            monitorEvent.Message,
            monitorEvent.Repository,
            monitorEvent.Branch);

        var sent = await _client.SendNotifyAsync(request, cancellationToken);

        if (sent)
        {
            _logger.LogInformation("Event '{EventType}' sent to monitor successfully.", monitorEvent.EventType);
        }
        else
        {
            _logger.LogWarning(
                "Monitor not running. Event '{EventType}' could not be delivered — {Message}. Start the monitor with: copilotclimon",
                monitorEvent.EventType, monitorEvent.Message);
        }
    }
}
