# User manual

This manual covers daily usage of ElBruno.CopilotCLIMonitor for end users.

## 1. Start the monitor

Launch the monitor:

```powershell
copilotclimon
```

The app runs in the Windows system tray.

## 2. Initialize a repository

In each repository where you want notifications:

```powershell
copilotclimon init
```

This creates local hook configuration under `.copilotclimonitor`.

## 3. Validate setup

Run diagnostics:

```powershell
copilotclimon doctor
```

Expected checks:
- monitor process reachable
- hook directory exists
- hook script and config present

## 4. Trigger a test notification

```powershell
copilotclimon notify --event task-completed --message "Test notification"
```

You should see a Windows notification.

## 5. Common event types

- `task-completed`
- `approval-required`
- `error`
- `warning`
- `build-completed`
- `test-completed`
- `workflow-completed`

## 6. Daily workflow

1. Keep the tray app running.
2. Run Copilot CLI tasks in terminal.
3. Receive notifications when tasks complete or need attention.
4. Open the dashboard from tray menu to view recent event history.

## 7. Optional security configuration

To require authenticated IPC routing:

```powershell
$env:COPILOTCLIMON_IPC_TOKEN = "your-shared-secret"
```

Set the same value in both monitor and CLI execution environments.

## 8. Quiet hours configuration

Use tray menu **Settings** to configure quiet hours:

1. Enable **Quiet hours**.
2. Choose start and end hour (24-hour format).
3. Save settings.

During quiet hours, notifications are muted but events are still stored in dashboard history.

## 9. Troubleshooting quick checks

- Re-run `copilotclimon doctor`
- Verify repository has `.copilotclimonitor\notify.ps1`
- Ensure tray app is running
- Check `docs/troubleshooting.md` for detailed fixes
