# Build an unsigned MSIX package layout for Microsoft Store submission.
param(
    [Parameter(Mandatory = $true)]
    [string]$Version,
    [string]$Configuration = "Release",
    [string]$Runtime = "win-x64",
    [string]$OutputDirectory = ".\artifacts\microsoft-store",
    [string]$PackageIdentityName = "ElBruno.CopilotCLIMonitor",
    [string]$PackageDisplayName = "CopilotCLI Monitor",
    [string]$Publisher = "CN=ElBruno"
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot
$project = Join-Path $repoRoot "src\ElBruno.CopilotCLIMonitor\ElBruno.CopilotCLIMonitor.csproj"
$outputRoot = Join-Path $repoRoot $OutputDirectory
$publishDir = Join-Path $outputRoot "publish"
$packageRoot = Join-Path $outputRoot "msix-content"
$assetsDir = Join-Path $packageRoot "Assets"

New-Item -ItemType Directory -Force -Path $publishDir | Out-Null
New-Item -ItemType Directory -Force -Path $assetsDir | Out-Null

dotnet publish $project `
  -c $Configuration `
  -r $Runtime `
  --self-contained true `
  /p:PublishSingleFile=true `
  /p:PublishTrimmed=false `
  -o $publishDir

if (Test-Path $packageRoot) {
    Remove-Item -Path $packageRoot -Recurse -Force
}
New-Item -ItemType Directory -Force -Path $packageRoot | Out-Null
New-Item -ItemType Directory -Force -Path $assetsDir | Out-Null
New-Item -ItemType Directory -Force -Path (Join-Path $packageRoot "App") | Out-Null

Copy-Item -Path (Join-Path $publishDir "*") -Destination (Join-Path $packageRoot "App") -Recurse -Force

$iconSource = Join-Path $repoRoot "images\nuget-icon.png"
if (-not (Test-Path $iconSource)) {
    throw "Icon file not found at '$iconSource'."
}

Copy-Item $iconSource (Join-Path $assetsDir "Square44x44Logo.png") -Force
Copy-Item $iconSource (Join-Path $assetsDir "Square150x150Logo.png") -Force
Copy-Item $iconSource (Join-Path $assetsDir "StoreLogo.png") -Force

$manifestPath = Join-Path $packageRoot "AppxManifest.xml"
$manifest = @"
<?xml version="1.0" encoding="utf-8"?>
<Package
  xmlns="http://schemas.microsoft.com/appx/manifest/foundation/windows10"
  xmlns:uap="http://schemas.microsoft.com/appx/manifest/uap/windows10"
  xmlns:desktop="http://schemas.microsoft.com/appx/manifest/desktop/windows10"
  IgnorableNamespaces="uap desktop">
  <Identity Name="$PackageIdentityName" Publisher="$Publisher" Version="$Version.0" />
  <Properties>
    <DisplayName>$PackageDisplayName</DisplayName>
    <PublisherDisplayName>ElBruno</PublisherDisplayName>
    <Logo>Assets\StoreLogo.png</Logo>
  </Properties>
  <Resources>
    <Resource Language="en-us" />
  </Resources>
  <Dependencies>
    <TargetDeviceFamily Name="Windows.Desktop" MinVersion="10.0.17763.0" MaxVersionTested="10.0.22631.0" />
  </Dependencies>
  <Applications>
    <Application Id="CopilotCliMonitor" Executable="App\ElBruno.CopilotCLIMonitor.exe" EntryPoint="Windows.FullTrustApplication">
      <uap:VisualElements
        DisplayName="$PackageDisplayName"
        Description="Windows tray monitor for GitHub Copilot CLI events."
        BackgroundColor="transparent"
        Square150x150Logo="Assets\Square150x150Logo.png"
        Square44x44Logo="Assets\Square44x44Logo.png">
        <uap:DefaultTile Wide310x150Logo="Assets\Square150x150Logo.png" />
        <uap:SplashScreen Image="Assets\Square150x150Logo.png" />
      </uap:VisualElements>
      <Extensions>
        <desktop:Extension Category="windows.fullTrustProcess" Executable="App\ElBruno.CopilotCLIMonitor.exe" />
      </Extensions>
    </Application>
  </Applications>
</Package>
"@
$manifest | Set-Content -Path $manifestPath -Encoding UTF8

$makeAppx = (Get-Command makeappx.exe -ErrorAction SilentlyContinue)?.Source
if (-not $makeAppx) {
    throw "makeappx.exe was not found. Install Windows 10/11 SDK (App Certification Kit tools) and rerun."
}

$msixPath = Join-Path $outputRoot "ElBruno.CopilotCLIMonitor-$Version-$Runtime.msix"
if (Test-Path $msixPath) {
    Remove-Item $msixPath -Force
}

& $makeAppx pack /d $packageRoot /p $msixPath /o | Out-Host

$notesPath = Join-Path $outputRoot "store-submission-notes.txt"
@"
Microsoft Store package created:
$msixPath

Next steps:
1. Sign the MSIX with your Partner Center certificate.
2. Run Windows App Certification Kit validation.
3. Upload the signed package in Microsoft Partner Center.
"@ | Set-Content -Path $notesPath -Encoding UTF8

Write-Host "Microsoft Store package created: $msixPath"
Write-Host "Submission notes: $notesPath"
