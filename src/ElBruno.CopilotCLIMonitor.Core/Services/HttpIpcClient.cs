using System.Net.Http.Json;
using System.Collections.Concurrent;
using ElBruno.CopilotCLIMonitor.Core.Interfaces;
using ElBruno.CopilotCLIMonitor.Core.Models;

namespace ElBruno.CopilotCLIMonitor.Core.Services;

public class HttpIpcClient : IIpcClient
{
    private static readonly ConcurrentDictionary<int, HttpClient> SharedClients = new();
    private readonly HttpClient _http;
    private readonly int _port;
    private readonly string? _authToken;
    private readonly bool _useAbsoluteUrls;

    public HttpIpcClient(int port = IpcConstants.DefaultPort, TimeSpan? timeout = null, string? authToken = null)
    {
        _port = port;
        _http = timeout.HasValue
            ? new HttpClient { Timeout = timeout.Value }
            : SharedClients.GetOrAdd(port, static p => new HttpClient { Timeout = TimeSpan.FromSeconds(5), BaseAddress = new Uri($"http://localhost:{p}/") });
        _useAbsoluteUrls = timeout.HasValue;
        _authToken = authToken ?? Environment.GetEnvironmentVariable(IpcConstants.AuthTokenEnvVar);
    }

    public async Task<bool> SendNotifyAsync(NotifyRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            using var httpRequest = new HttpRequestMessage(HttpMethod.Post, _useAbsoluteUrls ? IpcConstants.NotifyUrl(_port) : IpcConstants.NotifyPath)
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
            using var httpRequest = new HttpRequestMessage(HttpMethod.Get, _useAbsoluteUrls ? IpcConstants.HealthUrl(_port) : IpcConstants.HealthPath);
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
