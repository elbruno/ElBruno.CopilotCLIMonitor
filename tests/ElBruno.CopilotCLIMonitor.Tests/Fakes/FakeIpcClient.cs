using ElBruno.CopilotCLIMonitor.Core.Interfaces;
using ElBruno.CopilotCLIMonitor.Core.Models;

namespace ElBruno.CopilotCLIMonitor.Tests.Fakes;

public sealed class FakeIpcClient : IIpcClient
{
    public bool IsRunning { get; set; } = false;
    public List<NotifyRequest> SentRequests { get; } = [];

    public Task<bool> SendNotifyAsync(NotifyRequest request, CancellationToken cancellationToken = default)
    {
        SentRequests.Add(request);
        return Task.FromResult(IsRunning);
    }

    public Task<bool> IsRunningAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult(IsRunning);
}
