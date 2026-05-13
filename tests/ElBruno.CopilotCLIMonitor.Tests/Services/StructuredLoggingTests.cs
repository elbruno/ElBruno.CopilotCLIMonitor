using ElBruno.CopilotCLIMonitor.Core.Models;
using ElBruno.CopilotCLIMonitor.Core.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace ElBruno.CopilotCLIMonitor.Tests.Services;

/// <summary>
/// Verifies that Core services emit structured log entries at key lifecycle moments.
/// Uses Moq to capture <see cref="ILogger{T}"/> calls without real sinks.
/// </summary>
public class StructuredLoggingTests
{
    // ── helpers ─────────────────────────────────────────────────────────────

    /// <summary>
    /// Returns a Moq logger and a helper that asserts a log entry with the
    /// expected level and message fragment was recorded.
    /// </summary>
    private static (Mock<ILogger<T>> mock, Action<LogLevel, string> verify) MakeLogger<T>()
    {
        var mock = new Mock<ILogger<T>>();
        mock.Setup(x => x.IsEnabled(It.IsAny<LogLevel>())).Returns(true);

        void verify(LogLevel level, string fragment) =>
            mock.Verify(
                x => x.Log(
                    level,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains(fragment, StringComparison.OrdinalIgnoreCase)),
                    It.IsAny<Exception?>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.AtLeastOnce,
                $"Expected a {level} log containing '{fragment}'.");

        return (mock, verify);
    }

    // ── EventStore ───────────────────────────────────────────────────────────

    [Fact]
    public void EventStore_Constructor_LogsInitialisation()
    {
        var (logger, verify) = MakeLogger<EventStore>();

        _ = new EventStore(50, logger.Object);

        verify(LogLevel.Information, "initialised");
    }

    [Fact]
    public void EventStore_Add_BeyondCapacity_LogsCapacityWarning()
    {
        var (logger, verify) = MakeLogger<EventStore>();
        var store = new EventStore(2, logger.Object);

        store.Add(new MonitorEvent(EventType.Warning, "a"));
        store.Add(new MonitorEvent(EventType.Warning, "b"));
        store.Add(new MonitorEvent(EventType.Warning, "c")); // triggers eviction

        verify(LogLevel.Warning, "capacity");
    }

    // ── MonitorEventParser ───────────────────────────────────────────────────

    [Fact]
    public void MonitorEventParser_Constructor_LogsInitialisation()
    {
        var (logger, verify) = MakeLogger<MonitorEventParser>();

        _ = new MonitorEventParser(logger.Object);

        verify(LogLevel.Information, "initialised");
    }

    [Fact]
    public void MonitorEventParser_Parse_KnownType_LogsInformation()
    {
        var (logger, verify) = MakeLogger<MonitorEventParser>();
        var parser = new MonitorEventParser(logger.Object);

        parser.Parse("task-completed", "All done", "my-repo", "main");

        verify(LogLevel.Information, "task-completed");
    }

    [Fact]
    public void MonitorEventParser_Parse_UnknownType_LogsWarning()
    {
        var (logger, verify) = MakeLogger<MonitorEventParser>();
        var parser = new MonitorEventParser(logger.Object);

        parser.Parse("some-unknown-event", "??");

        verify(LogLevel.Warning, "Unrecognised");
    }
}
