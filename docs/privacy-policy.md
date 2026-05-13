# Privacy policy

Last updated: 2026-05-13

ElBruno.CopilotCLIMonitor is designed to run locally on user machines and minimize data exposure.

## Data collected and processed

The application may process:

- Notification event type
- Notification message text
- Repository name and branch (if provided by hook/CLI call)
- Timestamp of events
- User preferences stored locally (notifications, quiet hours, telemetry opt-in)

## Data storage

- Event history is stored in-memory for dashboard display.
- Preferences are stored locally at `%APPDATA%\CopilotCliMon\preferences.json`.
- Optional telemetry (if enabled) is stored locally at `%APPDATA%\CopilotCliMon\telemetry.log`.

## Telemetry

- Telemetry is **disabled by default**.
- When enabled, telemetry records anonymous event metadata and hashed repository values.
- Telemetry is not transmitted to third-party services by default implementation in this repository.

## Data sharing

This project does not intentionally send user event payloads to external services unless contributors add and configure external integrations.

## Security controls

- Optional IPC authentication token (`COPILOTCLIMON_IPC_TOKEN`)
- Optional encrypted token storage using Windows DPAPI (`%APPDATA%\CopilotCliMon\ipc-token.bin`)
- Input validation for IPC notify requests
- Security scanning in CI workflows

## User control

Users can:

- Disable notifications
- Enable/disable telemetry
- Clear dashboard history
- Remove local preference/telemetry files manually

## Contact

For privacy questions, open an issue in the repository:

`https://github.com/elbruno/ElBruno.CopilotCLIMonitor/issues`
