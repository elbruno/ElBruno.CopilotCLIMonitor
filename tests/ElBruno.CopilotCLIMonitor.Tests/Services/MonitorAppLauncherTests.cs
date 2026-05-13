using System.Diagnostics;
using ElBruno.CopilotCLIMonitor.Cli.Services;

namespace ElBruno.CopilotCLIMonitor.Tests.Services;

public sealed class MonitorAppLauncherTests
{
    [Fact]
    public async Task LaunchAsync_WhenAlreadyRunning_ReturnsZeroWithoutStartingProcess()
    {
        var launcher = new MonitorAppLauncher(
            isRunningAsync: _ => Task.FromResult(true),
            startProcess: _ => throw new InvalidOperationException("Process start should not be called."),
            resolveExecutablePath: () => null);

        var exitCode = await launcher.LaunchAsync();

        Assert.Equal(0, exitCode);
    }

    [Fact]
    public async Task LaunchAsync_WhenExecutableCannotBeResolved_ReturnsOne()
    {
        var launcher = new MonitorAppLauncher(
            isRunningAsync: _ => Task.FromResult(false),
            startProcess: _ => throw new InvalidOperationException("Process start should not be called."),
            resolveExecutablePath: () => null);

        var exitCode = await launcher.LaunchAsync();

        Assert.Equal(1, exitCode);
    }

    [Fact]
    public async Task LaunchAsync_WhenExecutableExists_StartsProcessAndReturnsZero()
    {
        var tempFile = Path.Combine(Path.GetTempPath(), $"copilotclimon-launcher-{Guid.NewGuid():N}.exe");
        await File.WriteAllTextAsync(tempFile, "stub");

        try
        {
            ProcessStartInfo? capturedStartInfo = null;
            var runningChecks = 0;

            var launcher = new MonitorAppLauncher(
                isRunningAsync: _ =>
                {
                    runningChecks++;
                    return Task.FromResult(runningChecks > 1);
                },
                startProcess: psi =>
                {
                    capturedStartInfo = psi;
                    return Process.GetCurrentProcess();
                },
                resolveExecutablePath: () => tempFile);

            var exitCode = await launcher.LaunchAsync();

            Assert.Equal(0, exitCode);
            Assert.NotNull(capturedStartInfo);
            Assert.Equal(tempFile, capturedStartInfo!.FileName);
            Assert.Equal(Path.GetDirectoryName(tempFile), capturedStartInfo.WorkingDirectory);
        }
        finally
        {
            if (File.Exists(tempFile))
            {
                File.Delete(tempFile);
            }
        }
    }
}
