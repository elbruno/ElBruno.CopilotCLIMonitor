using ElBruno.CopilotCLIMonitor.Core.Models;

namespace ElBruno.CopilotCLIMonitor.Services;

/// <summary>In-memory ring buffer of recent monitor events.</summary>
public sealed class EventStore
{
    private readonly List<MonitorEvent> _events = [];
    private readonly object _lock = new();
    private const int MaxEvents = 200;

    public void Add(MonitorEvent e)
    {
        lock (_lock)
        {
            _events.Add(e);
            if (_events.Count > MaxEvents)
                _events.RemoveAt(0);
        }
    }

    public void Clear()
    {
        lock (_lock) { _events.Clear(); }
    }

    public IReadOnlyList<MonitorEvent> Recent
    {
        get { lock (_lock) { return [.. _events]; } }
    }
}
