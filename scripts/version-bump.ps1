# Version Bump Script for ElBruno.CopilotCLIMonitor
# Automatically updates version across all .csproj files

param (
    [Parameter(Mandatory=$true)]
    [string]$Version,
    
    [Parameter(Mandatory=$false)]
    [switch]$DryRun,
    
    [Parameter(Mandatory=$false)]
    [string]$WorkingDirectory = $PSScriptRoot
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

# Validation
if ($Version -notmatch '^\d+\.\d+\.\d+(-[a-zA-Z0-9.]+)?$') {
    Write-Error "Invalid version format: $Version. Use semantic versioning (e.g., 1.0.0 or 1.0.0-beta)"
}

# Get repository root
$repoRoot = Split-Path -Parent $WorkingDirectory
$solutionPath = Join-Path $repoRoot "ElBruno.CopilotCLIMonitor.sln"

if (-not (Test-Path $solutionPath)) {
    Write-Error "Solution file not found at $solutionPath"
}

Write-Host "🔄 Version Bump Script" -ForegroundColor Cyan
Write-Host "Target Version: $Version" -ForegroundColor Yellow
if ($DryRun) {
    Write-Host "Mode: DRY RUN (no changes will be committed)" -ForegroundColor Yellow
}

# Find all .csproj files (exclude test projects)
$csprojFiles = @(
    Join-Path $repoRoot "src\ElBruno.CopilotCLIMonitor.Core\ElBruno.CopilotCLIMonitor.Core.csproj"
    Join-Path $repoRoot "src\ElBruno.CopilotCLIMonitor.Cli\ElBruno.CopilotCLIMonitor.Cli.csproj"
    Join-Path $repoRoot "src\ElBruno.CopilotCLIMonitor\ElBruno.CopilotCLIMonitor.csproj"
)

$updatedFiles = @()

foreach ($csprojFile in $csprojFiles) {
    if (-not (Test-Path $csprojFile)) {
        Write-Warning "File not found: $csprojFile"
        continue
    }

    Write-Host "`nProcessing: $($csprojFile | Split-Path -Leaf)" -ForegroundColor Cyan
    
    $content = Get-Content $csprojFile -Raw
    
    # Check if file already has Version tag
    if ($content -match '<Version>([^<]+)</Version>') {
        $oldVersion = $matches[1]
        $newContent = $content -replace '<Version>[^<]+</Version>', "<Version>$Version</Version>"
        Write-Host "  ✓ Updated version: $oldVersion → $Version"
    } else {
        # If no Version tag exists, add it after the root PropertyGroup start
        if ($content -match '(<PropertyGroup>)') {
            $newContent = $content -replace '(<PropertyGroup>)', "`$1`n    <Version>$Version</Version>"
            Write-Host "  ✓ Added version tag: $Version"
        } else {
            Write-Warning "Could not find PropertyGroup in $csprojFile"
            continue
        }
    }
    
    if (-not $DryRun) {
        Set-Content -Path $csprojFile -Value $newContent -NoNewline
        Write-Host "  ✓ File saved" -ForegroundColor Green
    } else {
        Write-Host "  [DRY RUN] Changes not saved" -ForegroundColor Gray
    }
    
    $updatedFiles += $csprojFile
}

if ($updatedFiles.Count -eq 0) {
    Write-Error "No .csproj files were updated"
}

Write-Host "`n✅ Version update complete" -ForegroundColor Green
Write-Host "Updated files: $($updatedFiles.Count)" -ForegroundColor Cyan

if (-not $DryRun) {
    # Git operations
    Write-Host "`n📝 Git operations:" -ForegroundColor Cyan
    
    try {
        # Stage changed files
        & git add $updatedFiles
        Write-Host "  ✓ Files staged" -ForegroundColor Green
        
        # Create commit
        $commitMessage = "chore: bump version to $Version`n`nAutomated version bump via version-bump.ps1"
        & git commit -m $commitMessage
        Write-Host "  ✓ Commit created" -ForegroundColor Green
        
        # Get commit hash
        $commitHash = & git rev-parse HEAD
        Write-Host "`n🎯 Release Ready:" -ForegroundColor Cyan
        Write-Host "  Version: $Version" -ForegroundColor Yellow
        Write-Host "  Commit: $commitHash" -ForegroundColor Yellow
        Write-Host "  Message: Version bump to $Version" -ForegroundColor Yellow
        
    } catch {
        Write-Error "Git operation failed: $_"
    }
} else {
    Write-Host "`n[DRY RUN] No git operations performed" -ForegroundColor Gray
}

Write-Host "`n✨ All done!" -ForegroundColor Green
