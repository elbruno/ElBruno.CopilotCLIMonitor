using System.Net.Http.Json;
using ElBruno.CopilotCLIMonitor.Core.Interfaces;
using ElBruno.CopilotCLIMonitor.Core.Models;

namespace ElBruno.CopilotCLIMonitor.Core.Services;

public class HttpIpcClient : IIpcClient
{
    private readonly HttpClient _http;
    private readonly int _port;

    public HttpIpcClient(int port = IpcConstants.DefaultPort, TimeSpan? timeout = null)
    {
        _port = port;
        _http = new HttpClient { Timeout = timeout ?? TimeSpan.FromSeconds(5) };
    }

    public async Task<bool> SendNotifyAsync(NotifyRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _http.PostAsJsonAsync(IpcConstants.NotifyUrl(_port), request, cancellationToken);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> IsRunningAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _http.GetAsync(IpcConstants.HealthUrl(_port), cancellationToken);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}
