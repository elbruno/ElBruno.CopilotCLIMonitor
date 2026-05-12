using System.Net;
using System.Text;
using System.Text.Json;
using ElBruno.CopilotCLIMonitor.Core.Models;

namespace ElBruno.CopilotCLIMonitor.Services;

/// <summary>
/// Lightweight HTTP server that listens for events from the copilotmon CLI.
/// Binds to localhost only — no elevated privileges required.
/// </summary>
public sealed class IpcServer : IDisposable
{
    private readonly HttpListener _listener;
    private readonly int _port;
    private CancellationTokenSource? _cts;
    private Task? _listenTask;

    public event Action<MonitorEvent>? EventReceived;

    public IpcServer(int port = IpcConstants.DefaultPort)
    {
        _port = port;
        _listener = new HttpListener();
        _listener.Prefixes.Add($"http://localhost:{port}/");
    }

    public void Start()
    {
        _listener.Start();
        _cts = new CancellationTokenSource();
        _listenTask = Task.Run(() => AcceptLoopAsync(_cts.Token));
    }

    public void Stop()
    {
        _cts?.Cancel();
        try { _listener.Stop(); } catch { /* ignore */ }
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
            catch (OperationCanceledException) { break; }
            catch { break; }

            _ = Task.Run(() => HandleRequestAsync(ctx), ct);
        }
    }

    private async Task HandleRequestAsync(HttpListenerContext ctx)
    {
        var req = ctx.Request;
        var resp = ctx.Response;

        try
        {
            if (req.HttpMethod == "GET" && req.Url?.AbsolutePath is IpcConstants.HealthPath or IpcConstants.StatusPath or "/open")
            {
                await WriteJsonAsync(resp, new { status = "running", port = _port });

                if (req.Url?.AbsolutePath == "/open")
                    EventReceived?.Invoke(MonitorEvent.Parse("workflow-completed", "Dashboard opened via CLI"));

                return;
            }

            if (req.HttpMethod == "POST" && req.Url?.AbsolutePath == IpcConstants.NotifyPath)
            {
                using var reader = new System.IO.StreamReader(req.InputStream, Encoding.UTF8);
                var body = await reader.ReadToEndAsync();
                var notifyReq = JsonSerializer.Deserialize<NotifyRequest>(body, _jsonOpts);

                if (notifyReq is null)
                {
                    resp.StatusCode = 400;
                    await WriteJsonAsync(resp, new NotifyResponse(false, "Invalid request body"));
                    return;
                }

                var monitorEvent = MonitorEvent.Parse(
                    notifyReq.Event,
                    notifyReq.Message,
                    notifyReq.Repository,
                    notifyReq.Branch);

                EventReceived?.Invoke(monitorEvent);
                await WriteJsonAsync(resp, new NotifyResponse(true));
                return;
            }

            resp.StatusCode = 404;
            await WriteJsonAsync(resp, new { error = "not found" });
        }
        catch (Exception ex)
        {
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
        var json = JsonSerializer.Serialize(payload, _jsonOpts);
        var bytes = Encoding.UTF8.GetBytes(json);
        resp.ContentType = "application/json";
        resp.ContentLength64 = bytes.Length;
        resp.StatusCode = resp.StatusCode == 0 ? 200 : resp.StatusCode;
        await resp.OutputStream.WriteAsync(bytes);
    }

    private static readonly JsonSerializerOptions _jsonOpts = new(JsonSerializerDefaults.Web);

    public void Dispose()
    {
        Stop();
        _listener.Close();
    }
}
