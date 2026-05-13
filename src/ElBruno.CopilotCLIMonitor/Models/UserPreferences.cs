namespace ElBruno.CopilotCLIMonitor.Models;

public sealed class UserPreferences
{
    public bool NotificationsEnabled { get; set; } = true;
    public bool SoundEnabled { get; set; }
    public bool QuietHoursEnabled { get; set; }
    public int QuietHoursStart { get; set; } = 22;
    public int QuietHoursEnd { get; set; } = 7;
    public string LogLevel { get; set; } = "Information";
    public bool StartWithWindows { get; set; }
}
