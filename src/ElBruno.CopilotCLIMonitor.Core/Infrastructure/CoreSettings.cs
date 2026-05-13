using ElBruno.CopilotCLIMonitor.Core.Models;

namespace ElBruno.CopilotCLIMonitor.Core.Infrastructure;

/// <summary>
/// Application-level settings for the CopilotCLI Monitor.
/// Bind this record from <c>appsettings.json</c> under the <c>"CopilotCLIMonitor"</c> section
/// or override individual properties at startup.
/// </summary>
public sealed record CoreSettings
{
    /// <summary>TCP port the systray app listens on for IPC notifications. Default: 54321.</summary>
    public int IpcPort { get; init; } = IpcConstants.DefaultPort;

    /// <summary>Maximum number of events retained in the in-memory store. Default: 200.</summary>
    public int EventStoreCapacity { get; init; } = 200;

    /// <summary>Timeout in seconds for outbound HTTP IPC calls from the CLI. Default: 5.</summary>
    public int IpcTimeoutSeconds { get; init; } = 5;

    /// <summary>Returns a <see cref="CoreSettings"/> instance populated entirely from defaults.</summary>
    public static CoreSettings Default => new();
}
