using System.Text;
using ElBruno.CopilotCLIMonitor.Cli.Handlers;
using ElBruno.CopilotCLIMonitor.Core.Interfaces;
using ElBruno.CopilotCLIMonitor.Tests.Fakes;

namespace ElBruno.CopilotCLIMonitor.Tests.Commands;

public class InitHandlerTests : IDisposable
{
    private readonly FakeRepositoryDetector _detector = new();
    private readonly FakeHookInstaller _installer = new();
    private readonly FakeIpcClient _ipc = new();
    private readonly StringBuilder _out = new();
    private readonly StringBuilder _err = new();
    private readonly string _tempDir;

    public InitHandlerTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), $"copilotmon-init-handler-{Guid.NewGuid():N}");
        Directory.CreateDirectory(_tempDir);
    }

    public void Dispose() => Directory.Delete(_tempDir, recursive: true);

    private CliCommandHandlers BuildSut() => new(
        _detector, _installer, _ipc,
        new StringWriter(_out),
        new StringWriter(_err));

    [Fact]
    public async Task Init_WhenRepoDetected_ReturnsZero()
    {
        _detector.RootToReturn = _tempDir;
        var exit = await BuildSut().RunInitAsync([]);
        Assert.Equal(0, exit);
    }

    [Fact]
    public async Task Init_WhenRepoDetected_CallsInstaller()
    {
        _detector.RootToReturn = _tempDir;
        await BuildSut().RunInitAsync([]);
        Assert.Equal(_tempDir, _installer.LastInstalledRoot);
    }

    [Fact]
    public async Task Init_WhenNoRepo_ReturnsOne()
    {
        _detector.RootToReturn = null;
        var exit = await BuildSut().RunInitAsync([]);
        Assert.Equal(1, exit);
    }

    [Fact]
    public async Task Init_WhenNoRepo_WritesErrorToStderr()
    {
        _detector.RootToReturn = null;
        await BuildSut().RunInitAsync([]);
        Assert.Contains("git repository", _err.ToString());
    }

    [Fact]
    public async Task Init_WhenNoRepo_DoesNotCallInstaller()
    {
        _detector.RootToReturn = null;
        await BuildSut().RunInitAsync([]);
        Assert.Null(_installer.LastInstalledRoot);
    }

    [Fact]
    public async Task Init_WhenInstallerFails_ReturnsOne()
    {
        _detector.RootToReturn = _tempDir;
        _installer.ResultToReturn = new HookInstallResult(false, "Permission denied");
        var exit = await BuildSut().RunInitAsync([]);
        Assert.Equal(1, exit);
    }

    [Fact]
    public async Task Init_WhenInstallerFails_WritesErrorToStderr()
    {
        _detector.RootToReturn = _tempDir;
        _installer.ResultToReturn = new HookInstallResult(false, "Permission denied");
        await BuildSut().RunInitAsync([]);
        Assert.Contains("Permission denied", _err.ToString());
    }

    [Fact]
    public async Task Init_WhenSuccess_WritesSuccessMessage()
    {
        _detector.RootToReturn = _tempDir;
        await BuildSut().RunInitAsync([]);
        Assert.Contains("installed successfully", _out.ToString());
    }
}
