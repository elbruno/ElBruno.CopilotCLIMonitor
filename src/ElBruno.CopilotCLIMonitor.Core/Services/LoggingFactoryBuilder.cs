using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;

namespace ElBruno.CopilotCLIMonitor.Core.Services;

public static class LoggingFactoryBuilder
{
    public static ILoggerFactory Create()
    {
        var minLevel = ResolveLogLevel();
        var logDirectory = ResolveLogDirectory();
        Directory.CreateDirectory(logDirectory);
        var logFilePath = Path.Combine(logDirectory, "copilotclimon-.log");

        var serilogLogger = new LoggerConfiguration()
            .MinimumLevel.Is(ToSerilogLevel(minLevel))
            .Enrich.FromLogContext()
            .WriteTo.Console(new RenderedCompactJsonFormatter())
            .WriteTo.File(
                formatter: new RenderedCompactJsonFormatter(),
                path: logFilePath,
                rollingInterval: RollingInterval.Day,
                rollOnFileSizeLimit: true,
                fileSizeLimitBytes: 10 * 1024 * 1024,
                retainedFileCountLimit: 14,
                shared: true)
            .CreateLogger();

        return LoggerFactory.Create(builder =>
        {
            builder.ClearProviders();
            builder.SetMinimumLevel(minLevel);
            builder.AddSerilog(serilogLogger, dispose: true);
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

    private static string ResolveLogDirectory()
    {
        var fromEnv = Environment.GetEnvironmentVariable("COPILOTCLIMON_LOG_DIR");
        if (!string.IsNullOrWhiteSpace(fromEnv))
        {
            return fromEnv;
        }

        var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        return Path.Combine(appData, "CopilotCliMon", "logs");
    }

    private static LogEventLevel ToSerilogLevel(LogLevel level) => level switch
    {
        LogLevel.Trace => LogEventLevel.Verbose,
        LogLevel.Debug => LogEventLevel.Debug,
        LogLevel.Information => LogEventLevel.Information,
        LogLevel.Warning => LogEventLevel.Warning,
        LogLevel.Error => LogEventLevel.Error,
        LogLevel.Critical => LogEventLevel.Fatal,
        _ => LogEventLevel.Information
    };
}
