using System.CommandLine;
using ElBruno.CopilotCLIMonitor.Core.Interfaces;

namespace ElBruno.CopilotCLIMonitor.Commands;

public static class DoctorCommand
{
    public static Command Build(IRepositoryDetector detector)
    {
        var command = new Command("doctor", "Validate the copilotmon setup");

        command.SetAction((_, _) =>
        {
            Console.WriteLine("copilotmon doctor — validating setup");
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
                    Console.WriteLine(File.Exists(script) ? "✓ notify.ps1 present" : "✗ notify.ps1 missing — run: copilotmon init");
                }
                else
                {
                    Console.WriteLine("✗ Hook directory not found — run: copilotmon init");
                }
            }
            else
            {
                Console.WriteLine("⚠ Not inside a git repository (optional for notifications)");
            }

            Console.WriteLine();
            Console.WriteLine("Done. Start the monitor with: copilotmon");
            return Task.FromResult(0);
        });

        return command;
    }
}
