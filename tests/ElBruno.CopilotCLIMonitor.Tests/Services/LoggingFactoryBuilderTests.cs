using ElBruno.CopilotCLIMonitor.Core.Services;
using Microsoft.Extensions.Logging;

namespace ElBruno.CopilotCLIMonitor.Tests.Services;

public class LoggingFactoryBuilderTests
{
    [Fact]
    public void Create_DefaultLevel_IsInformation()
    {
        var previous = Environment.GetEnvironmentVariable("COPILOTCLIMON_LOG_LEVEL");
        Environment.SetEnvironmentVariable("COPILOTCLIMON_LOG_LEVEL", null);

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
        }
    }

    [Fact]
    public void Create_WithDebugLevelEnv_EnablesDebugLogs()
    {
        var previous = Environment.GetEnvironmentVariable("COPILOTCLIMON_LOG_LEVEL");
        Environment.SetEnvironmentVariable("COPILOTCLIMON_LOG_LEVEL", "Debug");

        try
        {
            using var factory = LoggingFactoryBuilder.Create();
            var logger = factory.CreateLogger("test");
            Assert.True(logger.IsEnabled(LogLevel.Debug));
        }
        finally
        {
            Environment.SetEnvironmentVariable("COPILOTCLIMON_LOG_LEVEL", previous);
        }
    }
}
