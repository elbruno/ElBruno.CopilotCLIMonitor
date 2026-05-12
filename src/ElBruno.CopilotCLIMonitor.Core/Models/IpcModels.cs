namespace ElBruno.CopilotCLIMonitor.Core.Models;

/// <summary>Payload sent by the CLI to the systray app's HTTP endpoint.</summary>
public record NotifyRequest(
    string Event,
    string Message,
    string? Repository = null,
    string? Branch = null,
    string? Source = null);

/// <summary>Response from the systray app's HTTP endpoint.</summary>
public record NotifyResponse(bool Accepted, string? Error = null);

/// <summary>Shared IPC configuration (port, path, etc.).</summary>
public static class IpcConstants
{
    public const int DefaultPort = 54321;
    public const string NotifyPath = "/notify";
    public const string StatusPath = "/status";
    public const string HealthPath = "/health";

    public static string BaseUrl(int port = DefaultPort) => $"http://localhost:{port}";
    public static string NotifyUrl(int port = DefaultPort) => $"{BaseUrl(port)}{NotifyPath}";
    public static string StatusUrl(int port = DefaultPort) => $"{BaseUrl(port)}{StatusPath}";
    public static string HealthUrl(int port = DefaultPort) => $"{BaseUrl(port)}{HealthPath}";
}
