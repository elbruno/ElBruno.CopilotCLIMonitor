using System.Net.Http.Json;
using ElBruno.CopilotCLIMonitor.Core.Interfaces;
using ElBruno.CopilotCLIMonitor.Core.Models;

namespace ElBruno.CopilotCLIMonitor.Core.Services;

public class HttpIpcClient : IIpcClient
{
    private readonly HttpClient _http;
    private readonly int _port;
    private readonly string? _authToken;

    public HttpIpcClient(int port = IpcConstants.DefaultPort, TimeSpan? timeout = null, string? authToken = null)
    {
        _port = port;
        _http = new HttpClient { Timeout = timeout ?? TimeSpan.FromSeconds(5) };
        _authToken = authToken ?? Environment.GetEnvironmentVariable(IpcConstants.AuthTokenEnvVar);
    }

    public async Task<bool> SendNotifyAsync(NotifyRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            using var httpRequest = new HttpRequestMessage(HttpMethod.Post, IpcConstants.NotifyUrl(_port))
            {
                Content = JsonContent.Create(request)
            };
            AddAuthHeader(httpRequest);
            var response = await _http.SendAsync(httpRequest, cancellationToken);
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
            using var httpRequest = new HttpRequestMessage(HttpMethod.Get, IpcConstants.HealthUrl(_port));
            AddAuthHeader(httpRequest);
            var response = await _http.SendAsync(httpRequest, cancellationToken);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    private void AddAuthHeader(HttpRequestMessage request)
    {
        if (!string.IsNullOrWhiteSpace(_authToken))
        {
            request.Headers.TryAddWithoutValidation(IpcConstants.AuthHeaderName, _authToken);
        }
    }
}
