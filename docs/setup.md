# Setup guide

## Prerequisites

- **Windows 10/11** – The Systray application is Windows-only
- **.NET 10 runtime** – [Download .NET 10](https://dotnet.microsoft.com/download)
- **GitHub Copilot CLI** – Install separately if needed

## Installation

### Step 1: Install the .NET tool

```powershell
dotnet tool install -g ElBruno.CopilotCLIMonitor
```

Verify installation:

```powershell
copilotclimon --version
```

### Step 2: Start the Systray application

```powershell
copilotclimon
```

The application starts in the background. Look for the notification icon in your Windows system tray.

### Step 3: Initialize your repository

Navigate to your repository and run:

```bash
cd your-repository
copilotclimon init
```

`init` is interactive by default and lets you choose which Copilot hook triggers to register.
Generated hook entries include `powershell` and `bash`/`command` fields so they run on Windows-local Copilot CLI and Linux-based Copilot surfaces.

For automation/CI, use:

```bash
copilotclimon init --default
```

To overwrite managed files (`.copilotclimonitor/*` and `.github/hooks/copilotclimon-notify.json`), use:

```bash
copilotclimon init --default --force
```

Expected output:

```
✓ Repository detected: my-repo
✓ Hooks installed successfully
✓ Event forwarding configured
✓ Validation passed

Your repository is ready for notifications!
```

### Step 4: Test the setup

Validate that hooks are working:

```bash
copilotclimon doctor
```

This command checks:
- Hook files are present
- Hook `config.json` schema is valid
- Hook configuration is valid
- Systray application is running
- Network connectivity is available

After creating or editing hook files, restart Copilot CLI so it reloads hook configuration.

### Upgrade repository hook/config templates

After installing a newer version of the tool, refresh managed repository files:

```bash
copilotclimon upgrade
```

### Step 5: Test a notification

Send a test notification:

```bash
copilotclimon notify --event test --message "Test notification"
```

You should see a Windows toast notification appear.

## Configuration

### Global settings

Open settings from the Systray context menu:

1. Click the notification icon in the system tray
2. Select "Settings" (menu: Open Dashboard, Settings, About, Exit)
3. Configure preferences

**Suggested settings options:**
- Notifications: enabled, toast duration, sound on/off
- Focus mode: quiet hours and priority-only alerts
- Filters: repository include/exclude and event-type filters
- Startup: launch at Windows login
- Data and diagnostics: max recent events, retention period, log level

When sound is enabled, alert tones are event-aware:

- Error / hook failure → system **Hand** sound
- Warning / approval required / long-running warning → system **Exclamation** sound
- Completed events (task/build/test/workflow) → system **Asterisk** sound

Preferences are loaded in this order:

1. `%ProgramData%\CopilotCliMon\preferences.system.json` (machine-wide defaults)
2. `%AppData%\CopilotCliMon\preferences.json` (per-user overrides)
3. Environment variables (highest priority)

### Environment variable overrides

Set one or more of these variables to override file-based settings:

- `COPILOTCLIMON_PREFERENCES_PATH`
- `COPILOTCLIMON_SYSTEM_PREFERENCES_PATH`
- `COPILOTCLIMON_NOTIFICATIONS_ENABLED`
- `COPILOTCLIMON_SOUND_ENABLED`
- `COPILOTCLIMON_QUIET_HOURS_ENABLED`
- `COPILOTCLIMON_QUIET_HOURS_START`
- `COPILOTCLIMON_QUIET_HOURS_END`
- `COPILOTCLIMON_LOG_LEVEL`
- `COPILOTCLIMON_START_WITH_WINDOWS`
- `COPILOTCLIMON_TELEMETRY_OPT_IN`
- `COPILOTCLIMON_TELEMETRY_INSTALLATION_ID`

Example:

```powershell
$env:COPILOTCLIMON_LOG_LEVEL = "Debug"
$env:COPILOTCLIMON_NOTIFICATIONS_ENABLED = "true"
```

### Repository-specific configuration

Repository configuration is stored in `.copilotclimonitor`:

```text
.copilotclimonitor/
├── config.json
└── notify.ps1

.github/hooks/
└── copilotclimon-notify.json
```

Edit `.copilotclimonitor/config.json` to customize behavior per repository:

```json
{
  "enabled": true,
  "notificationTypes": ["toast", "sound"],
  "toastDuration": 5,
  "quiet-hours": {
    "enabled": false,
    "start": "22:00",
    "end": "08:00"
  },
  "event-filters": {
    "task-completed": true,
    "approval-required": true,
    "error": true,
    "agent-waiting": false
  }
}
```

Repository-level filtering is applied by `copilotclimon notify`:

- If `enabled` is `false`, notifications for that repository are skipped.
- If `notificationsEnabled` is `false`, notifications are skipped.
- Only event names listed in `events` are forwarded to the monitor.

By default, first-time initialization enables core event notifications, includes build/test/workflow events, and sets quiet hours to disabled.

## Upgrading

Update to the latest version:

```powershell
dotnet tool update -g ElBruno.CopilotCLIMonitor
```

Then restart the Systray application:

1. Right-click the notification icon in the system tray
2. Select "Exit"
3. Run `copilotclimon` again

## Uninstalling

Remove the .NET tool:

```powershell
dotnet tool uninstall -g ElBruno.CopilotCLIMonitor
```

Optional: Remove local repository configuration:

```bash
rm -r .copilotclimonitor
```

## Startup behavior

### Auto-start on Windows boot

Enable from Systray settings → "Startup on boot"

When enabled, the app writes a per-user startup entry in:

```text
HKCU\Software\Microsoft\Windows\CurrentVersion\Run
```

Or manually add to Windows Startup:

1. Press `Win + R`
2. Type `shell:startup`
3. Create a shortcut to `copilotclimon`

### Manual startup

```powershell
copilotclimon
```

## Logging

Logs are stored locally for debugging:

```text
%AppData%\ElBruno\CopilotCLIMonitor\logs\
```

View recent logs from Systray menu → "Open logs"

Enable debug logging from Settings → "Logging level" → "Debug"

## Troubleshooting

See [troubleshooting.md](troubleshooting.md) for common issues.

## Next steps

- Review [architecture.md](architecture.md) for technical details
- Check [examples.md](examples.md) for hook integration samples
- Read [hook-configuration.md](hook-configuration.md) for advanced setup
