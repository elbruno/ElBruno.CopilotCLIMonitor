using System.Net;
using System.Text;
using System.Text.Json;
using ElBruno.CopilotCLIMonitor.Cli.Handlers;
using ElBruno.CopilotCLIMonitor.Core.Models;
using ElBruno.CopilotCLIMonitor.Core.Services;
using ElBruno.CopilotCLIMonitor.Tests.Fakes;

namespace ElBruno.CopilotCLIMonitor.Tests.Commands;

public sealed class CliSystrayIntegrationTests : IDisposable
{
    private readonly string _tempRepoDir;

    public CliSystrayIntegrationTests()
    {
        _tempRepoDir = Path.Combine(Path.GetTempPath(), $"copilotclimon-e2e-{Guid.NewGuid():N}");
        Directory.CreateDirectory(_tempRepoDir);
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempRepoDir))
        {
            Directory.Delete(_tempRepoDir, recursive: true);
        }
    }

    [Fact]
    public async Task Notify_WithRunningIpcServer_PostsEventPayloadToSystrayEndpoint()
    {
        var port = ReserveFreePort();
        using var listener = new HttpListener();
        listener.Prefixes.Add($"http://localhost:{port}/");
        listener.Start();

        var receivedRequestTask = Task.Run(async () =>
        {
            var ctx = await listener.GetContextAsync();
            var path = ctx.Request.Url?.AbsolutePath;
            if (ctx.Request.HttpMethod == "POST" && path == IpcConstants.NotifyPath)
            {
                using var reader = new StreamReader(ctx.Request.InputStream, Encoding.UTF8);
                var body = await reader.ReadToEndAsync();
                ctx.Response.StatusCode = 200;
                var responseBytes = Encoding.UTF8.GetBytes("""{"accepted":true}""");
                await ctx.Response.OutputStream.WriteAsync(responseBytes);
                ctx.Response.Close();
                return body;
            }

            ctx.Response.StatusCode = 404;
            ctx.Response.Close();
            return string.Empty;
        });

        var detector = new FakeRepositoryDetector { NameToReturn = "demo-repo", BranchToReturn = "main" };
        var handlers = new CliCommandHandlers(
            detector,
            new FakeHookInstaller(),
            new HttpIpcClient(port),
            TextWriter.Null,
            TextWriter.Null);

        var exitCode = await handlers.RunNotifyAsync(["--event", "task-completed", "--message", "Done"]);
        var bodyJson = await receivedRequestTask.WaitAsync(TimeSpan.FromSeconds(5));
        var payload = JsonSerializer.Deserialize<NotifyRequest>(bodyJson, new JsonSerializerOptions(JsonSerializerDefaults.Web));

        Assert.Equal(0, exitCode);
        Assert.NotNull(payload);
        Assert.Equal("task-completed", payload.Event);
        Assert.Equal("Done", payload.Message);
        Assert.Equal("demo-repo", payload.Repository);
        Assert.Equal("main", payload.Branch);
    }

    [Fact]
    public async Task Doctor_WithRunningIpcServerAndInstalledHooks_ReturnsSuccess()
    {
        var port = ReserveFreePort();
        using var listener = new HttpListener();
        listener.Prefixes.Add($"http://localhost:{port}/");
        listener.Start();

        var healthTask = Task.Run(async () =>
        {
            var ctx = await listener.GetContextAsync();
            if (ctx.Request.HttpMethod == "GET" && ctx.Request.Url?.AbsolutePath == IpcConstants.HealthPath)
            {
                ctx.Response.StatusCode = 200;
                var responseBytes = Encoding.UTF8.GetBytes("""{"status":"running"}""");
                await ctx.Response.OutputStream.WriteAsync(responseBytes);
                ctx.Response.Close();
                return;
            }

            ctx.Response.StatusCode = 404;
            ctx.Response.Close();
        });

        var hookDir = Path.Combine(_tempRepoDir, ".copilotclimonitor");
        Directory.CreateDirectory(hookDir);
        File.WriteAllText(Path.Combine(hookDir, "notify.ps1"), "# test");
        File.WriteAllText(
            Path.Combine(hookDir, "config.json"),
            """
            {
              "version": "1.0",
              "repository": "integration-test-repo",
              "enabled": true,
              "notificationsEnabled": true,
              "events": ["task-completed"],
              "quietHours": {
                "enabled": false,
                "start": "22:00",
                "end": "08:00"
              },
              "routing": {
                "sourceTagging": true
              }
            }
            """);
        var githubHooksDir = Path.Combine(_tempRepoDir, ".github", "hooks");
        Directory.CreateDirectory(githubHooksDir);
        File.WriteAllText(Path.Combine(githubHooksDir, "copilotclimon-notify.json"), "{}");

        var detector = new FakeRepositoryDetector { RootToReturn = _tempRepoDir, BranchToReturn = "main" };
        var handlers = new CliCommandHandlers(
            detector,
            new FakeHookInstaller(),
            new HttpIpcClient(port),
            TextWriter.Null,
            TextWriter.Null);

        var exitCode = await handlers.RunDoctorAsync([]);
        await healthTask.WaitAsync(TimeSpan.FromSeconds(5));

        Assert.Equal(0, exitCode);
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
