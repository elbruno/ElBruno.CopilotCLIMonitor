# Troubleshooting

## Common issues

### Application doesn't start

**Symptom:** Running `copilotclimon` produces no output or error.

**Solutions:**

1. Verify .NET 10 installation:

```powershell
dotnet --version
```

Should show version 10.0.0 or later.

2. Reinstall the tool:

```powershell
dotnet tool uninstall -g ElBruno.CopilotCLIMonitor
dotnet tool install -g ElBruno.CopilotCLIMonitor
```

3. Check for port conflicts:

```powershell
netstat -ano | findstr :8765
```

If a process is using the default port, update configuration to use a different port.

### Hooks not installed

**Symptom:** Running `copilotclimon init` shows an error.

**Solutions:**

1. Verify you're in a Git repository:

```bash
git rev-parse --git-dir
```

2. Check directory permissions:

```bash
ls -la .copilotclimonitor/
```

Ensure the directory is readable and writable.

3. Try manual hook installation:

```bash
copilotclimon init --verbose
```

Review the verbose output for specific errors.

### Notifications not appearing

**Symptom:** Events are processed but no toast notifications appear.

**Solutions:**

1. Check if Systray application is running:

```powershell
Get-Process -Name "ElBruno.CopilotCLIMonitor" -ErrorAction SilentlyContinue
```

If not running, start it:

```powershell
copilotclimon
```

2. Verify notification settings:

Open Systray → Settings → Notifications. Ensure notifications are enabled.

3. Check Windows notification settings:

Settings → System → Notifications & actions → Ensure notifications are enabled for "Apps"

4. Send a test notification:

```bash
copilotclimon notify --event test --message "Test"
```

5. Review logs:

```powershell
Get-Content "$env:APPDATA\ElBruno\CopilotCLIMonitor\logs\*.log" -Tail 20
```

### "Permission denied" errors

**Symptom:** Hook installation fails with permission errors.

**Solutions:**

1. Ensure repository directory is writable:

```bash
touch .copilotclimonitor/test.txt && rm .copilotclimonitor/test.txt
```

2. Run with administrator privileges (if necessary):

```powershell
# Run PowerShell as administrator
copilotclimon init
```

Note: The application should work without admin privileges. If admin is required, there may be a configuration issue.

3. Check file permissions on existing hooks:

```bash
ls -la .copilotclimonitor/hooks/
```

### "IPC communication failed" error

**Symptom:** `copilotclimon notify` fails with communication error.

**Solutions:**

1. Restart the Systray application:

```powershell
# Kill existing process
Stop-Process -Name "ElBruno.CopilotCLIMonitor" -Force -ErrorAction SilentlyContinue

# Start new instance
copilotclimon
```

2. Check for firewall blocking:

Ensure Windows Firewall allows local communication on the configured port.

3. Verify localhost is accessible:

```powershell
Test-NetConnection -ComputerName 127.0.0.1 -Port 8765
```

(Replace 8765 with your configured port)

### Configuration not persisting

**Symptom:** Settings changes are lost after application restart.

**Solutions:**

1. Verify configuration file location:

```powershell
ls "$env:APPDATA\ElBruno\CopilotCLIMonitor\config.json"
```

2. Check file permissions:

```powershell
Get-Acl "$env:APPDATA\ElBruno\CopilotCLIMonitor\config.json"
```

Ensure your user has write permissions.

3. Manual configuration edit:

Edit the configuration file directly:

```json
{
  "notificationTypes": ["toast", "sound"],
  "toastDuration": 5,
  "quietHours": {
    "enabled": false
  }
}
```

### High memory usage

**Symptom:** Systray application consuming excessive memory.

**Solutions:**

1. Clear event history:

Systray menu → "Clear history"

2. Disable debug logging:

Settings → Logging level → "Info"

3. Reduce event retention:

Edit `.copilotclimonitor/config.json`:

```json
{
  "eventRetention": 100
}
```

### Windows notifications don't make sound

**Symptom:** Toast notifications appear but no sound plays.

**Solutions:**

1. Check Windows notification sound settings:

Settings → System → Notifications & actions → Sound settings

2. Enable sound in application settings:

Systray menu → Settings → Sound enabled: ON

3. Test system notification sounds:

Settings → System → Sound → Volume mixer

### Copilot CLI integration not working

**Symptom:** Copilot CLI completes but no notification appears.

**Solutions:**

1. Verify hook files exist:

```bash
ls -la .copilotclimonitor/hooks/
```

2. Test hook manually:

```bash
bash .copilotclimonitor/hooks/on-task-completed.sh
```

3. Check Copilot CLI hook configuration:

```bash
copilot --help | grep hook
```

Ensure Copilot CLI supports the hook you're trying to use.

4. Enable debug logging:

```bash
ELBRUNODEBUG=1 bash .copilotclimonitor/hooks/on-task-completed.sh
```

## Getting help

### Enable debug logging

Set environment variable:

```powershell
$env:ELBRUNODEBUG = "1"
copilotclimon
```

This produces verbose output helpful for diagnostics.

### Check logs

View application logs:

```powershell
$logsPath = "$env:APPDATA\ElBruno\CopilotCLIMonitor\logs"
Get-ChildItem $logsPath -Recurse | Sort-Object LastWriteTime -Descending | Select-Object -First 5
Get-Content (Get-ChildItem $logsPath -Recurse | Sort-Object LastWriteTime -Descending | Select-Object -First 1).FullName
```

### Run diagnostics

```bash
copilotclimon doctor
```

This produces a detailed system check report.

### Report an issue

When reporting issues on GitHub, include:

1. Output from `copilotclimon doctor`
2. Relevant log files (last 50 lines)
3. Steps to reproduce
4. Windows version (10/11)
5. .NET version
6. Application version

Visit: https://github.com/elbruno/ElBruno.CopilotCLIMonitor/issues
