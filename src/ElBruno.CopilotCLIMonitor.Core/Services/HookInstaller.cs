using ElBruno.CopilotCLIMonitor.Core.Interfaces;

namespace ElBruno.CopilotCLIMonitor.Core.Services;

public class HookInstaller : IHookInstaller
{
    private const string HookDirectory = ".copilotclimonitor";
    private const string HookScriptName = "notify.ps1";

    public HookInstallResult Install(string repositoryRoot)
    {
        if (!Directory.Exists(repositoryRoot))
            return new HookInstallResult(false, $"Repository root does not exist: {repositoryRoot}");

        try
        {
            var hookDir = Path.Combine(repositoryRoot, HookDirectory);
            Directory.CreateDirectory(hookDir);

            var scriptPath = Path.Combine(hookDir, HookScriptName);
            var scriptContent = BuildNotifyScript();
            File.WriteAllText(scriptPath, scriptContent);

            var configPath = Path.Combine(hookDir, "config.json");
            if (!File.Exists(configPath))
            {
                var repoName = Path.GetFileName(repositoryRoot.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
                File.WriteAllText(configPath, BuildDefaultConfig(repoName));
            }

            return new HookInstallResult(true, InstalledFiles: [scriptPath, configPath]);
        }
        catch (Exception ex)
        {
            return new HookInstallResult(false, ex.Message);
        }
    }

    private static string BuildNotifyScript() =>
        """
        # CopilotCLIMonitor hook notification script
        # Usage: .\notify.ps1 -Event task-completed -Message "Your task is done"
        param(
            [string]$Event = "task-completed",
            [string]$Message = "Task completed"
        )
        copilotclimon notify --event $Event --message $Message
        """;

    private static string BuildDefaultConfig(string repoName) =>
        $$"""
        {
            "version": "1.0",
            "repository": "{{repoName}}",
            "enabled": true,
            "notificationsEnabled": true,
            "events": [
                "task-completed",
                "approval-required",
                "error",
                "warning",
                "build-completed",
                "test-completed",
                "workflow-completed"
            ],
            "quietHours": {
                "enabled": false,
                "start": "22:00",
                "end": "08:00"
            },
            "routing": {
                "sourceTagging": true
            }
        }
        """;
}
