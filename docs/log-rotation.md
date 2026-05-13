# Log file rotation

CopilotCLI Monitor writes structured logs to rotating files.

## Default location

- `%APPDATA%\CopilotCliMon\logs`

Override with:

- `COPILOTCLIMON_LOG_DIR`

## Rotation policy

- Daily rolling file: `copilotclimon-YYYYMMDD.log`
- Size roll-over: 10 MB per file
- Retention: last 14 files

## Console output

Logs are also emitted to console in compact JSON format.
