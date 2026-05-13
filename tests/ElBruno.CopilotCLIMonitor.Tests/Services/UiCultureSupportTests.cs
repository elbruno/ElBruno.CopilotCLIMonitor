using System.Globalization;
using ElBruno.CopilotCLIMonitor.Services;

namespace ElBruno.CopilotCLIMonitor.Tests.Services;

public sealed class UiCultureSupportTests
{
    [Theory]
    [InlineData("en-US", System.Windows.FlowDirection.LeftToRight)]
    [InlineData("ar-SA", System.Windows.FlowDirection.RightToLeft)]
    [InlineData("he-IL", System.Windows.FlowDirection.RightToLeft)]
    public void GetFlowDirection_ReturnsExpectedDirection(string cultureName, System.Windows.FlowDirection expected)
    {
        var culture = CultureInfo.GetCultureInfo(cultureName);

        var direction = UiCultureSupport.GetFlowDirection(culture);

        Assert.Equal(expected, direction);
    }
}
