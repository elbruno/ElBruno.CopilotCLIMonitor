using ElBruno.CopilotCLIMonitor.Core.Interfaces;
using ElBruno.CopilotCLIMonitor.Core.Models;

namespace ElBruno.CopilotCLIMonitor.Tests.Fakes;

/// <summary>In-memory <see cref="IEventStore"/> for use in tests.</summary>
public sealed class FakeEventStore : IEventStore
{
    private readonly List<MonitorEvent> _events = [];

    public void Add(MonitorEvent monitorEvent) => _events.Add(monitorEvent);
    public void Clear() => _events.Clear();
    public IReadOnlyList<MonitorEvent> Recent => _events.AsReadOnly();
}
