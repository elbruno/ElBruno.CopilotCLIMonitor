using System.CommandLine;
using ElBruno.CopilotCLIMonitor.Cli.Services;

namespace ElBruno.CopilotCLIMonitor.Commands;

public static class UpdateCommand
{
    public static Command Build(UpdateChecker? checker = null)
    {
        var installOption = new Option<bool>("--install")
        {
            Description = "Install latest version if an update is available"
        };
        var currentVersionOption = new Option<string?>("--current-version")
        {
            Description = "Override current version (testing/advanced)"
        };
        var latestVersionOption = new Option<string?>("--latest-version")
        {
            Description = "Override latest version (testing/advanced)"
        };

        var command = new Command("update", "Check for updates and optionally install the latest version")
        {
            installOption,
            currentVersionOption,
            latestVersionOption
        };

        command.SetAction(async (parseResult, cancellationToken) =>
        {
            checker ??= new UpdateChecker();

            var install = parseResult.GetValue(installOption);
            var currentVersion = parseResult.GetValue(currentVersionOption)
                ?? typeof(Program).Assembly.GetName().Version?.ToString()
                ?? "0.0.0";
            var latestVersion = parseResult.GetValue(latestVersionOption)
                ?? await checker.GetLatestVersionAsync(cancellationToken)
                ?? currentVersion;

            Console.WriteLine($"Current version: {currentVersion}");
            Console.WriteLine($"Latest version:  {latestVersion}");

            if (!checker.IsUpdateAvailable(currentVersion, latestVersion))
            {
                Console.WriteLine("You are up to date.");
                return 0;
            }

            Console.WriteLine("Update available.");

            if (!install)
            {
                Console.WriteLine("Run: dotnet tool update -g ElBruno.CopilotCLIMonitor");
                return 0;
            }

            Console.WriteLine("Installing latest version...");
            var exitCode = checker.InstallLatest();
            if (exitCode == 0)
            {
                Console.WriteLine("Update installed successfully.");
            }
            else
            {
                Console.Error.WriteLine("Update installation failed.");
            }

            return exitCode;
        });

        return command;
    }
}
