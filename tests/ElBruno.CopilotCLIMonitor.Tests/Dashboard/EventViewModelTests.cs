using ElBruno.CopilotCLIMonitor.Core.Models;

namespace ElBruno.CopilotCLIMonitor.Tests.Dashboard;

public class EventViewModelTests
{
    [Fact]
    public void Constructor_WhenRepositoryAndBranchMissing_UsesEmptyStrings()
    {
        var ev = new MonitorEvent(EventType.Warning, "Heads up", null, null, DateTimeOffset.UtcNow);
        var vm = new EventViewModel(ev);

        Assert.Equal(string.Empty, vm.Repository);
        Assert.Equal(string.Empty, vm.Branch);
    }

    [Fact]
    public void Constructor_MapsEventTypeAndMessage()
    {
        var ev = new MonitorEvent(EventType.BuildCompleted, "Build done", "demo", "main", DateTimeOffset.UtcNow);
        var vm = new EventViewModel(ev);

        Assert.Equal("BuildCompleted", vm.EventTypeDisplay);
        Assert.Equal("Build done", vm.Message);
        Assert.Equal("demo", vm.Repository);
        Assert.Equal("main", vm.Branch);
    }
}
