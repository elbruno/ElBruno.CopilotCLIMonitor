# Telemetry (optional, opt-in)

CopilotCLI Monitor supports optional anonymous telemetry.

## What is collected

- Event name (for example `app_start`, `event_received`)
- Event type label (for notification events)
- Installation identifier (random GUID generated when telemetry is enabled)
- Repository hash (SHA-256 of repository name, never raw repository text)
- UTC timestamp

## What is not collected

- Notification message text
- Branch names
- Raw repository names
- Usernames, tokens, or file contents

## Opt-in behavior

Telemetry is **disabled by default**.

Enable it from **System Tray → Settings → Share anonymous telemetry (opt-in)**.

## Storage

Telemetry is stored locally in:

`%APPDATA%\CopilotCliMon\telemetry.log`

One JSON line is written per event.

## ETW support

The application also emits ETW events via EventSource name:

`ElBruno-CopilotCliMonitor`

Current ETW events:

- `AppStartup`
- `EventReceived`
- `NotificationShown`
- `NotificationSuppressed`
