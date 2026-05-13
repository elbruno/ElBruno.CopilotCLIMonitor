using Microsoft.Win32;

namespace ElBruno.CopilotCLIMonitor.Services;

public sealed class WindowsStartupManager(string? runKeyPath = null, string? entryName = null)
{
    private const string DefaultRunKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Run";
    private const string DefaultEntryName = "ElBruno.CopilotCLIMonitor";

    private readonly string _runKeyPath = string.IsNullOrWhiteSpace(runKeyPath) ? DefaultRunKeyPath : runKeyPath;
    private readonly string _entryName = string.IsNullOrWhiteSpace(entryName) ? DefaultEntryName : entryName;

    public void SetEnabled(bool enabled, string executablePath)
    {
        if (string.IsNullOrWhiteSpace(executablePath))
        {
            throw new ArgumentException("Executable path is required.", nameof(executablePath));
        }

        if (enabled)
        {
            using var key = OpenOrCreateRunKey(writable: true);
            key.SetValue(_entryName, BuildCommand(executablePath), RegistryValueKind.String);
            return;
        }

        using var readWriteKey = OpenRunKey(writable: true);
        if (readWriteKey?.GetValue(_entryName) is not null)
        {
            readWriteKey.DeleteValue(_entryName, throwOnMissingValue: false);
        }
    }

    public bool IsEnabled(string executablePath)
    {
        if (string.IsNullOrWhiteSpace(executablePath))
        {
            return false;
        }

        using var key = OpenRunKey(writable: false);
        var configured = key?.GetValue(_entryName) as string;
        return string.Equals(configured, BuildCommand(executablePath), StringComparison.OrdinalIgnoreCase);
    }

    internal static string BuildCommand(string executablePath) => $"\"{executablePath}\"";

    private RegistryKey OpenOrCreateRunKey(bool writable)
    {
        return writable
            ? Registry.CurrentUser.CreateSubKey(_runKeyPath, writable: true)
              ?? throw new InvalidOperationException($"Unable to open startup registry key: {_runKeyPath}")
            : Registry.CurrentUser.OpenSubKey(_runKeyPath, writable: false)
              ?? throw new InvalidOperationException($"Unable to open startup registry key: {_runKeyPath}");
    }

    private RegistryKey? OpenRunKey(bool writable) => Registry.CurrentUser.OpenSubKey(_runKeyPath, writable);
}
