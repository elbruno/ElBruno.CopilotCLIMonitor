using ElBruno.CopilotCLIMonitor.Core.Interfaces;
using ElBruno.CopilotCLIMonitor.Core.Models;

namespace ElBruno.CopilotCLIMonitor.Core.Services;

/// <summary>Thread-safe in-memory ring buffer of recent monitor events.</summary>
public sealed class EventStore : IEventStore
{
    private readonly List<MonitorEvent> _events = [];
    private readonly object _lock = new();

    /// <summary>Maximum number of events retained. Oldest entries are evicted when the limit is exceeded.</summary>
    public int Capacity { get; }

    /// <param name="capacity">Maximum number of events to retain. Defaults to 200.</param>
    public EventStore(int capacity = 200)
    {
        if (capacity <= 0) throw new ArgumentOutOfRangeException(nameof(capacity), "Capacity must be positive.");
        Capacity = capacity;
    }

    /// <inheritdoc/>
    public void Add(MonitorEvent monitorEvent)
    {
        lock (_lock)
        {
            _events.Add(monitorEvent);
            if (_events.Count > Capacity)
                _events.RemoveAt(0);
        }
    }

    /// <inheritdoc/>
    public void Clear()
    {
        lock (_lock) { _events.Clear(); }
    }

    /// <inheritdoc/>
    public IReadOnlyList<MonitorEvent> Recent
    {
        get { lock (_lock) { return [.. _events]; } }
    }
}
