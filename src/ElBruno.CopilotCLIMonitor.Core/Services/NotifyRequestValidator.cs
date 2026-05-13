using System.Text.RegularExpressions;
using ElBruno.CopilotCLIMonitor.Core.Models;

namespace ElBruno.CopilotCLIMonitor.Core.Services;

public static partial class NotifyRequestValidator
{
    private const int MaxEventLength = 64;
    private const int MaxMessageLength = 2048;
    private const int MaxContextLength = 256;

    public static bool TryValidate(NotifyRequest request, out string? error)
    {
        if (!IsValidEventName(request.Event))
        {
            error = "Invalid event format. Use lowercase letters, numbers, and dashes only.";
            return false;
        }

        if (!IsValidText(request.Message, MaxMessageLength))
        {
            error = "Invalid message content.";
            return false;
        }

        if (!IsValidOptionalText(request.Repository, MaxContextLength))
        {
            error = "Invalid repository value.";
            return false;
        }

        if (!IsValidOptionalText(request.Branch, MaxContextLength))
        {
            error = "Invalid branch value.";
            return false;
        }

        if (!IsValidOptionalText(request.Source, MaxContextLength))
        {
            error = "Invalid source value.";
            return false;
        }

        error = null;
        return true;
    }

    private static bool IsValidEventName(string value) =>
        !string.IsNullOrWhiteSpace(value) &&
        value.Length <= MaxEventLength &&
        EventNameRegex().IsMatch(value);

    private static bool IsValidOptionalText(string? value, int maxLength) =>
        string.IsNullOrEmpty(value) || IsValidText(value, maxLength);

    private static bool IsValidText(string value, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Length > maxLength)
        {
            return false;
        }

        foreach (var ch in value)
        {
            if (char.IsControl(ch))
            {
                return false;
            }
        }

        return true;
    }

    [GeneratedRegex("^[a-z0-9-]+$")]
    private static partial Regex EventNameRegex();
}
