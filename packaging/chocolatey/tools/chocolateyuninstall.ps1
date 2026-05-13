$ErrorActionPreference = 'Stop'

$appRoot = Join-Path $env:ProgramData 'ElBruno.CopilotCLIMonitor'
$shortcutPath = Join-Path $env:ProgramData 'Microsoft\Windows\Start Menu\Programs\CopilotCLI Monitor.lnk'

if (Test-Path $shortcutPath) {
    Remove-Item -Path $shortcutPath -Force
}

if (Test-Path $appRoot) {
    Remove-Item -Path $appRoot -Recurse -Force
}
