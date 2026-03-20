# =====================================================================
# Build script: Data Split Pro - Portable + MSI Installer
# Yêu cầu: .NET SDK, WiX Toolset v4+ (dotnet tool install --global wix)
# Cách dùng:  .\build-installer.ps1            (tự đọc version từ csproj)
#             .\build-installer.ps1 -Version 1.6.0
# Kết quả:   dist\DataSplitPro-vX.Y-Portable.exe
#            dist\DataSplitPro-vX.Y-Setup.msi
# =====================================================================
param([string]$Version)

$ErrorActionPreference = 'Stop'
$projectDir = $PSScriptRoot
$csproj = Join-Path $projectDir 'DataSplitPro.csproj'
$distDir = Join-Path $projectDir 'dist'
$publishDir = Join-Path $distDir 'publish'

# Read version from csproj when not provided
if (-not $Version) {
    $match = Select-String -Path $csproj -Pattern '<FileVersion>([\d\.]+)</FileVersion>'
    $Version = $match.Matches[0].Groups[1].Value
}
$shortVersion = ($Version -split '\.')[0..1] -join '.'
Write-Host "==> Building Data Split Pro v$shortVersion (MSI version $Version)" -ForegroundColor Cyan

# 1. Publish self-contained single-file (no .NET runtime required on target machine)
Write-Host '==> Publishing self-contained single-file exe...' -ForegroundColor Cyan
dotnet publish $csproj -c Release -r win-x64 --self-contained true `
    -p:PublishSingleFile=true `
    -p:EnableCompressionInSingleFile=true `
    -p:IncludeNativeLibrariesForSelfExtract=true `
    -p:DebugType=none `
    -o $publishDir
if ($LASTEXITCODE -ne 0) { throw 'dotnet publish failed' }

# Ensure the app icon sits next to the exe (single-file publish can skip it on incremental builds)
Copy-Item (Join-Path $projectDir 'hasoftware.ico') $publishDir -Force

# 2. Copy portable exe to dist root with versioned name
$portableName = "DataSplitPro-v$shortVersion-Portable.exe"
Copy-Item (Join-Path $publishDir 'DataSplitPro.exe') (Join-Path $distDir $portableName) -Force
Write-Host "==> Portable: dist\$portableName" -ForegroundColor Green

# 3. Build MSI with WiX
Write-Host '==> Building MSI installer...' -ForegroundColor Cyan
$msiName = "DataSplitPro-v$shortVersion-Setup.msi"
$msiVersion = ($Version -split '\.')[0..2] -join '.'  # MSI needs 3-part version
wix build (Join-Path $projectDir 'installer\Product.wxs') `
    -d "ProductVersion=$msiVersion" `
    -d "PublishDir=$publishDir" `
    -d "ProjectDir=$projectDir\" `
    -arch x64 `
    -o (Join-Path $distDir $msiName)
if ($LASTEXITCODE -ne 0) { throw 'wix build failed' }
Write-Host "==> MSI: dist\$msiName" -ForegroundColor Green

Write-Host '==> DONE. Output files:' -ForegroundColor Cyan
Get-ChildItem $distDir -File | Select-Object Name, @{N = 'SizeMB'; E = { [Math]::Round($_.Length / 1MB, 1) } }
