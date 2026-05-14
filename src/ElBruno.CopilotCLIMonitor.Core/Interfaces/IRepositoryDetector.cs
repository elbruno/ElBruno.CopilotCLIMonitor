namespace ElBruno.CopilotCLIMonitor.Core.Interfaces;

public interface IRepositoryDetector
{
    /// <summary>Returns the repository root path, or null if not in a git repo.</summary>
    string? DetectRepositoryRoot(string workingDirectory);

    /// <summary>Returns the repository name derived from the root path, or null.</summary>
    string? GetRepositoryName(string workingDirectory);

    /// <summary>Returns the current branch name, or null if unavailable.</summary>
    string? GetCurrentBranch(string workingDirectory);

    /// <summary>Returns the git origin remote URL, or null if unavailable.</summary>
    string? GetOriginRepository(string workingDirectory);
}
