using ElBruno.CopilotCLIMonitor.Core.Models;
using ElBruno.CopilotCLIMonitor.Core.Services;

namespace ElBruno.CopilotCLIMonitor.Tests.Services;

public class EventStoreTests
{
    private readonly EventStore _sut = new();

    [Fact]
    public void Add_SingleEvent_CanBeRetrievedViaRecent()
    {
        var evt = new MonitorEvent(EventType.TaskCompleted, "Done");
        _sut.Add(evt);
        Assert.Single(_sut.Recent);
    }

    [Fact]
    public void Recent_AfterAdd_ContainsAddedEvent()
    {
        var evt = new MonitorEvent(EventType.Error, "Build failed");
        _sut.Add(evt);
        Assert.Contains(evt, _sut.Recent);
    }

    [Fact]
    public void Clear_AfterAddingEvents_ReturnsEmptyRecent()
    {
        _sut.Add(new MonitorEvent(EventType.TaskCompleted, "done"));
        _sut.Clear();
        Assert.Empty(_sut.Recent);
    }

    [Fact]
    public void Add_BeyondMaxCapacity_DropsOldestEvents()
    {
        // Add 201 events – only 200 should be kept
        for (int i = 0; i < 201; i++)
            _sut.Add(new MonitorEvent(EventType.Warning, $"msg-{i}"));

        Assert.Equal(200, _sut.Recent.Count);
    }

    [Fact]
    public void Add_BeyondMaxCapacity_KeepsNewestEvents()
    {
        for (int i = 0; i < 201; i++)
            _sut.Add(new MonitorEvent(EventType.Warning, $"msg-{i}"));

        // Most recent event is msg-200
        Assert.Equal("msg-200", _sut.Recent[^1].Message);
    }

    [Fact]
    public void Recent_ReturnsSnapshot_NotLiveReference()
    {
        _sut.Add(new MonitorEvent(EventType.TaskCompleted, "first"));
        var snapshot = _sut.Recent;

        _sut.Add(new MonitorEvent(EventType.TaskCompleted, "second"));

        Assert.Single(snapshot);     // snapshot is frozen
        Assert.Equal(2, _sut.Recent.Count);
    }

    [Fact]
    public async Task Add_IsThreadSafe_DoesNotThrowUnderConcurrentLoad()
    {
        var tasks = Enumerable.Range(0, 50)
            .Select(i => Task.Run(() => _sut.Add(new MonitorEvent(EventType.TaskCompleted, $"msg-{i}"))))
            .ToArray();

        await Task.WhenAll(tasks);
        Assert.InRange(_sut.Recent.Count, 1, 200);
    }
}
