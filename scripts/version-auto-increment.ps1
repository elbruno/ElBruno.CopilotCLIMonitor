param(
    [ValidateSet("major", "minor", "patch")]
    [string]$Increment = "patch",
    [string]$PreReleaseLabel,
    [switch]$DryRun
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot
$coreProject = Join-Path $repoRoot "src\ElBruno.CopilotCLIMonitor.Core\ElBruno.CopilotCLIMonitor.Core.csproj"
$versionBumpScript = Join-Path $PSScriptRoot "version-bump.ps1"

if (-not (Test-Path $coreProject)) {
    throw "Core project not found at $coreProject"
}

if (-not (Test-Path $versionBumpScript)) {
    throw "version-bump.ps1 not found at $versionBumpScript"
}

$content = Get-Content $coreProject -Raw
if ($content -notmatch '<Version>([^<]+)</Version>') {
    throw "Current version not found in $coreProject"
}

$rawVersion = $matches[1]
if ($rawVersion -notmatch '^(\d+)\.(\d+)\.(\d+)(?:-[a-zA-Z0-9.]+)?$') {
    throw "Unsupported version format '$rawVersion'."
}

$major = [int]$matches[1]
$minor = [int]$matches[2]
$patch = [int]$matches[3]

switch ($Increment) {
    "major" {
        $major++
        $minor = 0
        $patch = 0
    }
    "minor" {
        $minor++
        $patch = 0
    }
    "patch" {
        $patch++
    }
}

$nextVersion = "$major.$minor.$patch"
if (-not [string]::IsNullOrWhiteSpace($PreReleaseLabel)) {
    $nextVersion = "$nextVersion-$PreReleaseLabel"
}

Write-Host "Current version: $rawVersion"
Write-Host "Next version: $nextVersion"

if ($DryRun) {
    & $versionBumpScript -Version $nextVersion -DryRun
}
else {
    & $versionBumpScript -Version $nextVersion
}
