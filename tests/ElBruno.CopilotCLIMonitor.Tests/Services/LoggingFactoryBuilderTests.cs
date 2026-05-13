using ElBruno.CopilotCLIMonitor.Core.Services;
using Microsoft.Extensions.Logging;

namespace ElBruno.CopilotCLIMonitor.Tests.Services;

public class LoggingFactoryBuilderTests : IDisposable
{
    private readonly string _tempLogDir = Path.Combine(Path.GetTempPath(), $"copilotclimon-log-{Guid.NewGuid():N}");

    [Fact]
    public void Create_DefaultLevel_IsInformation()
    {
        var previous = Environment.GetEnvironmentVariable("COPILOTCLIMON_LOG_LEVEL");
        var previousDiagnostic = Environment.GetEnvironmentVariable("COPILOTCLIMON_DIAGNOSTIC_MODE");
        var previousLogDir = Environment.GetEnvironmentVariable("COPILOTCLIMON_LOG_DIR");
        Environment.SetEnvironmentVariable("COPILOTCLIMON_LOG_LEVEL", null);
        Environment.SetEnvironmentVariable("COPILOTCLIMON_DIAGNOSTIC_MODE", null);
        Environment.SetEnvironmentVariable("COPILOTCLIMON_LOG_DIR", _tempLogDir);

        try
        {
            using var factory = LoggingFactoryBuilder.Create();
            var logger = factory.CreateLogger("test");

            Assert.True(logger.IsEnabled(LogLevel.Information));
            Assert.False(logger.IsEnabled(LogLevel.Debug));
        }
        finally
        {
            Environment.SetEnvironmentVariable("COPILOTCLIMON_LOG_LEVEL", previous);
            Environment.SetEnvironmentVariable("COPILOTCLIMON_DIAGNOSTIC_MODE", previousDiagnostic);
            Environment.SetEnvironmentVariable("COPILOTCLIMON_LOG_DIR", previousLogDir);
        }
    }

    [Fact]
    public void Create_WithDebugLevelEnv_EnablesDebugLogs()
    {
        var previous = Environment.GetEnvironmentVariable("COPILOTCLIMON_LOG_LEVEL");
        var previousDiagnostic = Environment.GetEnvironmentVariable("COPILOTCLIMON_DIAGNOSTIC_MODE");
        var previousLogDir = Environment.GetEnvironmentVariable("COPILOTCLIMON_LOG_DIR");
        Environment.SetEnvironmentVariable("COPILOTCLIMON_DIAGNOSTIC_MODE", null);
        Environment.SetEnvironmentVariable("COPILOTCLIMON_LOG_LEVEL", "Debug");
        Environment.SetEnvironmentVariable("COPILOTCLIMON_LOG_DIR", _tempLogDir);

        try
        {
            using var factory = LoggingFactoryBuilder.Create();
            var logger = factory.CreateLogger("test");
            Assert.True(logger.IsEnabled(LogLevel.Debug));
        }
        finally
        {
            Environment.SetEnvironmentVariable("COPILOTCLIMON_LOG_LEVEL", previous);
            Environment.SetEnvironmentVariable("COPILOTCLIMON_DIAGNOSTIC_MODE", previousDiagnostic);
            Environment.SetEnvironmentVariable("COPILOTCLIMON_LOG_DIR", previousLogDir);
        }
    }

    [Fact]
    public void Create_WhenDiagnosticModeEnabled_EnablesDebugLogs()
    {
        var previous = Environment.GetEnvironmentVariable("COPILOTCLIMON_LOG_LEVEL");
        var previousDiagnostic = Environment.GetEnvironmentVariable("COPILOTCLIMON_DIAGNOSTIC_MODE");
        var previousLogDir = Environment.GetEnvironmentVariable("COPILOTCLIMON_LOG_DIR");
        Environment.SetEnvironmentVariable("COPILOTCLIMON_LOG_LEVEL", "Information");
        Environment.SetEnvironmentVariable("COPILOTCLIMON_DIAGNOSTIC_MODE", "true");
        Environment.SetEnvironmentVariable("COPILOTCLIMON_LOG_DIR", _tempLogDir);

        try
        {
            using var factory = LoggingFactoryBuilder.Create();
            var logger = factory.CreateLogger("test");
            Assert.True(logger.IsEnabled(LogLevel.Debug));
        }
        finally
        {
            Environment.SetEnvironmentVariable("COPILOTCLIMON_LOG_LEVEL", previous);
            Environment.SetEnvironmentVariable("COPILOTCLIMON_DIAGNOSTIC_MODE", previousDiagnostic);
            Environment.SetEnvironmentVariable("COPILOTCLIMON_LOG_DIR", previousLogDir);
        }
    }

    [Fact]
    public void Create_WritesLogFile_WhenLogDirectoryConfigured()
    {
        var previousLogDir = Environment.GetEnvironmentVariable("COPILOTCLIMON_LOG_DIR");
        Environment.SetEnvironmentVariable("COPILOTCLIMON_LOG_DIR", _tempLogDir);

        try
        {
            using var factory = LoggingFactoryBuilder.Create();
            var logger = factory.CreateLogger("test");
            logger.LogInformation("file-log-test");
        }
        finally
        {
            Environment.SetEnvironmentVariable("COPILOTCLIMON_LOG_DIR", previousLogDir);
        }

        var files = Directory.GetFiles(_tempLogDir, "copilotclimon-*.log");
        Assert.NotEmpty(files);
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempLogDir))
        {
            Directory.Delete(_tempLogDir, recursive: true);
        }
    }
}
