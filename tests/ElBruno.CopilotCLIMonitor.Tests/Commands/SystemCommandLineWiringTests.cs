using System.CommandLine;
using ElBruno.CopilotCLIMonitor.Commands;
using ElBruno.CopilotCLIMonitor.Core.Models;
using ElBruno.CopilotCLIMonitor.Core.Services;
using ElBruno.CopilotCLIMonitor.Tests.Fakes;

namespace ElBruno.CopilotCLIMonitor.Tests.Commands;

/// <summary>
/// Tests for the System.CommandLine-wired command builders in Cli/Commands/.
/// These validate CLI argument parsing and command wiring end-to-end.
/// </summary>
public class SystemCommandLineWiringTests : IDisposable
{
    private readonly FakeEventNotifier _notifier = new();
    private readonly FakeRepositoryDetector _detector = new();
    private readonly FakeHookInstaller _installer = new();
    private readonly string _tempDir;

    public SystemCommandLineWiringTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), $"copilotclimon-scl-{Guid.NewGuid():N}");
        Directory.CreateDirectory(_tempDir);
    }

    public void Dispose() => Directory.Delete(_tempDir, recursive: true);

    // ── notify command ────────────────────────────────────────────────────────

    [Fact]
    public async Task Notify_WithRequiredArgs_InvokesNotifier()
    {
        var root = new RootCommand { NotifyCommand.Build(_notifier) };
        await root.Parse(["notify", "--event", "task-completed", "--message", "Done"]).InvokeAsync();
        Assert.Single(_notifier.ReceivedEvents);
    }

    [Fact]
    public async Task Notify_TaskCompleted_ParsesEventTypeCorrectly()
    {
        var root = new RootCommand { NotifyCommand.Build(_notifier) };
        await root.Parse(["notify", "--event", "task-completed", "--message", "Done"]).InvokeAsync();
        var evt = Assert.Single(_notifier.ReceivedEvents);
        Assert.Equal(EventType.TaskCompleted, evt.EventType);
    }

    [Fact]
    public async Task Notify_WithoutRequiredEvent_DoesNotInvokeNotifier()
    {
        var root = new RootCommand { NotifyCommand.Build(_notifier) };
        await root.Parse(["notify", "--message", "Done"]).InvokeAsync();
        Assert.Empty(_notifier.ReceivedEvents);
    }

    [Fact]
    public async Task Notify_WithRepository_PassesRepoToNotifier()
    {
        var root = new RootCommand { NotifyCommand.Build(_notifier) };
        await root.Parse(["notify", "--event", "error", "--message", "Fail", "--repository", "myrepo"]).InvokeAsync();
        var evt = Assert.Single(_notifier.ReceivedEvents);
        Assert.Equal("myrepo", evt.Repository);
    }

    [Fact]
    public async Task Notify_UnknownEvent_ForwardsAsUnknownEventType()
    {
        var root = new RootCommand { NotifyCommand.Build(_notifier) };
        await root.Parse(["notify", "--event", "custom-type", "--message", "msg"]).InvokeAsync();
        var evt = Assert.Single(_notifier.ReceivedEvents);
        Assert.Equal(EventType.Unknown, evt.EventType);
    }

    // ── init command ──────────────────────────────────────────────────────────

    [Fact]
    public async Task Init_WhenRepoFound_ReturnsZero()
    {
        _detector.RootToReturn = _tempDir;
        var root = new RootCommand { InitCommand.Build(_detector, _installer) };
        var exit = await root.Parse(["init"]).InvokeAsync();
        Assert.Equal(0, exit);
    }

    [Fact]
    public async Task Init_WhenRepoFound_CallsInstaller()
    {
        _detector.RootToReturn = _tempDir;
        var root = new RootCommand { InitCommand.Build(_detector, _installer) };
        await root.Parse(["init"]).InvokeAsync();
        Assert.Equal(_tempDir, _installer.LastInstalledRoot);
    }

    [Fact]
    public async Task Init_WhenNoRepo_ReturnsOne()
    {
        _detector.RootToReturn = null;
        var root = new RootCommand { InitCommand.Build(_detector, _installer) };
        var exit = await root.Parse(["init"]).InvokeAsync();
        Assert.Equal(1, exit);
    }

    [Fact]
    public async Task Init_WhenInstallerFails_ReturnsOne()
    {
        _detector.RootToReturn = _tempDir;
        _installer.ResultToReturn = new Core.Interfaces.HookInstallResult(false, "Access denied");
        var root = new RootCommand { InitCommand.Build(_detector, _installer) };
        var exit = await root.Parse(["init"]).InvokeAsync();
        Assert.Equal(1, exit);
    }

    [Fact]
    public async Task Init_WithDefaultFlag_SetsUseDefaultSelection()
    {
        _detector.RootToReturn = _tempDir;
        var root = new RootCommand { InitCommand.Build(_detector, _installer) };
        var exit = await root.Parse(["init", "--default"]).InvokeAsync();
        Assert.Equal(0, exit);
        Assert.True(_installer.LastOptions?.UseDefaultSelection);
    }

    [Fact]
    public async Task Init_WithForceFlag_SetsForce()
    {
        _detector.RootToReturn = _tempDir;
        var root = new RootCommand { InitCommand.Build(_detector, _installer) };
        var exit = await root.Parse(["init", "--force", "--default"]).InvokeAsync();
        Assert.Equal(0, exit);
        Assert.True(_installer.LastOptions?.Force);
    }

    // ── upgrade command ────────────────────────────────────────────────────────

    [Fact]
    public async Task Upgrade_WhenRepoFound_ReturnsZeroAndForcesInstall()
    {
        _detector.RootToReturn = _tempDir;
        var root = new RootCommand { UpgradeCommand.Build(_detector, _installer) };
        var exit = await root.Parse(["upgrade"]).InvokeAsync();
        Assert.Equal(0, exit);
        Assert.True(_installer.LastOptions?.Force);
        Assert.True(_installer.LastOptions?.UseDefaultSelection);
    }

    [Fact]
    public async Task Upgrade_WhenNoRepo_ReturnsOne()
    {
        _detector.RootToReturn = null;
        var root = new RootCommand { UpgradeCommand.Build(_detector, _installer) };
        var exit = await root.Parse(["upgrade"]).InvokeAsync();
        Assert.Equal(1, exit);
    }

    // ── doctor command ────────────────────────────────────────────────────────

    [Fact]
    public async Task Doctor_Always_ReturnsZero()
    {
        var root = new RootCommand { DoctorCommand.Build(_detector) };
        var exit = await root.Parse(["doctor"]).InvokeAsync();
        Assert.Equal(0, exit);
    }

    [Fact]
    public async Task Doctor_WhenHooksConfigured_PrintsSelectedHooksFromInit()
    {
        _detector.RootToReturn = _tempDir;

        var hookDir = Path.Combine(_tempDir, ".copilotclimonitor");
        Directory.CreateDirectory(hookDir);
        File.WriteAllText(Path.Combine(hookDir, "notify.ps1"), "");
        File.WriteAllText(Path.Combine(hookDir, "config.json"), "{}");

        var githubHooksDir = Path.Combine(_tempDir, ".github", "hooks");
        Directory.CreateDirectory(githubHooksDir);
        File.WriteAllText(
            Path.Combine(githubHooksDir, "copilotclimon-notify.json"),
            """
            {
              "version": 1,
              "hooks": {
                "agentStop": [
                  { "type": "command", "powershell": ".\\.copilotclimonitor\\notify.ps1 -Event task-completed -Message \"Copilot task completed\"", "cwd": ".", "timeoutSec": 10 }
                ],
                "userPromptSubmitted": [
                  { "type": "command", "powershell": ".\\.copilotclimonitor\\notify.ps1 -Event warning -Message \"Copilot prompt submitted\"", "cwd": ".", "timeoutSec": 10 }
                ]
              }
            }
            """);

        var originalOut = Console.Out;
        using var writer = new StringWriter();
        Console.SetOut(writer);

        try
        {
            var root = new RootCommand { DoctorCommand.Build(_detector) };
            await root.Parse(["doctor"]).InvokeAsync();
        }
        finally
        {
            Console.SetOut(originalOut);
        }

        Assert.Contains("Hooks selected in init: agentStop, userPromptSubmitted", writer.ToString());
    }

    [Fact]
    public async Task InitThenDoctor_EndToEnd_PrintsDefaultHookSelection()
    {
        _detector.RootToReturn = _tempDir;

        var root = new RootCommand
        {
            InitCommand.Build(_detector, new HookInstaller()),
            DoctorCommand.Build(_detector)
        };

        var originalOut = Console.Out;
        using var writer = new StringWriter();
        Console.SetOut(writer);

        try
        {
            var initExitCode = await root.Parse(["init", "--default"]).InvokeAsync();
            var doctorExitCode = await root.Parse(["doctor"]).InvokeAsync();

            Assert.Equal(0, initExitCode);
            Assert.Equal(0, doctorExitCode);
        }
        finally
        {
            Console.SetOut(originalOut);
        }

        Assert.Contains("Hooks selected in init: agentStop, errorOccurred", writer.ToString());
    }

    // ── update command ────────────────────────────────────────────────────────

    [Fact]
    public async Task Update_WhenLatestIsNewer_ReturnsZero()
    {
        var root = new RootCommand { UpdateCommand.Build() };
        var exit = await root.Parse(["update", "--current-version", "1.0.0", "--latest-version", "1.1.0"]).InvokeAsync();
        Assert.Equal(0, exit);
    }

    [Fact]
    public async Task Diagnostic_Enable_ReturnsZero()
    {
        var previous = Environment.GetEnvironmentVariable("COPILOTCLIMON_DIAGNOSTIC_MODE");
        try
        {
            var root = new RootCommand { DiagnosticCommand.Build() };
            var exit = await root.Parse(["diagnostic", "--enable"]).InvokeAsync();
            Assert.Equal(0, exit);
            Assert.Equal("true", Environment.GetEnvironmentVariable("COPILOTCLIMON_DIAGNOSTIC_MODE"));
        }
        finally
        {
            Environment.SetEnvironmentVariable("COPILOTCLIMON_DIAGNOSTIC_MODE", previous);
        }
    }

    [Fact]
    public async Task Diagnostic_EnableAndDisableTogether_ReturnsOne()
    {
        var root = new RootCommand { DiagnosticCommand.Build() };
        var exit = await root.Parse(["diagnostic", "--enable", "--disable"]).InvokeAsync();
        Assert.Equal(1, exit);
    }

    // ── command metadata ─────────────────────────────────────────────────────

    [Fact]
    public void RootCommand_Description_ContainsCopilotclimon()
    {
        var root = new RootCommand("copilotclimon — GitHub Copilot CLI task monitor");
        Assert.Contains("copilotclimon", root.Description);
    }

    [Fact]
    public void InitCommand_Description_ContainsCopilotclimon()
    {
        var cmd = InitCommand.Build(_detector, _installer);
        Assert.Contains("copilotclimon", cmd.Description);
    }

    [Fact]
    public void DoctorCommand_Description_ContainsCopilotclimon()
    {
        var cmd = DoctorCommand.Build(_detector);
        Assert.Contains("copilotclimon", cmd.Description);
    }

    [Fact]
    public void UpdateCommand_Description_ContainsUpdate()
    {
        var cmd = UpdateCommand.Build();
        Assert.Contains("update", cmd.Description, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void DiagnosticCommand_Description_ContainsDiagnostic()
    {
        var cmd = DiagnosticCommand.Build();
        Assert.Contains("diagnostic", cmd.Description, StringComparison.OrdinalIgnoreCase);
    }
}
