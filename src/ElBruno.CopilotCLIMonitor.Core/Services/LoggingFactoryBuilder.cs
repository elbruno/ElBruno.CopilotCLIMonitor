using Microsoft.Extensions.Logging;

namespace ElBruno.CopilotCLIMonitor.Core.Services;

public static class LoggingFactoryBuilder
{
    public static ILoggerFactory Create()
    {
        var minLevel = ResolveLogLevel();
        return LoggerFactory.Create(builder =>
        {
            builder.ClearProviders();
            builder.SetMinimumLevel(minLevel);
            builder.AddJsonConsole(options =>
            {
                options.IncludeScopes = true;
                options.TimestampFormat = "yyyy-MM-ddTHH:mm:ss.fffZ ";
            });
        });
    }

    private static LogLevel ResolveLogLevel()
    {
        if (IsDiagnosticModeEnabled())
        {
            return LogLevel.Debug;
        }

        var value = Environment.GetEnvironmentVariable("COPILOTCLIMON_LOG_LEVEL");
        if (!string.IsNullOrWhiteSpace(value) && Enum.TryParse<LogLevel>(value, ignoreCase: true, out var parsed))
        {
            return parsed;
        }

        return LogLevel.Information;
    }

    private static bool IsDiagnosticModeEnabled()
    {
        var value = Environment.GetEnvironmentVariable("COPILOTCLIMON_DIAGNOSTIC_MODE");
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        return value.Equals("1", StringComparison.OrdinalIgnoreCase)
            || value.Equals("true", StringComparison.OrdinalIgnoreCase)
            || value.Equals("yes", StringComparison.OrdinalIgnoreCase)
            || value.Equals("on", StringComparison.OrdinalIgnoreCase);
    }
}
