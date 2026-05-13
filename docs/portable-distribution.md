# Portable ZIP distribution

Create a no-installation portable package for Windows from source.

## Generate portable ZIP

```powershell
.\scripts\create-portable-zip.ps1
```

Output:

- `artifacts\portable\publish\` (published binary)
- `artifacts\portable\ElBruno.CopilotCLIMonitor-portable-win-x64.zip`

## Custom runtime or output folder

```powershell
.\scripts\create-portable-zip.ps1 -Runtime win-arm64 -OutputDirectory .\artifacts\portable-arm64
```

## Usage after unzip

1. Extract ZIP to any folder.
2. Run `ElBruno.CopilotCLIMonitor.exe`.
3. The app starts in the Windows system tray without requiring installation.
