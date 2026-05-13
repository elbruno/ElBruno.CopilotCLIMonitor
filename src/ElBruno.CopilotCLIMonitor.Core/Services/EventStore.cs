using ElBruno.CopilotCLIMonitor.Core.Interfaces;
using ElBruno.CopilotCLIMonitor.Core.Models;
using Microsoft.Extensions.Logging;

namespace ElBruno.CopilotCLIMonitor.Core.Services;

/// <summary>Thread-safe in-memory ring buffer of recent monitor events.</summary>
public sealed class EventStore : IEventStore
{
    private readonly Queue<MonitorEvent> _events;
    private readonly object _lock = new();
    private readonly ILogger<EventStore> _logger;

    /// <summary>Maximum number of events retained. Oldest entries are evicted when the limit is exceeded.</summary>
    public int Capacity { get; }

    /// <param name="capacity">Maximum number of events to retain. Defaults to 200.</param>
    /// <param name="logger">Logger injected by DI.</param>
    public EventStore(int capacity = 200, ILogger<EventStore>? logger = null)
    {
        if (capacity <= 0) throw new ArgumentOutOfRangeException(nameof(capacity), "Capacity must be positive.");
        Capacity = capacity;
        _events = new Queue<MonitorEvent>(capacity);
        _logger = logger ?? Microsoft.Extensions.Logging.Abstractions.NullLogger<EventStore>.Instance;
        _logger.LogInformation("EventStore initialised with capacity {Capacity}.", Capacity);
    }

    /// <inheritdoc/>
    public void Add(MonitorEvent monitorEvent)
    {
        lock (_lock)
        {
            _events.Enqueue(monitorEvent);
            if (_events.Count > Capacity)
            {
                _logger.LogWarning(
                    "EventStore capacity {Capacity} reached — evicting oldest event (type={EventType}).",
                    Capacity, _events.Peek().EventType);
                _events.Dequeue();
            }
        }
    }

    /// <inheritdoc/>
    public void Clear()
    {
        lock (_lock)
        {
            _logger.LogInformation("EventStore cleared ({Count} events removed).", _events.Count);
            _events.Clear();
        }
    }

    /// <inheritdoc/>
    public IReadOnlyList<MonitorEvent> Recent
    {
        get { lock (_lock) { return [.. _events.ToArray()]; } }
    }
}
