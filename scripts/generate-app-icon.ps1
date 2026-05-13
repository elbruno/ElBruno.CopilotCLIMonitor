param(
    [string]$OutputPath = ".\images\logo\app-icon.ico"
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

Add-Type -AssemblyName System.Drawing

$fullOutputPath = [System.IO.Path]::GetFullPath($OutputPath)
$dir = Split-Path -Parent $fullOutputPath
if (-not (Test-Path $dir)) { New-Item -ItemType Directory -Path $dir -Force | Out-Null }

$bmp = New-Object System.Drawing.Bitmap 256,256
$graphics = [System.Drawing.Graphics]::FromImage($bmp)
$graphics.Clear([System.Drawing.Color]::FromArgb(37,99,235))
$graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
$graphics.FillEllipse([System.Drawing.Brushes]::White, 40, 40, 176, 176)
$graphics.FillEllipse([System.Drawing.Brushes]::Red, 160, 40, 56, 56)
$font = New-Object System.Drawing.Font("Segoe UI", 96, [System.Drawing.FontStyle]::Bold, [System.Drawing.GraphicsUnit]::Pixel)
$graphics.DrawString("C", $font, [System.Drawing.Brushes]::DodgerBlue, 72, 84)

$icon = [System.Drawing.Icon]::FromHandle($bmp.GetHicon())
$fs = [System.IO.File]::Open($fullOutputPath, [System.IO.FileMode]::Create)
try { $icon.Save($fs) } finally { $fs.Dispose(); $icon.Dispose(); $graphics.Dispose(); $bmp.Dispose(); $font.Dispose() }

Write-Host "App icon generated at $fullOutputPath"
