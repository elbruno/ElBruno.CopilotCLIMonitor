# Chocolatey package

This repository includes Chocolatey packaging assets for `elbruno.copilotclimonitor`.

## Prerequisites

- Chocolatey installed (`choco` command available)
- Portable ZIP already generated

## Build package

1. Generate portable ZIP:

```powershell
.\scripts\create-portable-zip.ps1
```

2. Create Chocolatey package:

```powershell
.\scripts\create-chocolatey-package.ps1 -Version 1.0.0
```

Output package:

- `artifacts\chocolatey\elbruno.copilotclimonitor.1.0.0.nupkg`

## Package contents

- Chocolatey install/uninstall scripts
- Portable ZIP payload
- Start menu shortcut creation on install
