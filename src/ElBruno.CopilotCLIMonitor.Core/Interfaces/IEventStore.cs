using ElBruno.CopilotCLIMonitor.Core.Models;

namespace ElBruno.CopilotCLIMonitor.Core.Interfaces;

/// <summary>In-memory store for recent <see cref="MonitorEvent"/> entries.</summary>
public interface IEventStore
{
    /// <summary>Appends an event, evicting the oldest entry when the capacity limit is reached.</summary>
    void Add(MonitorEvent monitorEvent);

    /// <summary>Removes all stored events.</summary>
    void Clear();

    /// <summary>Returns a snapshot of events in arrival order.</summary>
    IReadOnlyList<MonitorEvent> Recent { get; }
}
