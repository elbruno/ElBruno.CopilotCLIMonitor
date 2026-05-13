$ErrorActionPreference = 'Stop'

$toolsDir = Split-Path -Parent $MyInvocation.MyCommand.Definition
$zipPath = Join-Path $toolsDir 'ElBruno.CopilotCLIMonitor-portable-win-x64.zip'
$appRoot = Join-Path $env:ProgramData 'ElBruno.CopilotCLIMonitor'

if (Test-Path $appRoot) {
    Remove-Item -Path $appRoot -Recurse -Force
}

New-Item -ItemType Directory -Path $appRoot | Out-Null
Get-ChocolateyUnzip -FileFullPath $zipPath -Destination $appRoot

Install-ChocolateyShortcut `
  -ShortcutFilePath (Join-Path $env:ProgramData 'Microsoft\Windows\Start Menu\Programs\CopilotCLI Monitor.lnk') `
  -TargetPath (Join-Path $appRoot 'ElBruno.CopilotCLIMonitor.exe')
