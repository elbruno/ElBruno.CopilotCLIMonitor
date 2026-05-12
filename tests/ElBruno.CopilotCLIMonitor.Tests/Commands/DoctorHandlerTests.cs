using System.Text;
using ElBruno.CopilotCLIMonitor.Cli.Handlers;
using ElBruno.CopilotCLIMonitor.Tests.Fakes;

namespace ElBruno.CopilotCLIMonitor.Tests.Commands;

public class DoctorHandlerTests : IDisposable
{
    private readonly FakeRepositoryDetector _detector = new();
    private readonly FakeHookInstaller _installer = new();
    private readonly FakeIpcClient _ipc = new();
    private readonly StringBuilder _out = new();
    private readonly StringBuilder _err = new();
    private readonly string _tempDir;

    public DoctorHandlerTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), $"copilotmon-doctor-{Guid.NewGuid():N}");
        Directory.CreateDirectory(_tempDir);
    }

    public void Dispose() => Directory.Delete(_tempDir, recursive: true);

    private CliCommandHandlers BuildSut() => new(
        _detector, _installer, _ipc,
        new StringWriter(_out),
        new StringWriter(_err));

    [Fact]
    public async Task Doctor_WhenMonitorRunningAndHooksInstalled_ReturnsZero()
    {
        _ipc.IsRunning = true;
        _detector.RootToReturn = _tempDir;
        _detector.BranchToReturn = "main";

        // Create hook files
        var hookDir = Path.Combine(_tempDir, ".copilotclimonitor");
        Directory.CreateDirectory(hookDir);
        File.WriteAllText(Path.Combine(hookDir, "notify.ps1"), "");
        File.WriteAllText(Path.Combine(hookDir, "config.json"), "{}");

        var exit = await BuildSut().RunDoctorAsync([]);
        Assert.Equal(0, exit);
    }

    [Fact]
    public async Task Doctor_WhenMonitorNotRunning_ReturnsOne()
    {
        _ipc.IsRunning = false;
        _detector.RootToReturn = _tempDir;

        var exit = await BuildSut().RunDoctorAsync([]);
        Assert.Equal(1, exit);
    }

    [Fact]
    public async Task Doctor_WhenMonitorNotRunning_WritesWarning()
    {
        _ipc.IsRunning = false;
        _detector.RootToReturn = null;
        await BuildSut().RunDoctorAsync([]);
        Assert.Contains("not running", _out.ToString());
    }

    [Fact]
    public async Task Doctor_WhenHooksDirMissing_ReturnsOne()
    {
        _ipc.IsRunning = true;
        _detector.RootToReturn = _tempDir;
        // No .copilotclimonitor directory created

        var exit = await BuildSut().RunDoctorAsync([]);
        Assert.Equal(1, exit);
    }

    [Fact]
    public async Task Doctor_WhenNotInGitRepo_ReturnsOne()
    {
        _ipc.IsRunning = false;
        _detector.RootToReturn = null;
        var exit = await BuildSut().RunDoctorAsync([]);
        Assert.Equal(1, exit);
    }

    [Fact]
    public async Task Doctor_WhenNotInGitRepo_OutputsMentionsOptional()
    {
        _ipc.IsRunning = true;
        _detector.RootToReturn = null;
        await BuildSut().RunDoctorAsync([]);
        Assert.Contains("optional", _out.ToString());
    }

    [Fact]
    public async Task Doctor_OutputContainsValidatingLine()
    {
        _detector.RootToReturn = null;
        _ipc.IsRunning = false;
        await BuildSut().RunDoctorAsync([]);
        Assert.Contains("copilotmon doctor", _out.ToString());
    }
}
