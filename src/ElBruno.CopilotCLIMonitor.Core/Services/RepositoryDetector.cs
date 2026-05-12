using System.Diagnostics;
using ElBruno.CopilotCLIMonitor.Core.Interfaces;

namespace ElBruno.CopilotCLIMonitor.Core.Services;

public class RepositoryDetector : IRepositoryDetector
{
    public string? DetectRepositoryRoot(string workingDirectory)
    {
        if (!Directory.Exists(workingDirectory))
            return null;

        try
        {
            var result = RunGit(workingDirectory, "rev-parse --show-toplevel");
            return result?.Trim();
        }
        catch
        {
            return null;
        }
    }

    public string? GetRepositoryName(string workingDirectory)
    {
        var root = DetectRepositoryRoot(workingDirectory);
        if (root is null) return null;
        return Path.GetFileName(root.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
    }

    public string? GetCurrentBranch(string workingDirectory)
    {
        if (!Directory.Exists(workingDirectory))
            return null;

        try
        {
            // Try the most reliable method first (works even with orphan branches)
            var result = RunGit(workingDirectory, "branch --show-current");
            if (!string.IsNullOrWhiteSpace(result?.Trim()))
                return result.Trim();

            // Fallback for older git versions
            var refResult = RunGit(workingDirectory, "symbolic-ref --short HEAD");
            var branch = refResult?.Trim();
            return string.IsNullOrWhiteSpace(branch) ? null : branch;
        }
        catch
        {
            return null;
        }
    }

    private static string? RunGit(string workingDirectory, string arguments)
    {
        var psi = new ProcessStartInfo("git", arguments)
        {
            WorkingDirectory = workingDirectory,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = Process.Start(psi);
        if (process is null) return null;

        var output = process.StandardOutput.ReadToEnd();
        process.WaitForExit();

        return process.ExitCode == 0 ? output : null;
    }
}
