using System.Diagnostics;
using ElBruno.CopilotCLIMonitor.Core.Models;
using ElBruno.CopilotCLIMonitor.Core.Services;
using Microsoft.Extensions.Logging;

namespace ElBruno.CopilotCLIMonitor.Cli.Services;

public sealed class MonitorAppLauncher
{
    private readonly Func<CancellationToken, Task<bool>> _isRunningAsync;
    private readonly Func<ProcessStartInfo, Process?> _startProcess;
    private readonly Func<string?> _resolveExecutablePath;
    private readonly ILogger<MonitorAppLauncher>? _logger;

    public MonitorAppLauncher(
        ILogger<MonitorAppLauncher>? logger = null,
        Func<CancellationToken, Task<bool>>? isRunningAsync = null,
        Func<ProcessStartInfo, Process?>? startProcess = null,
        Func<string?>? resolveExecutablePath = null)
    {
        var ipcClient = new HttpIpcClient(IpcConstants.DefaultPort);
        _logger = logger;
        _isRunningAsync = isRunningAsync ?? (_ => ipcClient.IsRunningAsync());
        _startProcess = startProcess ?? Process.Start;
        _resolveExecutablePath = resolveExecutablePath ?? ResolveMonitorExecutablePath;
    }

    public async Task<int> LaunchAsync(CancellationToken cancellationToken = default)
    {
        if (await _isRunningAsync(cancellationToken))
        {
            Console.WriteLine("✓ Monitor is already running.");
            return 0;
        }

        var executablePath = _resolveExecutablePath();
        if (string.IsNullOrWhiteSpace(executablePath))
        {
            Console.Error.WriteLine("✗ Could not find ElBruno.CopilotCLIMonitor.exe.");
            Console.Error.WriteLine("Install the desktop app (portable/installer/chocolatey), then run: copilotclimon");
            return 1;
        }

        var startInfo = new ProcessStartInfo
        {
            FileName = executablePath,
            WorkingDirectory = Path.GetDirectoryName(executablePath) ?? Directory.GetCurrentDirectory(),
            UseShellExecute = true
        };

        var process = _startProcess(startInfo);
        if (process is null)
        {
            Console.Error.WriteLine($"✗ Failed to launch monitor app from: {executablePath}");
            return 1;
        }

        _logger?.LogInformation("Started monitor app from {ExecutablePath}.", executablePath);

        if (await WaitForMonitorAsync(cancellationToken))
        {
            Console.WriteLine("✓ Monitor started.");
            return 0;
        }

        Console.WriteLine("⚠ Monitor launch requested, but IPC is not ready yet.");
        Console.WriteLine("  Wait a few seconds and run: copilotclimon doctor");
        return 0;
    }

    public static string? ResolveMonitorExecutablePath()
    {
        var candidates = BuildCandidatePaths();
        foreach (var candidate in candidates)
        {
            if (!string.IsNullOrWhiteSpace(candidate) && File.Exists(candidate))
            {
                return candidate;
            }
        }

        return null;
    }

    private static IReadOnlyList<string> BuildCandidatePaths()
    {
        var candidates = new List<string>();

        var fromEnv = Environment.GetEnvironmentVariable("COPILOTCLIMON_APP_PATH");
        if (!string.IsNullOrWhiteSpace(fromEnv))
        {
            candidates.Add(fromEnv);
        }

        var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        if (!string.IsNullOrWhiteSpace(localAppData))
        {
            candidates.Add(Path.Combine(localAppData, "Programs", "ElBruno.CopilotCLIMonitor", "ElBruno.CopilotCLIMonitor.exe"));
        }

        var programData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
        if (!string.IsNullOrWhiteSpace(programData))
        {
            candidates.Add(Path.Combine(programData, "ElBruno.CopilotCLIMonitor", "ElBruno.CopilotCLIMonitor.exe"));
        }

        candidates.Add(Path.Combine(AppContext.BaseDirectory, "ElBruno.CopilotCLIMonitor.exe"));
        candidates.Add(Path.Combine(Directory.GetCurrentDirectory(), "ElBruno.CopilotCLIMonitor.exe"));

        return candidates
            .Where(path => !string.IsNullOrWhiteSpace(path))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    private async Task<bool> WaitForMonitorAsync(CancellationToken cancellationToken)
    {
        const int attempts = 10;
        for (var i = 0; i < attempts; i++)
        {
            if (await _isRunningAsync(cancellationToken))
            {
                return true;
            }

            await Task.Delay(TimeSpan.FromMilliseconds(500), cancellationToken);
        }

        return false;
    }
}
