using System.Text;
using ElBruno.CopilotCLIMonitor.Cli.Handlers;
using ElBruno.CopilotCLIMonitor.Tests.Fakes;

namespace ElBruno.CopilotCLIMonitor.Tests.Commands;

public class NotifyHandlerTests
{
    private readonly FakeRepositoryDetector _detector = new();
    private readonly FakeHookInstaller _installer = new();
    private readonly FakeIpcClient _ipc = new();
    private readonly StringBuilder _out = new();
    private readonly StringBuilder _err = new();

    private CliCommandHandlers BuildSut() => new(
        _detector, _installer, _ipc,
        new StringWriter(_out),
        new StringWriter(_err));

    [Fact]
    public async Task Notify_WhenMonitorRunning_ReturnsZero()
    {
        _ipc.IsRunning = true;
        var sut = BuildSut();
        var exit = await sut.RunNotifyAsync(["--event", "task-completed", "--message", "Done"]);
        Assert.Equal(0, exit);
    }

    [Fact]
    public async Task Notify_WhenMonitorRunning_SendsRequestToIpc()
    {
        _ipc.IsRunning = true;
        var sut = BuildSut();
        await sut.RunNotifyAsync(["--event", "task-completed", "--message", "Migration done"]);

        var req = Assert.Single(_ipc.SentRequests);
        Assert.Equal("task-completed", req.Event);
        Assert.Equal("Migration done", req.Message);
    }

    [Fact]
    public async Task Notify_WhenMonitorNotRunning_ReturnsNonZero()
    {
        _ipc.IsRunning = false;
        var sut = BuildSut();
        var exit = await sut.RunNotifyAsync(["--event", "error", "--message", "Build failed"]);
        Assert.NotEqual(0, exit);
    }

    [Fact]
    public async Task Notify_WithoutEvent_ReturnsOne()
    {
        var sut = BuildSut();
        var exit = await sut.RunNotifyAsync(["--message", "Done"]);
        Assert.Equal(1, exit);
    }

    [Fact]
    public async Task Notify_WithoutEvent_WritesErrorMessage()
    {
        var sut = BuildSut();
        await sut.RunNotifyAsync(["--message", "Done"]);
        Assert.Contains("--event is required", _err.ToString());
    }

    [Fact]
    public async Task Notify_WithoutMessage_DefaultsToEventName()
    {
        _ipc.IsRunning = true;
        var sut = BuildSut();
        await sut.RunNotifyAsync(["--event", "error"]);

        var req = Assert.Single(_ipc.SentRequests);
        Assert.Equal("error", req.Message);
    }

    [Fact]
    public async Task Notify_WithRepository_ForwardsToIpc()
    {
        _ipc.IsRunning = true;
        var sut = BuildSut();
        await sut.RunNotifyAsync(["--event", "task-completed", "--message", "Done", "--repository", "myrepo"]);

        var req = Assert.Single(_ipc.SentRequests);
        Assert.Equal("myrepo", req.Repository);
    }

    [Fact]
    public async Task Notify_WithBranch_ForwardsToIpc()
    {
        _ipc.IsRunning = true;
        var sut = BuildSut();
        await sut.RunNotifyAsync(["--event", "task-completed", "--message", "Done", "--branch", "feature/ai"]);

        var req = Assert.Single(_ipc.SentRequests);
        Assert.Equal("feature/ai", req.Branch);
    }

    [Fact]
    public async Task Notify_WithShortFlags_ParsesCorrectly()
    {
        _ipc.IsRunning = true;
        var sut = BuildSut();
        await sut.RunNotifyAsync(["-e", "task-completed", "-m", "Done", "-r", "repo", "-b", "main"]);

        var req = Assert.Single(_ipc.SentRequests);
        Assert.Equal("task-completed", req.Event);
        Assert.Equal("Done", req.Message);
        Assert.Equal("repo", req.Repository);
        Assert.Equal("main", req.Branch);
    }
}
