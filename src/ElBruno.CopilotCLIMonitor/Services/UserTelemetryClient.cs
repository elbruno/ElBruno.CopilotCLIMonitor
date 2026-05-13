using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace ElBruno.CopilotCLIMonitor.Services;

public sealed class UserTelemetryClient
{
    private readonly string _path;
    private readonly string _installationId;

    public UserTelemetryClient(string installationId, string? path = null)
    {
        _installationId = installationId;
        _path = path ?? GetDefaultPath();
    }

    public void TrackEvent(string eventName, string? eventType = null, string? repository = null)
    {
        var payload = new
        {
            timestampUtc = DateTime.UtcNow,
            eventName,
            eventType,
            installationId = _installationId,
            repositoryHash = HashValue(repository)
        };

        var directory = Path.GetDirectoryName(_path);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var line = JsonSerializer.Serialize(payload);
        File.AppendAllText(_path, line + Environment.NewLine);
    }

    public static string? HashValue(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return null;
        }

        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));
        return Convert.ToHexString(bytes);
    }

    internal static string GetDefaultPath()
    {
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        return Path.Combine(appData, "CopilotCliMon", "telemetry.log");
    }
}
