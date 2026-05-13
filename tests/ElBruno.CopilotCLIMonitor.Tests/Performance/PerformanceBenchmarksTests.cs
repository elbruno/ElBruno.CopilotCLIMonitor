using System.Diagnostics;
using ElBruno.CopilotCLIMonitor.Core.Models;
using ElBruno.CopilotCLIMonitor.Core.Services;

namespace ElBruno.CopilotCLIMonitor.Tests.Performance;

public sealed class PerformanceBenchmarksTests
{
    [Fact]
    public void EventStore_AddHighVolumeEvents_CompletesWithinBudget()
    {
        var store = new EventStore(capacity: 5000);
        var sw = Stopwatch.StartNew();

        for (var i = 0; i < 20_000; i++)
        {
            store.Add(new MonitorEvent(EventType.TaskCompleted, $"perf-{i}", "repo", "main"));
        }

        sw.Stop();
        Assert.InRange(sw.ElapsedMilliseconds, 0, 5000);
        Assert.Equal(5000, store.Recent.Count);
    }

    [Fact]
    public void DashboardFilter_LargeDataset_CompletesWithinBudget()
    {
        var events = Enumerable.Range(0, 10_000)
            .Select(i => new MonitorEvent(
                i % 2 == 0 ? EventType.TaskCompleted : EventType.Warning,
                $"message-{i}",
                $"repo-{i % 10}",
                "main"))
            .ToList();

        var sw = Stopwatch.StartNew();
        var filtered = DashboardWindow.FilterEvents(events, "repo-3", EventType.Warning.ToString());
        sw.Stop();

        Assert.InRange(sw.ElapsedMilliseconds, 0, 3000);
        Assert.All(filtered, item => Assert.Equal(EventType.Warning, item.EventType));
    }

    [Fact]
    public void EventStore_ConcurrentLoad_CompletesWithinBudget()
    {
        var store = new EventStore(capacity: 2000);
        var sw = Stopwatch.StartNew();

        Parallel.For(0, 10_000, i =>
        {
            store.Add(new MonitorEvent(EventType.WorkflowCompleted, $"parallel-{i}"));
        });

        sw.Stop();
        Assert.InRange(sw.ElapsedMilliseconds, 0, 5000);
        Assert.Equal(2000, store.Recent.Count);
    }
}
