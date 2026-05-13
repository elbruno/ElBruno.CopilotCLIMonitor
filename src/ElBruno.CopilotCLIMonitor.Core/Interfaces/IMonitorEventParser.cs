using ElBruno.CopilotCLIMonitor.Core.Models;

namespace ElBruno.CopilotCLIMonitor.Core.Interfaces;

/// <summary>
/// Parses raw string payloads into strongly-typed <see cref="MonitorEvent"/> instances.
/// </summary>
public interface IMonitorEventParser
{
    /// <summary>Converts a raw event-type name and message into a <see cref="MonitorEvent"/>.</summary>
    MonitorEvent Parse(string eventTypeName, string message, string? repository = null, string? branch = null);
}
