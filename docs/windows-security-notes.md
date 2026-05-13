# Windows Defender / security notes

ElBruno.CopilotCLIMonitor is a local desktop utility and may trigger security prompts in strict enterprise environments.

## Defender behavior

- Unsigned local builds can trigger SmartScreen warnings.
- Real-time protection may inspect hook scripts generated in repositories.
- First launch after download may be slower due to Defender scanning.

## Recommended practices

1. Use release binaries from trusted repository releases.
2. Keep Defender enabled; avoid blanket exclusions.
3. If needed, exclude only specific development folders (not entire drives).
4. Validate SHA/checksum of downloaded artifacts in secured environments.

## Enterprise environments

- Coordinate with IT/security teams for approved allow-list rules.
- Prefer signed distributions (MSI/Chocolatey in controlled channels).
- Document required executable paths and hash/version per release.

## Hook script considerations

Repository hook scripts are plain PowerShell/Bash files under:

- `.copilotclimonitor/notify.ps1`
- `.copilotclimonitor/hooks/*.sh`

These are expected by design and can be reviewed before execution.
