# Comprehensive Issue Assignment Script
# Applies squad labels, priority labels, and wave labels to all 63 GitHub issues
# Requires: GitHub CLI (gh) with authentication or $env:GITHUB_TOKEN set

param(
  [string]$Token = $env:GITHUB_TOKEN,
  [bool]$DryRun = $false
)

$Owner = "elbruno"
$Repo = "ElBruno.CopilotCLIMonitor"

# All 63 issues with assignments
$assignments = @(
  # WAVE 1 - Infrastructure (7 issues)
  @{num=5;  name='Configuration File Format'; assigned='dolph'; priority='high'; wave='wave-1'},
  @{num=6;  name='Environment Variables'; assigned='dolph'; priority='high'; wave='wave-1'},
  @{num=8;  name='Configuration Validation'; assigned='dolph'; priority='high'; wave='wave-1'},
  @{num=18; name='IPC Optimization'; assigned='dolph'; priority='high'; wave='wave-1'},
  @{num=49; name='Default Configuration'; assigned='dolph'; priority='high'; wave='wave-1'},
  @{num=29; name='Release Branch Strategy'; assigned='sish'; priority='high'; wave='wave-1'},
  @{num=64; name='Semantic Versioning Strategy'; assigned='sish'; priority='high'; wave='wave-1'},
  
  # WAVE 2 - Core Features (8 issues)
  @{num=2;  name='Sound Alerts'; assigned='chevy'; priority='high'; wave='wave-2'},
  @{num=3;  name='Auto-start on Windows Boot'; assigned='dolph'; priority='high'; wave='wave-2'},
  @{num=15; name='Diagnostic Mode'; assigned='dolph'; priority='high'; wave='wave-2'},
  @{num=30; name='Quiet Hours Configuration'; assigned='dolph'; priority='high'; wave='wave-2'},
  @{num=41; name='Settings/Preferences UI'; assigned='chevy'; priority='high'; wave='wave-2'},
  @{num=42; name='System Tray Context Menu'; assigned='chevy'; priority='high'; wave='wave-2'},
  @{num=43; name='Toast Notification UI'; assigned='chevy'; priority='high'; wave='wave-2'},
  @{num=44; name='Event History/Dashboard'; assigned='chevy'; priority='high'; wave='wave-2'},
  
  # WAVE 3 - Testing & Quality (6 issues)
  @{num=19; name='Code Quality Metrics'; assigned='river'; priority='high'; wave='wave-3'},
  @{num=20; name='Linting Rules'; assigned='river'; priority='high'; wave='wave-3'},
  @{num=25; name='UI/UX Tests'; assigned='river'; priority='high'; wave='wave-3'},
  @{num=26; name='Code Coverage Reports'; assigned='river'; priority='high'; wave='wave-3'},
  @{num=61; name='Unit Tests for WPF Components'; assigned='river'; priority='high'; wave='wave-3'},
  @{num=63; name='Hook Integration Tests'; assigned='river'; priority='high'; wave='wave-3'},
  
  # WAVE 4 - Docs, Security & Polish (8 issues)
  @{num=11; name='API Documentation'; assigned='dolph'; priority='high'; wave='wave-4'},
  @{num=23; name='Encrypted Storage'; assigned='dolph'; priority='high'; wave='wave-4'},
  @{num=52; name='User Manual'; assigned='chevy'; priority='high'; wave='wave-4'},
  @{num=53; name='Developer Guide'; assigned='dolph'; priority='high'; wave='wave-4'},
  @{num=54; name='Hook Configuration Guide'; assigned='dolph'; priority='high'; wave='wave-4'},
  @{num=57; name='Security Scanning'; assigned='river'; priority='high'; wave='wave-4'},
  @{num=59; name='Input Validation'; assigned='dolph'; priority='high'; wave='wave-4'},
  @{num=60; name='Dependency Vulnerabilities'; assigned='river'; priority='high'; wave='wave-4'},
  
  # WAVE 5 - Medium Priority (28 issues)
  @{num=4;  name='Multi-Repository Support'; assigned='dolph'; priority='medium'; wave='wave-5'},
  @{num=12; name='FAQ/Troubleshooting'; assigned='chevy'; priority='medium'; wave='wave-5'},
  @{num=13; name='Windows Defender/Security Notes'; assigned='chevy'; priority='medium'; wave='wave-5'},
  @{num=14; name='Log File Rotation'; assigned='dolph'; priority='medium'; wave='wave-5'},
  @{num=16; name='Memory Optimization'; assigned='dolph'; priority='medium'; wave='wave-5'},
  @{num=17; name='Startup Time'; assigned='dolph'; priority='medium'; wave='wave-5'},
  @{num=21; name='Performance Testing'; assigned='river'; priority='medium'; wave='wave-5'},
  @{num=22; name='Accessibility Testing'; assigned='river'; priority='medium'; wave='wave-5'},
  @{num=24; name='Privacy Policy'; assigned='chevy'; priority='medium'; wave='wave-5'},
  @{num=27; name='Release Notes Template'; assigned='sish'; priority='medium'; wave='wave-5'},
  @{num=28; name='Version Auto-Increment'; assigned='sish'; priority='medium'; wave='wave-5'},
  @{num=31; name='Marketing Graphics'; assigned='chevy'; priority='medium'; wave='wave-5'},
  @{num=34; name='Video Tutorials'; assigned='chevy'; priority='medium'; wave='wave-5'},
  @{num=35; name='Resource Strings'; assigned='dolph'; priority='medium'; wave='wave-5'},
  @{num=36; name='Multi-Language Support'; assigned='dolph'; priority='medium'; wave='wave-5'},
  @{num=37; name='RTL Language Support'; assigned='dolph'; priority='medium'; wave='wave-5'},
  @{num=38; name='Event Tracing'; assigned='dolph'; priority='medium'; wave='wave-5'},
  @{num=39; name='Telemetry (Optional)'; assigned='river'; priority='medium'; wave='wave-5'},
  @{num=40; name='Hotfix Process'; assigned='sish'; priority='medium'; wave='wave-5'},
  @{num=45; name='Application Icon'; assigned='chevy'; priority='medium'; wave='wave-5'},
  @{num=46; name='Logo Variants'; assigned='chevy'; priority='medium'; wave='wave-5'},
  @{num=47; name='Notification Icon'; assigned='chevy'; priority='medium'; wave='wave-5'},
  @{num=48; name='Product Screenshots'; assigned='chevy'; priority='medium'; wave='wave-5'},
  @{num=50; name='Standalone Installer'; assigned='sish'; priority='medium'; wave='wave-5'},
  @{num=51; name='Automatic Update Mechanism'; assigned='sish'; priority='medium'; wave='wave-5'},
  @{num=55; name='Structured Logging'; assigned='dolph'; priority='medium'; wave='wave-5'},
  @{num=56; name='Notification Latency'; assigned='dolph'; priority='medium'; wave='wave-5'},
  @{num=62; name='Integration Tests'; assigned='river'; priority='medium'; wave='wave-5'},
  
  # WAVE 6 - Low Priority (6 issues)
  @{num=7;  name='Multi-User Support'; assigned='dolph'; priority='low'; wave='wave-6'},
  @{num=9;  name='Windows Store/Microsoft Store'; assigned='sish'; priority='low'; wave='wave-6'},
  @{num=10; name='WinGet Package'; assigned='sish'; priority='low'; wave='wave-6'},
  @{num=32; name='Chocolatey Package'; assigned='sish'; priority='low'; wave='wave-6'},
  @{num=33; name='Portable/ZIP Distribution'; assigned='sish'; priority='low'; wave='wave-6'},
  @{num=58; name='Authentication & Authorization'; assigned='dolph'; priority='low'; wave='wave-6'}
)

function Apply-Labels {
  param(
    [int]$IssueNum,
    [string]$AssignedTeam,
    [string]$Priority,
    [string]$Wave
  )
  
  $labels = @("squad", "squad:$AssignedTeam", "priority:$Priority", $Wave)
  $labelStr = $labels -join ','
  
  Write-Host "Issue #$IssueNum ($labelStr)" -ForegroundColor Cyan
  
  if ($DryRun) {
    Write-Host "  [DRY RUN] Would add labels: $labelStr" -ForegroundColor Yellow
    return
  }
  
  try {
    if ($Token) {
      # Use REST API directly
      $uri = "https://api.github.com/repos/$Owner/$Repo/issues/$IssueNum/labels"
      $headers = @{
        "Authorization" = "token $Token"
        "Accept" = "application/vnd.github.v3+json"
        "Content-Type" = "application/json"
      }
      $body = @{ labels = $labels } | ConvertTo-Json
      
      Invoke-RestMethod -Uri $uri -Headers $headers -Method POST -Body $body | Out-Null
      Write-Host "  ✓ Labels added" -ForegroundColor Green
    } else {
      # Try GitHub CLI
      & gh issue edit $IssueNum --repo "$Owner/$Repo" --add-label $labelStr 2>$null
      if ($LASTEXITCODE -eq 0) {
        Write-Host "  ✓ Labels added (via gh)" -ForegroundColor Green
      } else {
        Write-Host "  ⚠ Failed (no auth)" -ForegroundColor Yellow
      }
    }
  } catch {
    Write-Host "  ✗ Error: $_" -ForegroundColor Red
  }
}

Write-Host "=== APPLYING ISSUE LABELS ===" -ForegroundColor Magenta
Write-Host "Owner: $Owner"
Write-Host "Repo: $Repo"
Write-Host "Total issues: $($assignments.Count)"
Write-Host "DryRun: $DryRun"
Write-Host ""

$successCount = 0
foreach ($assignment in $assignments) {
  Apply-Labels -IssueNum $assignment.num -AssignedTeam $assignment.assigned -Priority $assignment.priority -Wave $assignment.wave
  $successCount++
}

Write-Host ""
Write-Host "=== SUMMARY ===" -ForegroundColor Magenta
Write-Host "Processed: $successCount issues"
Write-Host "Review at: https://github.com/$Owner/$Repo/issues"
