param(
    [string]$Configuration = "Release",
    [string]$OutputDirectory = ".\artifacts\coverage"
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot
$testProject = Join-Path $repoRoot "tests\ElBruno.CopilotCLIMonitor.Tests\ElBruno.CopilotCLIMonitor.Tests.csproj"
$resultsDir = Join-Path $OutputDirectory "results"

New-Item -ItemType Directory -Force -Path $resultsDir | Out-Null

dotnet test $testProject `
  -c $Configuration `
  --collect:"XPlat Code Coverage" `
  --results-directory $resultsDir `
  --logger "trx"

Write-Host "Coverage results stored in $resultsDir"
