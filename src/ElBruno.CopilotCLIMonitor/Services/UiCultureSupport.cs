using System.Globalization;
using System.Windows;

namespace ElBruno.CopilotCLIMonitor.Services;

public static class UiCultureSupport
{
    public static System.Windows.FlowDirection GetFlowDirection(CultureInfo culture) =>
        culture.TextInfo.IsRightToLeft ? System.Windows.FlowDirection.RightToLeft : System.Windows.FlowDirection.LeftToRight;

    public static void ApplyFlowDirection(Window window, CultureInfo? culture = null)
    {
        window.FlowDirection = GetFlowDirection(culture ?? CultureInfo.CurrentUICulture);
    }
}
