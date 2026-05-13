# Standalone installer packaging

This repository includes a packaging script for a standalone Windows distribution.

## Build a standalone package

```powershell
.\scripts\create-installer.ps1
```

Output:
- `artifacts\installer\publish\` (published files)
- `artifacts\installer\ElBruno.CopilotCLIMonitor-win-x64.zip`

## Custom runtime or output folder

```powershell
.\scripts\create-installer.ps1 -Runtime win-arm64 -OutputDirectory .\artifacts\installer-arm64
```

## Notes

- Packaging is self-contained and single-file publish based.
- The generated ZIP can be distributed directly or wrapped by MSI/EXE tooling in release pipelines.
