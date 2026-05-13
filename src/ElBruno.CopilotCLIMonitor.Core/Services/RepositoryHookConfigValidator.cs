using System.Text.Json;

namespace ElBruno.CopilotCLIMonitor.Core.Services;

public static class RepositoryHookConfigValidator
{
    public static bool TryValidateFile(string configPath, out string? error)
    {
        error = null;
        if (!File.Exists(configPath))
        {
            error = $"config.json not found at: {configPath}";
            return false;
        }

        try
        {
            using var stream = File.OpenRead(configPath);
            using var document = JsonDocument.Parse(stream);
            return TryValidateJson(document.RootElement, out error);
        }
        catch (JsonException ex)
        {
            error = $"config.json is not valid JSON: {ex.Message}";
            return false;
        }
    }

    public static bool TryValidateJson(JsonElement root, out string? error)
    {
        error = null;

        if (root.ValueKind != JsonValueKind.Object)
        {
            error = "config.json root must be a JSON object.";
            return false;
        }

        if (!TryGetRequiredString(root, "version", out _, out error)) return false;
        if (!TryGetRequiredString(root, "repository", out _, out error)) return false;
        if (!TryGetRequiredBoolean(root, "enabled", out _, out error)) return false;
        if (!TryGetRequiredBoolean(root, "notificationsEnabled", out _, out error)) return false;

        if (!root.TryGetProperty("events", out var eventsElement) || eventsElement.ValueKind != JsonValueKind.Array)
        {
            error = "config.json field 'events' must be an array.";
            return false;
        }

        var hasEvent = false;
        foreach (var eventName in eventsElement.EnumerateArray())
        {
            if (eventName.ValueKind != JsonValueKind.String || string.IsNullOrWhiteSpace(eventName.GetString()))
            {
                error = "config.json field 'events' must contain only non-empty strings.";
                return false;
            }

            hasEvent = true;
        }

        if (!hasEvent)
        {
            error = "config.json field 'events' must contain at least one event.";
            return false;
        }

        if (!root.TryGetProperty("quietHours", out var quietHours) || quietHours.ValueKind != JsonValueKind.Object)
        {
            error = "config.json field 'quietHours' must be an object.";
            return false;
        }

        if (!TryGetRequiredBoolean(quietHours, "enabled", out _, out error)) return false;
        if (!TryGetRequiredTime(quietHours, "start", out error)) return false;
        if (!TryGetRequiredTime(quietHours, "end", out error)) return false;

        if (!root.TryGetProperty("routing", out var routing) || routing.ValueKind != JsonValueKind.Object)
        {
            error = "config.json field 'routing' must be an object.";
            return false;
        }

        if (!TryGetRequiredBoolean(routing, "sourceTagging", out _, out error)) return false;

        return true;
    }

    private static bool TryGetRequiredString(JsonElement root, string propertyName, out string value, out string? error)
    {
        value = string.Empty;
        error = null;

        if (!root.TryGetProperty(propertyName, out var element) || element.ValueKind != JsonValueKind.String)
        {
            error = $"config.json field '{propertyName}' must be a string.";
            return false;
        }

        var parsed = element.GetString();
        if (string.IsNullOrWhiteSpace(parsed))
        {
            error = $"config.json field '{propertyName}' must not be empty.";
            return false;
        }

        value = parsed;
        return true;
    }

    private static bool TryGetRequiredBoolean(JsonElement root, string propertyName, out bool value, out string? error)
    {
        value = false;
        error = null;

        if (!root.TryGetProperty(propertyName, out var element) || element.ValueKind != JsonValueKind.True && element.ValueKind != JsonValueKind.False)
        {
            error = $"config.json field '{propertyName}' must be a boolean.";
            return false;
        }

        value = element.GetBoolean();
        return true;
    }

    private static bool TryGetRequiredTime(JsonElement root, string propertyName, out string? error)
    {
        error = null;
        if (!TryGetRequiredString(root, propertyName, out var value, out error))
        {
            return false;
        }

        if (!TimeOnly.TryParseExact(value, "HH:mm", out _))
        {
            error = $"config.json field '{propertyName}' must use HH:mm format.";
            return false;
        }

        return true;
    }
}
