using ElBruno.CopilotCLIMonitor.Services;

namespace ElBruno.CopilotCLIMonitor.Tests.Services;

public sealed class UiResourcesTests
{
    [Fact]
    public void Get_ReturnsDefinedResourceValue()
    {
        var value = UiResources.Get("MenuOpenDashboard");

        Assert.Equal("Open Dashboard", value);
    }

    [Fact]
    public void Get_WithMissingKey_ReturnsKeyName()
    {
        var value = UiResources.Get("MissingKey123");

        Assert.Equal("MissingKey123", value);
    }
}
