using System.IO;
using System.Text.Json;
using ElBruno.CopilotCLIMonitor.Models;

namespace ElBruno.CopilotCLIMonitor.Services;

public sealed class UserPreferencesStore
{
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };
    private readonly string _path;

    public UserPreferencesStore(string? path = null)
    {
        _path = path ?? GetDefaultPath();
    }

    public UserPreferences Load()
    {
        if (!File.Exists(_path))
        {
            return new UserPreferences();
        }

        var content = File.ReadAllText(_path);
        if (string.IsNullOrWhiteSpace(content))
        {
            return new UserPreferences();
        }

        return JsonSerializer.Deserialize<UserPreferences>(content) ?? new UserPreferences();
    }

    public void Save(UserPreferences preferences)
    {
        var directory = Path.GetDirectoryName(_path);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var content = JsonSerializer.Serialize(preferences, JsonOptions);
        File.WriteAllText(_path, content);
    }

    internal static string GetDefaultPath()
    {
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        return Path.Combine(appData, "CopilotCliMon", "preferences.json");
    }
}
