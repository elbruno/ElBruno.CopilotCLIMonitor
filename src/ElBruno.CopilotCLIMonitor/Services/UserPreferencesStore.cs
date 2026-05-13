using System.IO;
using System.Text.Json;
using ElBruno.CopilotCLIMonitor.Models;

namespace ElBruno.CopilotCLIMonitor.Services;

public sealed class UserPreferencesStore
{
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };
    private readonly string _userPath;
    private readonly string _systemPath;

    public UserPreferencesStore(string? userPath = null, string? systemPath = null)
    {
        _userPath = userPath ?? GetDefaultUserPath();
        _systemPath = systemPath ?? GetDefaultSystemPath();
    }

    public UserPreferences Load()
    {
        var preferences = new UserPreferences();
        ApplyFromFile(preferences, _systemPath);
        ApplyFromFile(preferences, _userPath);
        return preferences;
    }

    public void Save(UserPreferences preferences)
    {
        var directory = Path.GetDirectoryName(_userPath);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var content = JsonSerializer.Serialize(preferences, JsonOptions);
        File.WriteAllText(_userPath, content);
    }

    private static void ApplyFromFile(UserPreferences target, string path)
    {
        if (!File.Exists(path))
        {
            return;
        }

        var content = File.ReadAllText(path);
        if (string.IsNullOrWhiteSpace(content))
        {
            return;
        }

        using var document = JsonDocument.Parse(content);
        var root = document.RootElement;
        if (root.ValueKind != JsonValueKind.Object)
        {
            return;
        }

        ApplyBoolean(root, nameof(UserPreferences.NotificationsEnabled), value => target.NotificationsEnabled = value);
        ApplyBoolean(root, nameof(UserPreferences.SoundEnabled), value => target.SoundEnabled = value);
        ApplyBoolean(root, nameof(UserPreferences.QuietHoursEnabled), value => target.QuietHoursEnabled = value);
        ApplyInt(root, nameof(UserPreferences.QuietHoursStart), value => target.QuietHoursStart = value);
        ApplyInt(root, nameof(UserPreferences.QuietHoursEnd), value => target.QuietHoursEnd = value);
        ApplyString(root, nameof(UserPreferences.LogLevel), value => target.LogLevel = value);
        ApplyBoolean(root, nameof(UserPreferences.StartWithWindows), value => target.StartWithWindows = value);
        ApplyBoolean(root, nameof(UserPreferences.TelemetryOptIn), value => target.TelemetryOptIn = value);
        ApplyNullableString(root, nameof(UserPreferences.TelemetryInstallationId), value => target.TelemetryInstallationId = value);
    }

    private static void ApplyBoolean(JsonElement root, string propertyName, Action<bool> apply)
    {
        if (root.TryGetProperty(propertyName, out var value) && (value.ValueKind == JsonValueKind.True || value.ValueKind == JsonValueKind.False))
        {
            apply(value.GetBoolean());
        }
    }

    private static void ApplyInt(JsonElement root, string propertyName, Action<int> apply)
    {
        if (root.TryGetProperty(propertyName, out var value) && value.ValueKind == JsonValueKind.Number && value.TryGetInt32(out var parsed))
        {
            apply(parsed);
        }
    }

    private static void ApplyString(JsonElement root, string propertyName, Action<string> apply)
    {
        if (root.TryGetProperty(propertyName, out var value) && value.ValueKind == JsonValueKind.String)
        {
            var parsed = value.GetString();
            if (!string.IsNullOrWhiteSpace(parsed))
            {
                apply(parsed);
            }
        }
    }

    private static void ApplyNullableString(JsonElement root, string propertyName, Action<string?> apply)
    {
        if (!root.TryGetProperty(propertyName, out var value))
        {
            return;
        }

        if (value.ValueKind == JsonValueKind.Null)
        {
            apply(null);
            return;
        }

        if (value.ValueKind == JsonValueKind.String)
        {
            apply(value.GetString());
        }
    }

    internal static string GetDefaultUserPath()
    {
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        return Path.Combine(appData, "CopilotCliMon", "preferences.json");
    }

    internal static string GetDefaultSystemPath()
    {
        var programData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
        return Path.Combine(programData, "CopilotCliMon", "preferences.system.json");
    }
}
