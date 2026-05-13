using ElBruno.CopilotCLIMonitor.Cli.Services;

namespace ElBruno.CopilotCLIMonitor.Tests.Services;

public class UpdateCheckerTests
{
    private readonly UpdateChecker _sut = new();

    [Theory]
    [InlineData("1.0.0", "1.0.1", true)]
    [InlineData("1.2.0", "1.2.0", false)]
    [InlineData("1.2.1", "1.2.0", false)]
    [InlineData("1.2.0-beta.1", "1.2.0", false)]
    public void IsUpdateAvailable_ComparesVersions(string current, string latest, bool expected)
    {
        var actual = _sut.IsUpdateAvailable(current, latest);
        Assert.Equal(expected, actual);
    }
}
