using ElBruno.CopilotCLIMonitor.Core.Models;

namespace ElBruno.CopilotCLIMonitor.Core.Interfaces;

/// <summary>
/// Represents the local HTTP server that receives IPC notifications from the CLI.
/// </summary>
public interface IIpcServer : IDisposable
{
    /// <summary>Raised on the thread pool each time a <see cref="MonitorEvent"/> is received.</summary>
    event Action<MonitorEvent>? EventReceived;

    /// <summary>Starts listening. Safe to call once per lifetime.</summary>
    void Start();

    /// <summary>Stops listening and releases the bound port.</summary>
    void Stop();
}
