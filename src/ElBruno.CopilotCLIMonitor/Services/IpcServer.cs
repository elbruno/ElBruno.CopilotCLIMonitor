using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using ElBruno.CopilotCLIMonitor.Core.Interfaces;
using ElBruno.CopilotCLIMonitor.Core.Models;
using ElBruno.CopilotCLIMonitor.Core.Services;
using Microsoft.Extensions.Logging;

namespace ElBruno.CopilotCLIMonitor.Services;

/// <summary>
/// Lightweight HTTP server that listens for events from the copilotclimon CLI.
/// Binds to localhost only — no elevated privileges required.
/// </summary>
public sealed class IpcServer : IIpcServer
{
    private readonly HttpListener _listener;
    private readonly int _port;
    private readonly ILogger<IpcServer> _logger;
    private CancellationTokenSource? _cts;
    private Task? _listenTask;

    public event Action<MonitorEvent>? EventReceived;

    public IpcServer(int port = IpcConstants.DefaultPort, ILogger<IpcServer>? logger = null)
    {
        _port = port;
        _logger = logger ?? Microsoft.Extensions.Logging.Abstractions.NullLogger<IpcServer>.Instance;
        _listener = new HttpListener();
        _listener.Prefixes.Add($"http://localhost:{port}/");
        _logger.LogInformation("IpcServer initialised on port {Port}.", port);
    }

    public void Start()
    {
        _listener.Start();
        _cts = new CancellationTokenSource();
        _listenTask = Task.Run(() => AcceptLoopAsync(_cts.Token));
        _logger.LogInformation("IpcServer started, listening on port {Port}.", _port);
    }

    public void Stop()
    {
        _cts?.Cancel();
        try { _listener.Stop(); } catch { /* ignore */ }
        _logger.LogInformation("IpcServer stopped.");
    }

    private async Task AcceptLoopAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            HttpListenerContext ctx;
            try
            {
                ctx = await _listener.GetContextAsync().WaitAsync(ct);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("IPC connection accept loop cancelled.");
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "IPC accept loop encountered an unexpected error — stopping.");
                break;
            }

            _ = Task.Run(() => HandleRequestAsync(ctx), ct);
        }
    }

    private async Task HandleRequestAsync(HttpListenerContext ctx)
    {
        var req = ctx.Request;
        var resp = ctx.Response;

        try
        {
            if (!IsAuthorizedRequest(req))
            {
                resp.StatusCode = 401;
                await WriteJsonAsync(resp, new { error = "unauthorized" });
                return;
            }

            if (req.HttpMethod == "GET" && req.Url?.AbsolutePath is IpcConstants.HealthPath or IpcConstants.StatusPath or "/open")
            {
                await WriteJsonAsync(resp, new { status = "running", port = _port });

                if (req.Url?.AbsolutePath == "/open")
                {
                    _logger.LogInformation("Dashboard open requested via CLI.");
                    EventReceived?.Invoke(MonitorEvent.Parse("workflow-completed", "Dashboard opened via CLI"));
                }

                return;
            }

            if (req.HttpMethod == "POST" && req.Url?.AbsolutePath == IpcConstants.NotifyPath)
            {
                var notifyReq = await JsonSerializer.DeserializeAsync<NotifyRequest>(req.InputStream, _jsonOpts);

                if (notifyReq is null)
                {
                    _logger.LogWarning("Received invalid (null) notify request body.");
                    resp.StatusCode = 400;
                    await WriteJsonAsync(resp, new NotifyResponse(false, "Invalid request body"));
                    return;
                }

                if (!NotifyRequestValidator.TryValidate(notifyReq, out var validationError))
                {
                    _logger.LogWarning("Rejected invalid notify request: {Reason}", validationError);
                    resp.StatusCode = 400;
                    await WriteJsonAsync(resp, new NotifyResponse(false, validationError));
                    return;
                }

                var monitorEvent = MonitorEvent.Parse(
                    notifyReq.Event,
                    notifyReq.Message,
                    notifyReq.Repository,
                    notifyReq.Branch,
                    notifyReq.Source,
                    notifyReq.OriginRepository);

                _logger.LogInformation(
                    "Event received via IPC: type={EventType} repo={Repository} branch={Branch} source={Source} origin={Origin}",
                    monitorEvent.EventType,
                    monitorEvent.Repository ?? "(none)",
                    monitorEvent.Branch ?? "(none)",
                    monitorEvent.Source ?? "(none)",
                    monitorEvent.OriginRepository ?? "(none)");

                DispatchEventAsync(monitorEvent);
                await WriteJsonAsync(resp, new NotifyResponse(true));
                return;
            }

            resp.StatusCode = 404;
            await WriteJsonAsync(resp, new { error = "not found" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled error while processing IPC request {Method} {Path}.",
                req.HttpMethod, req.Url?.AbsolutePath);
            try
            {
                resp.StatusCode = 500;
                await WriteJsonAsync(resp, new { error = ex.Message });
            }
            catch { /* response already sent */ }
        }
        finally
        {
            resp.Close();
        }
    }

    private static async Task WriteJsonAsync(HttpListenerResponse resp, object payload)
    {
        var bytes = JsonSerializer.SerializeToUtf8Bytes(payload, _jsonOpts);
        resp.ContentType = "application/json";
        resp.ContentLength64 = bytes.Length;
        resp.StatusCode = resp.StatusCode == 0 ? 200 : resp.StatusCode;
        await resp.OutputStream.WriteAsync(bytes);
    }

    private static bool IsAuthorizedRequest(HttpListenerRequest req)
    {
        var expectedToken = Environment.GetEnvironmentVariable(IpcConstants.AuthTokenEnvVar);
        if (string.IsNullOrWhiteSpace(expectedToken))
        {
            return true;
        }

        var providedToken = req.Headers[IpcConstants.AuthHeaderName];
        if (string.IsNullOrEmpty(providedToken))
        {
            return false;
        }

        var expectedBytes = Encoding.UTF8.GetBytes(expectedToken);
        var providedBytes = Encoding.UTF8.GetBytes(providedToken);
        return CryptographicOperations.FixedTimeEquals(expectedBytes, providedBytes);
    }

    private static readonly JsonSerializerOptions _jsonOpts = new(JsonSerializerDefaults.Web);

    private void DispatchEventAsync(MonitorEvent monitorEvent)
    {
        var handlers = EventReceived;
        if (handlers is null)
        {
            return;
        }

        _ = Task.Run(() =>
        {
            try
            {
                handlers.Invoke(monitorEvent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Event handler failed while processing IPC event.");
            }
        });
    }

    public void Dispose()
    {
        Stop();
        _listener.Close();
    }
}
