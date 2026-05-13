using ElBruno.CopilotCLIMonitor.Core.Interfaces;

namespace ElBruno.CopilotCLIMonitor.Tests.Fakes;

public sealed class FakeHookInstaller : IHookInstaller
{
    public HookInstallResult ResultToReturn { get; set; } =
        new HookInstallResult(true, InstalledFiles: [".copilotclimonitor/notify.ps1", ".copilotclimonitor/config.json", ".github/hooks/copilotclimon-notify.json"]);

    public string? LastInstalledRoot { get; private set; }
    public HookInstallOptions? LastOptions { get; private set; }

    public HookInstallResult Install(string repositoryRoot, HookInstallOptions? options = null)
    {
        LastInstalledRoot = repositoryRoot;
        LastOptions = options;
        return ResultToReturn;
    }
}
