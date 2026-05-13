using System.CommandLine;
using ElBruno.CopilotCLIMonitor.Core.Interfaces;
using ElBruno.CopilotCLIMonitor.Core.Services;

namespace ElBruno.CopilotCLIMonitor.Commands;

public static class DoctorCommand
{
    public static Command Build(IRepositoryDetector detector)
    {
        var command = new Command("doctor", "Validate the copilotclimon setup");

        command.SetAction((_, _) =>
        {
            Console.WriteLine("copilotclimon doctor — validating setup");
            Console.WriteLine();

            var cwd = Directory.GetCurrentDirectory();
            var repoRoot = detector.DetectRepositoryRoot(cwd);

            if (repoRoot is not null)
            {
                Console.WriteLine($"✓ Git repository: {repoRoot}");
                var name = detector.GetRepositoryName(cwd);
                var branch = detector.GetCurrentBranch(cwd);
                if (name is not null) Console.WriteLine($"  Name: {name}");
                if (branch is not null) Console.WriteLine($"  Branch: {branch}");

                var hookDir = Path.Combine(repoRoot, ".copilotclimonitor");
                if (Directory.Exists(hookDir))
                {
                    Console.WriteLine("✓ Hook directory: .copilotclimonitor");
                    var script = Path.Combine(hookDir, "notify.ps1");
                    var config = Path.Combine(hookDir, "config.json");
                    var copilotHookFile = Path.Combine(repoRoot, ".github", "hooks", "copilotclimon-notify.json");
                    Console.WriteLine(File.Exists(script) ? "✓ notify.ps1 present" : "✗ notify.ps1 missing — run: copilotclimon init");
                    Console.WriteLine(File.Exists(config) ? "✓ config.json present" : "✗ config.json missing — run: copilotclimon init");
                    Console.WriteLine(File.Exists(copilotHookFile)
                        ? "✓ .github/hooks/copilotclimon-notify.json present"
                        : "✗ .github/hooks/copilotclimon-notify.json missing — run: copilotclimon init or copilotclimon upgrade");

                    if (File.Exists(copilotHookFile))
                    {
                        if (HookSelectionReader.TryReadSelectedTriggers(copilotHookFile, out var selectedHooks, out var readError))
                        {
                            var selectedHooksText = selectedHooks.Count == 0 ? "(none)" : string.Join(", ", selectedHooks);
                            Console.WriteLine($"✓ Hooks selected in init: {selectedHooksText}");
                        }
                        else
                        {
                            Console.WriteLine($"⚠ Could not read init hook selections: {readError}");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("✗ Hook directory not found — run: copilotclimon init");
                }
            }
            else
            {
                Console.WriteLine("⚠ Not inside a git repository (optional for notifications)");
            }

            Console.WriteLine();
            Console.WriteLine("Done. Start the monitor with: copilotclimon");
            return Task.FromResult(0);
        });

        return command;
    }
}
