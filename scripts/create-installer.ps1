# Build a standalone Windows package for ElBruno.CopilotCLIMonitor.
param(
    [string]$Configuration = "Release",
    [string]$Runtime = "win-x64",
    [string]$OutputDirectory = ".\artifacts\installer"
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot
$project = Join-Path $repoRoot "src\ElBruno.CopilotCLIMonitor\ElBruno.CopilotCLIMonitor.csproj"
$publishDir = Join-Path $OutputDirectory "publish"

New-Item -ItemType Directory -Force -Path $publishDir | Out-Null

dotnet publish $project `
  -c $Configuration `
  -r $Runtime `
  --self-contained true `
  /p:PublishSingleFile=true `
  /p:PublishTrimmed=false `
  -o $publishDir

$zipPath = Join-Path $OutputDirectory "ElBruno.CopilotCLIMonitor-$Runtime.zip"
if (Test-Path $zipPath) { Remove-Item $zipPath -Force }
Compress-Archive -Path (Join-Path $publishDir "*") -DestinationPath $zipPath -Force

Write-Host "Standalone package created: $zipPath"
