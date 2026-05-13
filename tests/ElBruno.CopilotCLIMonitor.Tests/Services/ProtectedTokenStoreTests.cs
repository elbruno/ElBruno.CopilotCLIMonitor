using ElBruno.CopilotCLIMonitor.Services;

namespace ElBruno.CopilotCLIMonitor.Tests.Services;

public sealed class ProtectedTokenStoreTests : IDisposable
{
    private readonly string _tempFile = Path.Combine(Path.GetTempPath(), $"copilotclimon-token-{Guid.NewGuid():N}.bin");

    [Fact]
    public void SaveAndLoadToken_RoundTripsValue()
    {
        var store = new ProtectedTokenStore(_tempFile);

        store.SaveToken("secret-token");
        var loaded = store.LoadToken();

        Assert.Equal("secret-token", loaded);
    }

    [Fact]
    public void LoadToken_WhenMissing_ReturnsNull()
    {
        var store = new ProtectedTokenStore(_tempFile);

        var loaded = store.LoadToken();

        Assert.Null(loaded);
    }

    [Fact]
    public void SaveToken_PersistsEncryptedBytes()
    {
        var store = new ProtectedTokenStore(_tempFile);

        store.SaveToken("plain-text");
        var content = File.ReadAllBytes(_tempFile);

        Assert.NotEmpty(content);
        Assert.DoesNotContain(System.Text.Encoding.UTF8.GetBytes("plain-text"), content);
    }

    public void Dispose()
    {
        if (File.Exists(_tempFile))
        {
            File.Delete(_tempFile);
        }
    }
}
