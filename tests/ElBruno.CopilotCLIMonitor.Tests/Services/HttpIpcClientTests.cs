using ElBruno.CopilotCLIMonitor.Core.Models;
using ElBruno.CopilotCLIMonitor.Core.Services;

namespace ElBruno.CopilotCLIMonitor.Tests.Services;

public class HttpIpcClientTests
{
    [Fact]
    public async Task IsRunningAsync_WhenNoServerListening_ReturnsFalse()
    {
        // Port unlikely to be in use in CI
        var client = new HttpIpcClient(port: 59821);
        var result = await client.IsRunningAsync();
        Assert.False(result);
    }

    [Fact]
    public async Task SendNotifyAsync_WhenNoServerListening_ReturnsFalse()
    {
        var client = new HttpIpcClient(port: 59822);
        var request = new NotifyRequest("task-completed", "Done");
        var result = await client.SendNotifyAsync(request);
        Assert.False(result);
    }
}
