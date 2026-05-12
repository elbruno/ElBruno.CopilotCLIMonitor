using System.CommandLine;
using ElBruno.CopilotCLIMonitor.Core.Interfaces;

namespace ElBruno.CopilotCLIMonitor.Commands;

public static class InitCommand
{
    public static Command Build(IRepositoryDetector detector, IHookInstaller installer)
    {
        var pathOption = new Option<string?>("--path", "Repository root path (defaults to current directory)");

        var command = new Command("init", "Initialize copilotmon hooks in the current repository")
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
            var result = installer.Install(repoRoot);

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
}
