using System.Net;
using System.Diagnostics;
using ElBruno.CopilotCLIMonitor.Core.Models;
using ElBruno.CopilotCLIMonitor.Core.Services;
using ElBruno.CopilotCLIMonitor.Services;

namespace ElBruno.CopilotCLIMonitor.Tests.Services;

public class IpcServerTests
{
    [Fact]
    public async Task SendNotifyAsync_WhenServerRunning_RaisesEventWithRequestData()
    {
        var port = ReserveFreePort();
        using var server = new IpcServer(port);
        server.Start();

        try
        {
            var received = new TaskCompletionSource<MonitorEvent>(TaskCreationOptions.RunContinuationsAsynchronously);
            server.EventReceived += evt => received.TrySetResult(evt);

            var client = new HttpIpcClient(port);
            var sent = await client.SendNotifyAsync(new NotifyRequest("task-completed", "Done", "repo", "main"));
            var monitorEvent = await received.Task.WaitAsync(TimeSpan.FromSeconds(5));

            Assert.True(sent);
            Assert.Equal(EventType.TaskCompleted, monitorEvent.EventType);
            Assert.Equal("Done", monitorEvent.Message);
            Assert.Equal("repo", monitorEvent.Repository);
            Assert.Equal("main", monitorEvent.Branch);
        }
        finally
        {
            server.Stop();
        }
    }

    [Fact]
    public async Task IsRunningAsync_WhenServerRunning_ReturnsTrue()
    {
        var port = ReserveFreePort();
        using var server = new IpcServer(port);
        server.Start();

        try
        {
            var client = new HttpIpcClient(port);
            var isRunning = await client.IsRunningAsync();
            Assert.True(isRunning);
        }
        finally
        {
            server.Stop();
        }
    }

    [Fact]
    public async Task SendNotifyAsync_WhenPayloadContainsControlChars_IsRejected()
    {
        var port = ReserveFreePort();
        using var server = new IpcServer(port);
        var eventRaised = false;
        server.EventReceived += _ => eventRaised = true;
        server.Start();

        try
        {
            var client = new HttpIpcClient(port);
            var sent = await client.SendNotifyAsync(new NotifyRequest("task-completed", "line1\nline2"));
            Assert.False(sent);
            Assert.False(eventRaised);
        }
        finally
        {
            server.Stop();
        }
    }

    [Fact]
    public async Task SendNotifyAsync_WhenAuthTokenRequiredAndMismatched_IsRejected()
    {
        var previousToken = Environment.GetEnvironmentVariable(IpcConstants.AuthTokenEnvVar);
        Environment.SetEnvironmentVariable(IpcConstants.AuthTokenEnvVar, "server-secret");
        try
        {
            var port = ReserveFreePort();
            using var server = new IpcServer(port);
            server.Start();

            try
            {
                var client = new HttpIpcClient(port, authToken: "wrong-secret");
                var sent = await client.SendNotifyAsync(new NotifyRequest("task-completed", "Done"));
                Assert.False(sent);
            }
            finally
            {
                server.Stop();
            }
        }
        finally
        {
            Environment.SetEnvironmentVariable(IpcConstants.AuthTokenEnvVar, previousToken);
        }
    }

    [Fact]
    public async Task SendNotifyAsync_WhenAuthTokenMatches_IsAccepted()
    {
        var previousToken = Environment.GetEnvironmentVariable(IpcConstants.AuthTokenEnvVar);
        Environment.SetEnvironmentVariable(IpcConstants.AuthTokenEnvVar, "shared-secret");
        try
        {
            var port = ReserveFreePort();
            using var server = new IpcServer(port);
            server.Start();

            try
            {
                var client = new HttpIpcClient(port, authToken: "shared-secret");
                var sent = await client.SendNotifyAsync(new NotifyRequest("task-completed", "Done"));
                Assert.True(sent);
            }
            finally
            {
                server.Stop();
            }
        }
        finally
        {
            Environment.SetEnvironmentVariable(IpcConstants.AuthTokenEnvVar, previousToken);
        }
    }

    [Fact]
    public async Task SendNotifyAsync_DoesNotBlockOnSlowEventHandlers()
    {
        var port = ReserveFreePort();
        using var server = new IpcServer(port);
        server.EventReceived += _ => Thread.Sleep(500);
        server.Start();

        try
        {
            var client = new HttpIpcClient(port);
            var sw = Stopwatch.StartNew();
            var sent = await client.SendNotifyAsync(new NotifyRequest("task-completed", "Done"));
            sw.Stop();

            Assert.True(sent);
            Assert.True(sw.ElapsedMilliseconds < 400, $"Expected fast response, got {sw.ElapsedMilliseconds}ms.");
        }
        finally
        {
            server.Stop();
        }
    }

    private static int ReserveFreePort()
    {
        var listener = new System.Net.Sockets.TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        var port = ((IPEndPoint)listener.LocalEndpoint).Port;
        listener.Stop();
        return port;
    }
}
