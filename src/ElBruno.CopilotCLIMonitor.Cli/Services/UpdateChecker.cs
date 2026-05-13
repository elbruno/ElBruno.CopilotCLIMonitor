using System.Diagnostics;
using System.Text.Json;

namespace ElBruno.CopilotCLIMonitor.Cli.Services;

public sealed class UpdateChecker
{
    private static readonly Uri VersionsEndpoint = new("https://api.nuget.org/v3-flatcontainer/elbruno.copilotclimonitor/index.json");

    public async Task<string?> GetLatestVersionAsync(CancellationToken cancellationToken = default)
    {
        using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(10) };
        var json = await client.GetStringAsync(VersionsEndpoint, cancellationToken);
        using var doc = JsonDocument.Parse(json);

        if (!doc.RootElement.TryGetProperty("versions", out var versions) || versions.ValueKind != JsonValueKind.Array)
        {
            return null;
        }

        Version? latest = null;
        foreach (var v in versions.EnumerateArray())
        {
            if (v.ValueKind != JsonValueKind.String)
            {
                continue;
            }

            var candidate = v.GetString();
            if (!TryParseSemVerCore(candidate, out var parsed))
            {
                continue;
            }

            if (latest is null || parsed > latest)
            {
                latest = parsed;
            }
        }

        return latest?.ToString();
    }

    public bool IsUpdateAvailable(string currentVersion, string latestVersion)
    {
        if (!TryParseSemVerCore(currentVersion, out var current))
        {
            return false;
        }

        if (!TryParseSemVerCore(latestVersion, out var latest))
        {
            return false;
        }

        return latest > current;
    }

    public int InstallLatest()
    {
        var psi = new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = "tool update -g ElBruno.CopilotCLIMonitor",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        };

        using var process = Process.Start(psi);
        if (process is null)
        {
            return 1;
        }

        process.WaitForExit();
        return process.ExitCode;
    }

    private static bool TryParseSemVerCore(string? value, out Version version)
    {
        version = new Version(0, 0, 0);
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        var core = value.Split('-', 2)[0];
        if (!Version.TryParse(core, out var parsed) || parsed is null)
        {
            return false;
        }

        version = parsed;
        return true;
    }
}
