using System.CommandLine;

namespace ElBruno.CopilotCLIMonitor.Commands;

public static class DiagnosticCommand
{
    public static Command Build()
    {
        var enableOption = new Option<bool>("--enable")
        {
            Description = "Enable diagnostic mode (verbose logging)"
        };
        var disableOption = new Option<bool>("--disable")
        {
            Description = "Disable diagnostic mode"
        };

        var command = new Command("diagnostic", "Manage diagnostic troubleshooting mode")
        {
            enableOption,
            disableOption
        };

        command.SetAction(parseResult =>
        {
            var enable = parseResult.GetValue(enableOption);
            var disable = parseResult.GetValue(disableOption);

            if (enable && disable)
            {
                Console.Error.WriteLine("Choose either --enable or --disable.");
                return 1;
            }

            if (enable)
            {
                Environment.SetEnvironmentVariable("COPILOTCLIMON_DIAGNOSTIC_MODE", "true");
                Environment.SetEnvironmentVariable("COPILOTCLIMON_LOG_LEVEL", "Debug");
                Console.WriteLine("Diagnostic mode enabled.");
                return 0;
            }

            if (disable)
            {
                Environment.SetEnvironmentVariable("COPILOTCLIMON_DIAGNOSTIC_MODE", null);
                Console.WriteLine("Diagnostic mode disabled.");
                return 0;
            }

            var isEnabled = !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("COPILOTCLIMON_DIAGNOSTIC_MODE"));
            Console.WriteLine($"Diagnostic mode: {(isEnabled ? "enabled" : "disabled")}");
            return 0;
        });

        return command;
    }
}
