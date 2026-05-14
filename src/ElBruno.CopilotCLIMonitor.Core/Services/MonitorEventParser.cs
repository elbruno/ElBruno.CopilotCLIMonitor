using ElBruno.CopilotCLIMonitor.Core.Interfaces;
using ElBruno.CopilotCLIMonitor.Core.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace ElBruno.CopilotCLIMonitor.Core.Services;

/// <summary>
/// Parses raw event-type strings into <see cref="MonitorEvent"/> instances,
/// logging each parse operation and any unrecognised event types.
/// </summary>
public sealed class MonitorEventParser : IMonitorEventParser
{
    private readonly ILogger<MonitorEventParser> _logger;

    public MonitorEventParser(ILogger<MonitorEventParser>? logger = null)
    {
        _logger = logger ?? NullLogger<MonitorEventParser>.Instance;
        _logger.LogInformation("MonitorEventParser initialised.");
    }

    /// <inheritdoc/>
    public MonitorEvent Parse(
        string eventTypeName,
        string message,
        string? repository = null,
        string? branch = null,
        string? source = null,
        string? originRepository = null)
    {
        _logger.LogInformation(
            "Parsing event: type={EventType} repo={Repository} branch={Branch} source={Source} origin={Origin}",
            eventTypeName,
            repository ?? "(none)",
            branch ?? "(none)",
            source ?? "(none)",
            originRepository ?? "(none)");

        var evt = MonitorEvent.Parse(eventTypeName, message, repository, branch, source, originRepository);

        if (evt.EventType == EventType.Unknown)
            _logger.LogWarning("Unrecognised event type '{EventType}' — mapped to Unknown.", eventTypeName);

        return evt;
    }
}
