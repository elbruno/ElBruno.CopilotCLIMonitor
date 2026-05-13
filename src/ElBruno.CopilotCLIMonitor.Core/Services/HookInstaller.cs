using System.Text.Json;
using ElBruno.CopilotCLIMonitor.Core.Interfaces;

namespace ElBruno.CopilotCLIMonitor.Core.Services;

public class HookInstaller : IHookInstaller
{
    private const string HookDirectory = ".copilotclimonitor";
    private const string HookScriptName = "notify.ps1";
    private const string GithubHooksDirectory = ".github";
    private const string GithubHooksSubDirectory = "hooks";
    private const string GithubHookConfigName = "copilotclimon-notify.json";
    private static readonly string[] DefaultHookTriggers = ["agentStop", "errorOccurred"];
    private static readonly IReadOnlyDictionary<string, HookTemplate> HookTemplates =
        new Dictionary<string, HookTemplate>(StringComparer.OrdinalIgnoreCase)
        {
            ["agentStop"] = new("task-completed", "Copilot task completed"),
            ["errorOccurred"] = new("error", "Copilot command failed"),
            ["sessionEnd"] = new("task-completed", "Copilot session ended"),
            ["sessionStart"] = new("warning", "Copilot session started"),
            ["userPromptSubmitted"] = new("warning", "Copilot prompt submitted")
        };

    public HookInstallResult Install(string repositoryRoot, HookInstallOptions? options = null)
    {
        if (!Directory.Exists(repositoryRoot))
            return new HookInstallResult(false, $"Repository root does not exist: {repositoryRoot}");

        options ??= new HookInstallOptions();

        try
        {
            var hookDir = Path.Combine(repositoryRoot, HookDirectory);
            Directory.CreateDirectory(hookDir);

            var scriptPath = Path.Combine(hookDir, HookScriptName);
            var scriptContent = BuildNotifyScript();
            if (!File.Exists(scriptPath) || options.Force)
                File.WriteAllText(scriptPath, scriptContent);

            var configPath = Path.Combine(hookDir, "config.json");
            if (!File.Exists(configPath) || options.Force)
            {
                var repoName = Path.GetFileName(repositoryRoot.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
                File.WriteAllText(configPath, $$"""
                    {
                        "repository": "{{repoName}}",
                        "notificationsEnabled": true,
                        "events": ["task-completed", "approval-required", "error", "warning"]
                    }
                    """);
            }

            var githubHooksDir = Path.Combine(repositoryRoot, GithubHooksDirectory, GithubHooksSubDirectory);
            Directory.CreateDirectory(githubHooksDir);

            var githubHookConfigPath = Path.Combine(githubHooksDir, GithubHookConfigName);
            var selectedHooks = ResolveHookSelection(options.EnabledHookTriggers);
            if (!File.Exists(githubHookConfigPath) || options.Force)
                File.WriteAllText(githubHookConfigPath, BuildCopilotHookConfig(selectedHooks));

            return new HookInstallResult(true, InstalledFiles: [scriptPath, configPath, githubHookConfigPath]);
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

    private static IReadOnlyList<string> ResolveHookSelection(IReadOnlyList<string>? enabledHookTriggers)
    {
        if (enabledHookTriggers is null || enabledHookTriggers.Count == 0)
            return DefaultHookTriggers;

        var selected = enabledHookTriggers
            .Where(h => HookTemplates.ContainsKey(h))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        return selected.Length == 0 ? DefaultHookTriggers : selected;
    }

    private static string BuildCopilotHookConfig(IReadOnlyList<string> selectedHooks)
    {
        var hooks = new Dictionary<string, object[]>(StringComparer.OrdinalIgnoreCase);
        foreach (var hookName in selectedHooks)
        {
            var template = HookTemplates[hookName];
            hooks[hookName] =
            [
                new
                {
                    type = "command",
                    powershell = $".\\.copilotclimonitor\\notify.ps1 -Event {template.EventName} -Message \"{template.Message}\"",
                    cwd = ".",
                    timeoutSec = 10
                }
            ];
        }

        var payload = new { version = 1, hooks };
        return JsonSerializer.Serialize(payload, new JsonSerializerOptions { WriteIndented = true });
    }

    private sealed record HookTemplate(string EventName, string Message);
}
