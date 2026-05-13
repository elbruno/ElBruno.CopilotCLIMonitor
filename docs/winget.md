# WinGet package

This repository includes automation for generating WinGet manifests.

## Generate manifests

```powershell
.\scripts\create-winget-manifest.ps1 `
  -Version 1.0.0 `
  -InstallerUrl "https://github.com/elbruno/ElBruno.CopilotCLIMonitor/releases/download/v1.0.0/ElBruno.CopilotCLIMonitor-portable-win-x64.zip" `
  -InstallerSha256 "<SHA256>"
```

Output location:

- `packaging/winget/manifests/`

Files generated:

- `ElBruno.CopilotCLIMonitor.yaml`
- `ElBruno.CopilotCLIMonitor.installer.yaml`
- `ElBruno.CopilotCLIMonitor.locale.en-US.yaml`

## Publishing flow

1. Generate manifests with release-specific URL and SHA256.
2. Validate manifests with `winget validate` (if WinGet tooling is available).
3. Submit to `microsoft/winget-pkgs` repository.
