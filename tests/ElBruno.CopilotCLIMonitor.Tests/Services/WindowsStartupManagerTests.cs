using Microsoft.Win32;
using ElBruno.CopilotCLIMonitor.Services;

namespace ElBruno.CopilotCLIMonitor.Tests.Services;

public sealed class WindowsStartupManagerTests : IDisposable
{
    private readonly string _subKeyName = $@"Software\CopilotCliMon.Tests\Startup\{Guid.NewGuid():N}";
    private readonly string _entryName = $"CopilotCliMon.Test.{Guid.NewGuid():N}";
    private readonly string _exePath = @"C:\Tools\copilotclimon.exe";

    [Fact]
    public void SetEnabled_WhenTrue_WritesStartupEntry()
    {
        var sut = new WindowsStartupManager(_subKeyName, _entryName);

        sut.SetEnabled(enabled: true, executablePath: _exePath);

        using var key = Registry.CurrentUser.OpenSubKey(_subKeyName, writable: false);
        var value = key?.GetValue(_entryName) as string;
        Assert.Equal($"\"{_exePath}\"", value);
    }

    [Fact]
    public void SetEnabled_WhenFalse_RemovesStartupEntry()
    {
        var sut = new WindowsStartupManager(_subKeyName, _entryName);
        sut.SetEnabled(enabled: true, executablePath: _exePath);

        sut.SetEnabled(enabled: false, executablePath: _exePath);

        using var key = Registry.CurrentUser.OpenSubKey(_subKeyName, writable: false);
        Assert.Null(key?.GetValue(_entryName));
    }

    [Fact]
    public void IsEnabled_WhenEntryMatchesExecutable_ReturnsTrue()
    {
        var sut = new WindowsStartupManager(_subKeyName, _entryName);
        sut.SetEnabled(enabled: true, executablePath: _exePath);

        var enabled = sut.IsEnabled(_exePath);

        Assert.True(enabled);
    }

    public void Dispose()
    {
        try
        {
            Registry.CurrentUser.DeleteSubKeyTree(_subKeyName, throwOnMissingSubKey: false);
            Registry.CurrentUser.DeleteSubKeyTree(@"Software\CopilotCliMon.Tests\Startup", throwOnMissingSubKey: false);
            Registry.CurrentUser.DeleteSubKeyTree(@"Software\CopilotCliMon.Tests", throwOnMissingSubKey: false);
        }
        catch (ArgumentException)
        {
            // Key already removed by previous cleanup operation.
        }
    }
}
