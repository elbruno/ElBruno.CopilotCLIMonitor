namespace ElBruno.CopilotCLIMonitor.Core.Interfaces;

public interface IHookInstaller
{
    /// <summary>Installs Copilot CLI hooks in the given repository root.</summary>
    HookInstallResult Install(string repositoryRoot);
}

public record HookInstallResult(
    bool Success,
    string? ErrorMessage = null,
    IReadOnlyList<string>? InstalledFiles = null);
