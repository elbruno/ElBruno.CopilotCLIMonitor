using System.Globalization;
using System.Resources;

namespace ElBruno.CopilotCLIMonitor.Services;

public static class UiResources
{
    private static readonly ResourceManager ResourceManager = new("ElBruno.CopilotCLIMonitor.Resources.UiStrings", typeof(UiResources).Assembly);

    public static string Get(string key, params object[] arguments)
    {
        var value = ResourceManager.GetString(key, CultureInfo.CurrentUICulture) ?? key;
        return arguments.Length == 0 ? value : string.Format(CultureInfo.CurrentUICulture, value, arguments);
    }
}
