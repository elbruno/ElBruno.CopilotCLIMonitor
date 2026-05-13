using System.Diagnostics;
using ElBruno.CopilotCLIMonitor.Core.Interfaces;
using ElBruno.CopilotCLIMonitor.Core.Services;
using ElBruno.CopilotCLIMonitor.Core.Interfaces;

namespace ElBruno.CopilotCLIMonitor.Tests.Services;

public class HookInstallerTests : IDisposable
{
    private readonly string _tempDir;
    private readonly HookInstaller _sut = new();

    public HookInstallerTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), $"copilotclimon-installer-{Guid.NewGuid():N}");
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
    public void Install_ValidRepo_NotifyScriptContainsCopilotclimonCommand()
    {
        _sut.Install(_tempDir);
        var scriptPath = Path.Combine(_tempDir, ".copilotclimonitor", "notify.ps1");
        var content = File.ReadAllText(scriptPath);
        Assert.Contains("copilotclimon notify", content);
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
    public void Install_ValidRepo_ConfigJsonContainsSensibleDefaults()
    {
        _sut.Install(_tempDir);
        var configPath = Path.Combine(_tempDir, ".copilotclimonitor", "config.json");
        var content = File.ReadAllText(configPath);

        Assert.Contains("\"version\": \"1.0\"", content);
        Assert.Contains("\"enabled\": true", content);
        Assert.Contains("\"quietHours\"", content);
        Assert.Contains("\"build-completed\"", content);
        Assert.Contains("\"sourceTagging\": true", content);
    }

    [Fact]
    public void Install_ValidRepo_CreatesCopilotHookConfig()
    {
        _sut.Install(_tempDir);
        var hookConfigPath = Path.Combine(_tempDir, ".github", "hooks", "copilotclimon-notify.json");
        Assert.True(File.Exists(hookConfigPath));
    }

    [Fact]
    public void Install_ValidRepo_CopilotHookConfigContainsAgentStop()
    {
        _sut.Install(_tempDir);
        var hookConfigPath = Path.Combine(_tempDir, ".github", "hooks", "copilotclimon-notify.json");
        var content = File.ReadAllText(hookConfigPath);
        Assert.Contains("\"agentStop\"", content);
        Assert.Contains("notify.ps1", content);
    }

    [Fact]
    public void Install_WithSelectedHooks_WritesOnlySelectedHookEntries()
    {
        _sut.Install(_tempDir, new HookInstallOptions(EnabledHookTriggers: ["sessionEnd"]));
        var hookConfigPath = Path.Combine(_tempDir, ".github", "hooks", "copilotclimon-notify.json");
        var content = File.ReadAllText(hookConfigPath);
        Assert.Contains("\"sessionEnd\"", content);
        Assert.DoesNotContain("\"agentStop\"", content);
        Assert.DoesNotContain("\"errorOccurred\"", content);
    }

    [Fact]
    public void Install_ValidRepo_ReturnsInstalledFilePaths()
    {
        var result = _sut.Install(_tempDir);
        Assert.NotNull(result.InstalledFiles);
        Assert.Equal(3, result.InstalledFiles.Count);
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

        Thread.Sleep(10);
        _sut.Install(_tempDir);

        var secondWriteTime = File.GetLastWriteTimeUtc(configPath);
        Assert.Equal(firstWriteTime, secondWriteTime);
    }

    [Fact]
    public void Install_WithForce_OverwritesExistingConfig()
    {
        _sut.Install(_tempDir);
        var configPath = Path.Combine(_tempDir, ".copilotclimonitor", "config.json");
        File.WriteAllText(configPath, "{}");
        var firstWriteTime = File.GetLastWriteTimeUtc(configPath);

        Thread.Sleep(10);
        _sut.Install(_tempDir, new HookInstallOptions(Force: true));

        var secondWriteTime = File.GetLastWriteTimeUtc(configPath);
        var content = File.ReadAllText(configPath);
        Assert.True(secondWriteTime > firstWriteTime);
        Assert.Contains("notificationsEnabled", content);
    }

    [Fact]
    public void Install_WithForce_OverwritesExistingHookConfig()
    {
        _sut.Install(_tempDir);
        var hookConfigPath = Path.Combine(_tempDir, ".github", "hooks", "copilotclimon-notify.json");
        File.WriteAllText(hookConfigPath, "{}");
        var firstWriteTime = File.GetLastWriteTimeUtc(hookConfigPath);

        Thread.Sleep(10);
        _sut.Install(_tempDir, new HookInstallOptions(Force: true));

        var secondWriteTime = File.GetLastWriteTimeUtc(hookConfigPath);
        var content = File.ReadAllText(hookConfigPath);
        Assert.True(secondWriteTime > firstWriteTime);
        Assert.Contains("\"agentStop\"", content);
    }

    [Fact]
    public void Install_NotifyScript_WhenExecuted_ForwardsEventAndMessageToCli()
    {
        _sut.Install(_tempDir);

        var scriptPath = Path.Combine(_tempDir, ".copilotclimonitor", "notify.ps1");
        var argsFile = Path.Combine(_tempDir, "captured-args.txt");
        var fakeCliPath = Path.Combine(_tempDir, "copilotclimon.cmd");

        File.WriteAllText(fakeCliPath, $"@echo off{Environment.NewLine}echo %* > \"{argsFile}\"{Environment.NewLine}");

        var psi = new ProcessStartInfo
        {
            FileName = "pwsh",
            Arguments = $"-NoProfile -ExecutionPolicy Bypass -File \"{scriptPath}\" -Event \"error\" -Message \"Build failed\"",
            WorkingDirectory = _tempDir,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        };
        psi.Environment["PATH"] = $"{_tempDir};{Environment.GetEnvironmentVariable("PATH")}";

        using var process = Process.Start(psi);
        Assert.NotNull(process);
        process!.WaitForExit();

        var stdErr = process.StandardError.ReadToEnd();
        Assert.Equal(0, process.ExitCode);
        Assert.True(File.Exists(argsFile), $"Expected captured args file to exist. stderr: {stdErr}");

        var captured = File.ReadAllText(argsFile);
        Assert.Contains("notify --event error --message \"Build failed\"", captured);
    }
}
