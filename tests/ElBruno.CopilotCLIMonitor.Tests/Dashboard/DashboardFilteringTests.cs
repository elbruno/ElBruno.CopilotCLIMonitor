using ElBruno.CopilotCLIMonitor.Core.Models;

namespace ElBruno.CopilotCLIMonitor.Tests.Dashboard;

public class DashboardFilteringTests
{
    [Fact]
    public void FilterEvents_ByEventType_ReturnsMatchingEvents()
    {
        var events = new[]
        {
            new MonitorEvent(EventType.TaskCompleted, "done", "repo-a", "main"),
            new MonitorEvent(EventType.Error, "failed", "repo-b", "main")
        };

        var filtered = DashboardWindow.FilterEvents(events, null, "Error");
        Assert.Single(filtered);
        Assert.Equal(EventType.Error, filtered[0].EventType);
    }

    [Fact]
    public void FilterEvents_ByText_MatchesMessageAndRepository()
    {
        var events = new[]
        {
            new MonitorEvent(EventType.TaskCompleted, "migration complete", "repo-a", "main"),
            new MonitorEvent(EventType.Warning, "slow build", "repo-b", "feature/test")
        };

        var byMessage = DashboardWindow.FilterEvents(events, "migration", "All");
        var byRepo = DashboardWindow.FilterEvents(events, "repo-b", "All");

        Assert.Single(byMessage);
        Assert.Single(byRepo);
        Assert.Equal("repo-b", byRepo[0].Repository);
    }

    [Fact]
    public void FilterEvents_ByText_MatchesOriginRepository()
    {
        var events = new[]
        {
            new MonitorEvent(
                EventType.TaskCompleted,
                "migration complete",
                "repo-a",
                "main",
                OriginRepository: "https://github.com/elbruno/repo-a.git"),
            new MonitorEvent(
                EventType.Warning,
                "slow build",
                "repo-b",
                "feature/test",
                OriginRepository: "https://github.com/elbruno/repo-b.git")
        };

        var filtered = DashboardWindow.FilterEvents(events, "repo-b.git", "All");

        Assert.Single(filtered);
        Assert.Equal("repo-b", filtered[0].Repository);
    }
}
