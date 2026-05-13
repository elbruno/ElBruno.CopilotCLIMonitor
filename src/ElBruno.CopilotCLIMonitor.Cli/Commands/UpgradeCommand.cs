using System.CommandLine;
using ElBruno.CopilotCLIMonitor.Core.Interfaces;

namespace ElBruno.CopilotCLIMonitor.Commands;

public static class UpgradeCommand
{
    public static Command Build(IRepositoryDetector detector, IHookInstaller installer)
    {
        var pathOption = new Option<string?>("--path") { Description = "Repository root path (defaults to current directory)" };

        var command = new Command("upgrade", "Upgrade managed copilotclimon hook/config files in the current repository")
        {
            pathOption
        };

        command.SetAction((parseResult, _) =>
        {
            var explicitPath = parseResult.GetValue(pathOption);
            var cwd = explicitPath ?? Directory.GetCurrentDirectory();

            var repoRoot = detector.DetectRepositoryRoot(cwd);
            if (repoRoot is null)
            {
                Console.Error.WriteLine("Error: Not inside a git repository.");
                return Task.FromResult(1);
            }

            Console.WriteLine($"Repository detected: {repoRoot}");
            var result = installer.Install(
                repoRoot,
                new HookInstallOptions(Force: true, UseDefaultSelection: true));

            if (result.Success)
            {
                Console.WriteLine("✓ Hook integration upgraded.");
                if (result.InstalledFiles is { Count: > 0 } files)
                {
                    foreach (var f in files)
                        Console.WriteLine($"  {f}");
                }

                return Task.FromResult(0);
            }

            Console.Error.WriteLine($"✗ Hook upgrade failed: {result.ErrorMessage}");
            return Task.FromResult(1);
        });

        return command;
    }
}
