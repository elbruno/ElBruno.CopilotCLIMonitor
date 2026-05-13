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
        var value = Environment.GetEnvironmentVariable("COPILOTCLIMON_LOG_LEVEL");
        if (!string.IsNullOrWhiteSpace(value) && Enum.TryParse<LogLevel>(value, ignoreCase: true, out var parsed))
        {
            return parsed;
        }

        return LogLevel.Information;
    }
}
