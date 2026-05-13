# Windows Store / Microsoft Store package

This repository includes automation for producing an unsigned MSIX package for Partner Center submission.

## Prerequisites

- Windows 10/11 SDK installed (`makeappx.exe` available in `PATH`)
- .NET 10 SDK
- A Partner Center signing certificate (for final signing before upload)

## Build MSIX package

```powershell
.\scripts\create-microsoft-store-package.ps1 -Version 1.0.0
```

Output:

- `artifacts\microsoft-store\ElBruno.CopilotCLIMonitor-1.0.0-win-x64.msix`
- `artifacts\microsoft-store\store-submission-notes.txt`
- `artifacts\microsoft-store\publish\` (published binaries)
- `artifacts\microsoft-store\msix-content\` (MSIX staging layout)

## Custom runtime and identity

```powershell
.\scripts\create-microsoft-store-package.ps1 `
  -Version 1.0.0 `
  -Runtime win-arm64 `
  -PackageIdentityName "12345ElBruno.CopilotCLIMonitor" `
  -Publisher "CN=YOUR-PARTNER-CENTER-PUBLISHER"
```

## Publishing flow

1. Build the MSIX package with the target runtime.
2. Sign the package with your Partner Center certificate.
3. Validate with Windows App Certification Kit.
4. Submit package and store listing metadata in Microsoft Partner Center.
