using ElBruno.CopilotCLIMonitor.Core.Interfaces;
using ElBruno.CopilotCLIMonitor.Core.Models;
using ElBruno.CopilotCLIMonitor.Core.Services;

namespace ElBruno.CopilotCLIMonitor.Cli.Handlers;

/// <summary>
/// Testable command handlers for the copilotclimon CLI.
/// All I/O goes through injected interfaces so tests can swap them out.
/// </summary>
public sealed class CliCommandHandlers(
    IRepositoryDetector detector,
    IHookInstaller installer,
    IIpcClient ipcClient,
    TextWriter? stdout = null,
    TextWriter? stderr = null)
{
    private readonly TextWriter _out = stdout ?? Console.Out;
    private readonly TextWriter _err = stderr ?? Console.Error;

    // ── notify ────────────────────────────────────────────────────────────────

    public async Task<int> RunNotifyAsync(string[] args)
    {
        string? eventName = null;
        string? message = null;
        string? repository = null;
        string? branch = null;
        string? source = null;

        for (int i = 0; i < args.Length; i++)
        {
            switch (args[i])
            {
                case "--event" or "-e" when i + 1 < args.Length:
                    eventName = args[++i]; break;
                case "--message" or "-m" when i + 1 < args.Length:
                    message = args[++i]; break;
                case "--repository" or "--repo" or "-r" when i + 1 < args.Length:
                    repository = args[++i]; break;
                case "--branch" or "-b" when i + 1 < args.Length:
                    branch = args[++i]; break;
                case "--source" or "-s" when i + 1 < args.Length:
                    source = args[++i]; break;
            }
        }

        if (string.IsNullOrWhiteSpace(eventName))
        {
            await _err.WriteLineAsync("Error: --event is required.");
            return 1;
        }

        message ??= eventName;

        var cwd = Directory.GetCurrentDirectory();
        repository ??= detector.GetRepositoryName(cwd);
        branch ??= detector.GetCurrentBranch(cwd);

        var request = new NotifyRequest(eventName, message, repository, branch, source);
        if (!NotifyRequestValidator.TryValidate(request, out var validationError))
        {
            await _err.WriteLineAsync($"Error: {validationError}");
            return 1;
        }

        var sent = await ipcClient.SendNotifyAsync(request);

        if (sent)
        {
            await _out.WriteLineAsync($"✓ Event '{eventName}' sent to monitor.");
            return 0;
        }

        await _err.WriteLineAsync("✗ Monitor is not running. Start it with: copilotclimon");
        return 2;
    }

    // ── init ──────────────────────────────────────────────────────────────────

    public async Task<int> RunInitAsync(string[] args)
    {
        var cwd = Directory.GetCurrentDirectory();
        var repoRoot = detector.DetectRepositoryRoot(cwd);

        if (repoRoot is null)
        {
            await _err.WriteLineAsync("Error: Not inside a git repository. Run 'git init' first.");
            return 1;
        }

        await _out.WriteLineAsync($"Repository detected: {repoRoot}");

        var result = installer.Install(repoRoot);

        if (result.Success)
        {
            await _out.WriteLineAsync("✓ Hook integration installed successfully.");
            if (result.InstalledFiles is { Count: > 0 } files)
                foreach (var f in files)
                    await _out.WriteLineAsync($"    {f}");
            return 0;
        }

        await _err.WriteLineAsync($"✗ Hook installation failed: {result.ErrorMessage}");
        return 1;
    }

    // ── doctor ────────────────────────────────────────────────────────────────

    public async Task<int> RunDoctorAsync(string[] args)
    {
        await _out.WriteLineAsync("copilotclimon doctor — validating setup");

        var allOk = true;
        var cwd = Directory.GetCurrentDirectory();
        var repoRoot = detector.DetectRepositoryRoot(cwd);

        if (repoRoot is not null)
        {
            await _out.WriteLineAsync($"✓ Git repository: {repoRoot}");
            var branch = detector.GetCurrentBranch(cwd);
            if (branch is not null)
                await _out.WriteLineAsync($"  Branch: {branch}");

            var hookDir = Path.Combine(repoRoot, ".copilotclimonitor");
            if (Directory.Exists(hookDir))
            {
                await _out.WriteLineAsync("✓ Hook directory exists: .copilotclimonitor");
                var script = Path.Combine(hookDir, "notify.ps1");
                var config = Path.Combine(hookDir, "config.json");
                await _out.WriteLineAsync(File.Exists(script) ? "✓ notify.ps1 present" : "✗ notify.ps1 missing — run: copilotclimon init");
                await _out.WriteLineAsync(File.Exists(config) ? "✓ config.json present" : "✗ config.json missing — run: copilotclimon init");
                if (File.Exists(config))
                {
                    if (RepositoryHookConfigValidator.TryValidateFile(config, out var configError))
                    {
                        await _out.WriteLineAsync("✓ config.json schema is valid");
                    }
                    else
                    {
                        await _out.WriteLineAsync($"✗ config.json invalid: {configError}");
                        allOk = false;
                    }
                }
            }
            else
            {
                await _out.WriteLineAsync("✗ Hook directory not found — run: copilotclimon init");
                allOk = false;
            }
        }
        else
        {
            await _out.WriteLineAsync("⚠ Not inside a git repository (hooks are optional)");
        }

        var running = await ipcClient.IsRunningAsync();
        if (running)
            await _out.WriteLineAsync($"✓ Monitor is running at {IpcConstants.HealthUrl()}");
        else
        {
            await _out.WriteLineAsync("✗ Monitor is not running — start it with: copilotclimon");
            allOk = false;
        }

        await _out.WriteLineAsync(allOk ? "All checks passed." : "Some checks failed.");
        return allOk ? 0 : 1;
    }

    // ── open ──────────────────────────────────────────────────────────────────

    public async Task<int> RunOpenAsync(string[] args)
    {
        var running = await ipcClient.IsRunningAsync();
        if (!running)
        {
            await _err.WriteLineAsync("Monitor is not running. Start it with: copilotclimon");
            return 1;
        }

        await _out.WriteLineAsync("Opening monitor dashboard…");
        return 0;
    }
}
