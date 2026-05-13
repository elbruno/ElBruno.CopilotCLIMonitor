using System.CommandLine;
using ElBruno.CopilotCLIMonitor.Core.Interfaces;
using Spectre.Console;

namespace ElBruno.CopilotCLIMonitor.Commands;

public static class InitCommand
{
    public static Command Build(IRepositoryDetector detector, IHookInstaller installer)
    {
        var pathOption = new Option<string?>("--path") { Description = "Repository root path (defaults to current directory)" };
        var defaultOption = new Option<bool>("--default")
        {
            Description = "Use default hook set without interactive selection"
        };
        var forceOption = new Option<bool>("--force")
        {
            Description = "Overwrite managed hook/config files with latest templates"
        };

        var command = new Command("init", "Initialize copilotclimon hooks in the current repository")
        {
            pathOption,
            defaultOption,
            forceOption
        };

        command.SetAction((parseResult, _) =>
        {
            var explicitPath = parseResult.GetValue(pathOption);
            var useDefault = parseResult.GetValue(defaultOption);
            var force = parseResult.GetValue(forceOption);
            var cwd = explicitPath ?? Directory.GetCurrentDirectory();

            var repoRoot = detector.DetectRepositoryRoot(cwd);
            if (repoRoot is null)
            {
                Console.Error.WriteLine("Error: Not inside a git repository.");
                return Task.FromResult(1);
            }

            Console.WriteLine($"Repository detected: {repoRoot}");
            var selectedHooks = useDefault ? null : PromptForHookSelection();
            var usingDefaultSelection = useDefault || selectedHooks is null;
            var result = installer.Install(
                repoRoot,
                new HookInstallOptions(EnabledHookTriggers: selectedHooks, Force: force, UseDefaultSelection: usingDefaultSelection));

            if (result.Success)
            {
                Console.WriteLine("✓ Hook integration installed.");
                if (result.InstalledFiles is { Count: > 0 } files)
                {
                    foreach (var f in files)
                        Console.WriteLine($"  {f}");
                }
                return Task.FromResult(0);
            }

            Console.Error.WriteLine($"✗ Hook installation failed: {result.ErrorMessage}");
            return Task.FromResult(1);
        });

        return command;
    }

    private static IReadOnlyList<string>? PromptForHookSelection()
    {
        if (Console.IsInputRedirected || Console.IsOutputRedirected)
            return null;

        var prompt = new MultiSelectionPrompt<string>()
            .Title("Select [green]Copilot hooks[/] to install")
            .NotRequired()
            .InstructionsText("[grey](Press <space> to toggle, <enter> to accept)[/]")
            .AddChoices("agentStop", "errorOccurred", "sessionEnd", "sessionStart", "userPromptSubmitted");

        prompt.Select("agentStop");
        prompt.Select("errorOccurred");

        List<string> selected = AnsiConsole.Prompt(prompt);
        return selected.Count == 0 ? null : selected;
    }
}
