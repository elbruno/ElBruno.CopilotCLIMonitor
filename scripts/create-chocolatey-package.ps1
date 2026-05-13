param(
    [Parameter(Mandatory = $true)]
    [string]$Version,
    [string]$Runtime = "win-x64",
    [string]$OutputDirectory = ".\artifacts\chocolatey"
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot
$portableZip = Join-Path $repoRoot "artifacts\portable\ElBruno.CopilotCLIMonitor-portable-$Runtime.zip"
$chocoRoot = Join-Path $repoRoot "packaging\chocolatey"
$toolsDir = Join-Path $chocoRoot "tools"

if (-not (Test-Path $portableZip)) {
    throw "Portable ZIP not found at '$portableZip'. Run scripts/create-portable-zip.ps1 first."
}

Copy-Item -Path $portableZip -Destination (Join-Path $toolsDir "ElBruno.CopilotCLIMonitor-portable-$Runtime.zip") -Force

New-Item -ItemType Directory -Force -Path $OutputDirectory | Out-Null
Push-Location $chocoRoot
try {
    choco pack .\elbruno.copilotclimonitor.nuspec --version $Version --outputdirectory $OutputDirectory
}
finally {
    Pop-Location
}

Write-Host "Chocolatey package created in $OutputDirectory"
