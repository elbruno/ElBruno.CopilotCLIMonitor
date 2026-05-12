using ElBruno.CopilotCLIMonitor.Core.Services;

namespace ElBruno.CopilotCLIMonitor.Tests.Services;

public class HookInstallerTests : IDisposable
{
    private readonly string _tempDir;
    private readonly HookInstaller _sut = new();

    public HookInstallerTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), $"copilotmon-installer-{Guid.NewGuid():N}");
        Directory.CreateDirectory(_tempDir);
    }

    public void Dispose() => Directory.Delete(_tempDir, recursive: true);

    [Fact]
    public void Install_ValidRepo_ReturnsSuccess()
    {
        var result = _sut.Install(_tempDir);
        Assert.True(result.Success);
        Assert.Null(result.ErrorMessage);
    }

    [Fact]
    public void Install_ValidRepo_CreatesHookDirectory()
    {
        _sut.Install(_tempDir);
        var hookDir = Path.Combine(_tempDir, ".copilotclimonitor");
        Assert.True(Directory.Exists(hookDir));
    }

    [Fact]
    public void Install_ValidRepo_CreatesNotifyScript()
    {
        _sut.Install(_tempDir);
        var scriptPath = Path.Combine(_tempDir, ".copilotclimonitor", "notify.ps1");
        Assert.True(File.Exists(scriptPath));
    }

    [Fact]
    public void Install_ValidRepo_NotifyScriptContainsCopilotmonCommand()
    {
        _sut.Install(_tempDir);
        var scriptPath = Path.Combine(_tempDir, ".copilotclimonitor", "notify.ps1");
        var content = File.ReadAllText(scriptPath);
        Assert.Contains("copilotmon notify", content);
    }

    [Fact]
    public void Install_ValidRepo_CreatesConfigJson()
    {
        _sut.Install(_tempDir);
        var configPath = Path.Combine(_tempDir, ".copilotclimonitor", "config.json");
        Assert.True(File.Exists(configPath));
    }

    [Fact]
    public void Install_ValidRepo_ConfigJsonContainsNotificationsEnabled()
    {
        _sut.Install(_tempDir);
        var configPath = Path.Combine(_tempDir, ".copilotclimonitor", "config.json");
        var content = File.ReadAllText(configPath);
        Assert.Contains("notificationsEnabled", content);
    }

    [Fact]
    public void Install_ValidRepo_ReturnsInstalledFilePaths()
    {
        var result = _sut.Install(_tempDir);
        Assert.NotNull(result.InstalledFiles);
        Assert.Equal(2, result.InstalledFiles.Count);
    }

    [Fact]
    public void Install_NonExistentDirectory_ReturnsFailure()
    {
        var result = _sut.Install(@"C:\this-path-does-not-exist-8f3a2b9c");
        Assert.False(result.Success);
        Assert.NotNull(result.ErrorMessage);
    }

    [Fact]
    public void Install_WhenCalledTwice_DoesNotOverwriteExistingConfig()
    {
        _sut.Install(_tempDir);
        var configPath = Path.Combine(_tempDir, ".copilotclimonitor", "config.json");
        var firstWriteTime = File.GetLastWriteTimeUtc(configPath);

        // Short sleep ensures file timestamps differ if overwritten
        Thread.Sleep(10);
        _sut.Install(_tempDir);

        var secondWriteTime = File.GetLastWriteTimeUtc(configPath);
        Assert.Equal(firstWriteTime, secondWriteTime);
    }
}
