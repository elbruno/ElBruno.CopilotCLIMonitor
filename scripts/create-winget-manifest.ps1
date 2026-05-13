param(
    [Parameter(Mandatory = $true)]
    [string]$Version,
    [Parameter(Mandatory = $true)]
    [string]$InstallerUrl,
    [Parameter(Mandatory = $true)]
    [string]$InstallerSha256,
    [string]$OutputDirectory = ".\packaging\winget\manifests"
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot
$outputRoot = Join-Path $repoRoot $OutputDirectory
New-Item -ItemType Directory -Force -Path $outputRoot | Out-Null

$packageId = "ElBruno.CopilotCLIMonitor"
$manifestVersion = "1.9.0"

$versionFile = Join-Path $outputRoot "$packageId.yaml"
$installerFile = Join-Path $outputRoot "$packageId.installer.yaml"
$localeFile = Join-Path $outputRoot "$packageId.locale.en-US.yaml"

@"
PackageIdentifier: $packageId
PackageVersion: $Version
DefaultLocale: en-US
ManifestType: version
ManifestVersion: $manifestVersion
"@ | Set-Content -Path $versionFile

@"
PackageIdentifier: $packageId
PackageVersion: $Version
InstallerType: zip
Installers:
  - Architecture: x64
    InstallerUrl: $InstallerUrl
    InstallerSha256: $InstallerSha256
ManifestType: installer
ManifestVersion: $manifestVersion
"@ | Set-Content -Path $installerFile

@"
PackageIdentifier: $packageId
PackageVersion: $Version
PackageLocale: en-US
Publisher: El Bruno
PublisherUrl: https://github.com/elbruno
PackageName: CopilotCLI Monitor
PackageUrl: https://github.com/elbruno/ElBruno.CopilotCLIMonitor
ShortDescription: Windows tray monitor for GitHub Copilot CLI events.
License: MIT
LicenseUrl: https://github.com/elbruno/ElBruno.CopilotCLIMonitor/blob/main/LICENSE
ManifestType: defaultLocale
ManifestVersion: $manifestVersion
"@ | Set-Content -Path $localeFile

Write-Host "WinGet manifests created in $outputRoot"
