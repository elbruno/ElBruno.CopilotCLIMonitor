# Diagnostic mode

Diagnostic mode enables verbose logging for troubleshooting.

## Enable

```powershell
copilotclimon diagnostic --enable
```

This sets:

- `COPILOTCLIMON_DIAGNOSTIC_MODE=true`
- `COPILOTCLIMON_LOG_LEVEL=Debug`

## Disable

```powershell
copilotclimon diagnostic --disable
```

## Check status

```powershell
copilotclimon diagnostic
```
