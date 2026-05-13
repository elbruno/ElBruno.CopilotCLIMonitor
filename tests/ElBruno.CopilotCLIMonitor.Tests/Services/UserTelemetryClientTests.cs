using System.Text.Json;
using ElBruno.CopilotCLIMonitor.Services;

namespace ElBruno.CopilotCLIMonitor.Tests.Services;

public sealed class UserTelemetryClientTests : IDisposable
{
    private readonly string _tempFile = Path.Combine(Path.GetTempPath(), $"copilotclimon-telemetry-{Guid.NewGuid():N}.log");

    [Fact]
    public void TrackEvent_WritesTelemetryLineWithHashedRepository()
    {
        var client = new UserTelemetryClient("installation123", _tempFile);

        client.TrackEvent("event_received", "TaskCompleted", "elbruno/repo");

        var lines = File.ReadAllLines(_tempFile);
        Assert.Single(lines);

        using var doc = JsonDocument.Parse(lines[0]);
        var root = doc.RootElement;
        Assert.Equal("event_received", root.GetProperty("eventName").GetString());
        Assert.Equal("TaskCompleted", root.GetProperty("eventType").GetString());
        Assert.Equal("installation123", root.GetProperty("installationId").GetString());
        Assert.NotNull(root.GetProperty("repositoryHash").GetString());
        Assert.NotEqual("elbruno/repo", root.GetProperty("repositoryHash").GetString());
    }

    [Fact]
    public void HashValue_ReturnsNullForBlankInput()
    {
        Assert.Null(UserTelemetryClient.HashValue(null));
        Assert.Null(UserTelemetryClient.HashValue(string.Empty));
    }

    public void Dispose()
    {
        if (File.Exists(_tempFile))
        {
            File.Delete(_tempFile);
        }
    }
}
