using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ElBruno.CopilotCLIMonitor.Services;

public sealed class ProtectedTokenStore
{
    private readonly string _tokenPath;

    public ProtectedTokenStore(string? tokenPath = null)
    {
        _tokenPath = tokenPath ?? GetDefaultPath();
    }

    public void SaveToken(string token)
    {
        var tokenBytes = Encoding.UTF8.GetBytes(token);
        var protectedToken = ProtectedData.Protect(tokenBytes, optionalEntropy: null, scope: DataProtectionScope.CurrentUser);
        var directory = Path.GetDirectoryName(_tokenPath);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        File.WriteAllBytes(_tokenPath, protectedToken);
    }

    public string? LoadToken()
    {
        if (!File.Exists(_tokenPath))
        {
            return null;
        }

        var protectedToken = File.ReadAllBytes(_tokenPath);
        if (protectedToken.Length == 0)
        {
            return null;
        }

        var plainBytes = ProtectedData.Unprotect(protectedToken, optionalEntropy: null, scope: DataProtectionScope.CurrentUser);
        return Encoding.UTF8.GetString(plainBytes);
    }

    internal static string GetDefaultPath()
    {
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        return Path.Combine(appData, "CopilotCliMon", "ipc-token.bin");
    }
}
