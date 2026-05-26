# dev-install.ps1
# Installs Modeller CLI locally for development testing

param(
	[switch]$SkipBuild
)

Write-Host "Building and installing Modeller CLI locally..." -ForegroundColor Cyan

# Find repository root (where .git folder is)
$scriptPath = $PSScriptRoot
if ([string]::IsNullOrEmpty($scriptPath)) {
	$scriptPath = Get-Location
}

$repoRoot = $scriptPath
while ($repoRoot -and !(Test-Path (Join-Path $repoRoot ".git"))) {
	$repoRoot = Split-Path $repoRoot -Parent
}

if (!$repoRoot) {
	Write-Host "Error: Could not find repository root (.git folder)" -ForegroundColor Red
	exit 1
}

Write-Host "Repository root: $repoRoot" -ForegroundColor Gray

$projectPath = Join-Path $repoRoot "src\Modeller.Cli\Modeller.Cli.csproj"
$nupkgPath = Join-Path $repoRoot "nupkg"

if (!(Test-Path $projectPath)) {
	Write-Host "Error: Project file not found at $projectPath" -ForegroundColor Red
	exit 1
}

# Clean previous packages
if (Test-Path $nupkgPath) {
	Write-Host "Cleaning previous packages..." -ForegroundColor Yellow
	Remove-Item $nupkgPath -Recurse -Force
}

# Build solution first (unless skipped)
if (!$SkipBuild) {
	Write-Host "Building solution..." -ForegroundColor Yellow
	$slnPath = Join-Path $repoRoot "Modeller.sln"
	dotnet build $slnPath -c Release --nologo -v quiet

	if ($LASTEXITCODE -ne 0) {
		Write-Host "Build failed!" -ForegroundColor Red
		exit 1
	}
}

# Pack the tool
Write-Host "Packing Modeller.Cli..." -ForegroundColor Yellow
dotnet pack $projectPath -c Release -o $nupkgPath --no-build --nologo

if ($LASTEXITCODE -ne 0) {
	Write-Host "Pack failed!" -ForegroundColor Red
	exit 1
}

# Get the package file
$packageFile = Get-ChildItem $nupkgPath -Filter "Modeller.Cli.*.nupkg" | Select-Object -First 1

if (!$packageFile) {
	Write-Host "Error: Package file not found in $nupkgPath" -ForegroundColor Red
	exit 1
}

Write-Host "Package created: $($packageFile.Name)" -ForegroundColor Green

# Uninstall existing
Write-Host "Uninstalling previous version..." -ForegroundColor Yellow
dotnet tool uninstall -g Modeller.Cli 2>$null

# Install from local package
Write-Host "Installing from local package..." -ForegroundColor Yellow

# Try with --add-source first
dotnet tool install -g Modeller.Cli --add-source $nupkgPath --no-cache 2>&1 | Out-Null
$installSuccess = $LASTEXITCODE -eq 0

# If that fails due to package source mapping, try with --prerelease to use config
if (!$installSuccess) {
	Write-Host "Retrying with alternative method..." -ForegroundColor Yellow

	# Create a temporary NuGet.config that allows the local source
	$tempConfig = @"
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
	<add key="local-dev" value="$nupkgPath" />
	<add key="nuget.org" value="https://api.nuget.org/v3/index.json" />
  </packageSources>
  <packageSourceMapping>
	<packageSource key="local-dev">
	  <package pattern="Modeller.*" />
	</packageSource>
	<packageSource key="nuget.org">
	  <package pattern="*" />
	</packageSource>
  </packageSourceMapping>
</configuration>
"@

	$tempConfigPath = Join-Path $env:TEMP "modeller-dev-nuget.config"
	$tempConfig | Out-File -FilePath $tempConfigPath -Encoding UTF8

	dotnet tool install -g Modeller.Cli --configfile $tempConfigPath --no-cache
	$installSuccess = $LASTEXITCODE -eq 0

	# Clean up temp config
	Remove-Item $tempConfigPath -ErrorAction SilentlyContinue
}

if ($installSuccess) {
	Write-Host "`n✓ Success! Modeller CLI installed." -ForegroundColor Green
	Write-Host "`nVerifying installation:" -ForegroundColor Cyan
	modeller --version
	Write-Host "`nYou can now run 'modeller' from anywhere." -ForegroundColor Gray
} else {
	Write-Host "`nInstallation failed!" -ForegroundColor Red
	Write-Host "Try clearing NuGet cache: dotnet nuget locals all --clear" -ForegroundColor Yellow
	Write-Host "Or use dev-run.ps1 to run without installation" -ForegroundColor Yellow
	exit 1
}
