using ElBruno.CopilotCLIMonitor.Core.Infrastructure;
using ElBruno.CopilotCLIMonitor.Core.Models;

namespace ElBruno.CopilotCLIMonitor.Tests.Infrastructure;

public class CoreSettingsTests
{
    [Fact]
    public void Default_HasExpectedIpcPort()
    {
        Assert.Equal(IpcConstants.DefaultPort, CoreSettings.Default.IpcPort);
    }

    [Fact]
    public void Default_HasPositiveEventStoreCapacity()
    {
        Assert.True(CoreSettings.Default.EventStoreCapacity > 0);
    }

    [Fact]
    public void Default_HasPositiveIpcTimeout()
    {
        Assert.True(CoreSettings.Default.IpcTimeoutSeconds > 0);
    }

    [Fact]
    public void Custom_IpcPort_CanBeOverridden()
    {
        var settings = new CoreSettings { IpcPort = 12345 };
        Assert.Equal(12345, settings.IpcPort);
    }
}
