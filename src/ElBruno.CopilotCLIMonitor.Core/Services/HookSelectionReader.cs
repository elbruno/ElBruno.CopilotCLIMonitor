using System.Text.Json;

namespace ElBruno.CopilotCLIMonitor.Core.Services;

public static class HookSelectionReader
{
    public static bool TryReadSelectedTriggers(string hookConfigPath, out IReadOnlyList<string> selectedTriggers, out string? error)
    {
        selectedTriggers = [];
        error = null;

        try
        {
            using var stream = File.OpenRead(hookConfigPath);
            using var document = JsonDocument.Parse(stream);

            if (!document.RootElement.TryGetProperty("hooks", out var hooksElement) || hooksElement.ValueKind != JsonValueKind.Object)
            {
                error = "missing 'hooks' object";
                return false;
            }

            selectedTriggers = hooksElement
                .EnumerateObject()
                .Select(property => property.Name)
                .Where(name => !string.IsNullOrWhiteSpace(name))
                .OrderBy(name => name, StringComparer.OrdinalIgnoreCase)
                .ToArray();

            return true;
        }
        catch (Exception ex)
        {
            error = ex.Message;
            return false;
        }
    }
}
