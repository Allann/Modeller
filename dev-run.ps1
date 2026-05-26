# dev-run.ps1
# Runs Modeller CLI directly without installation (fastest for development)

param(
	[Parameter(ValueFromRemainingArguments=$true)]
	[string[]]$Arguments
)

# Find repository root
$scriptPath = $PSScriptRoot
if ([string]::IsNullOrEmpty($scriptPath)) {
	$scriptPath = Get-Location
}

$repoRoot = $scriptPath
while ($repoRoot -and !(Test-Path (Join-Path $repoRoot ".git"))) {
	$repoRoot = Split-Path $repoRoot -Parent
}

if (!$repoRoot) {
	Write-Host "Error: Could not find repository root" -ForegroundColor Red
	exit 1
}

$projectPath = Join-Path $repoRoot "src\Modeller.Cli\Modeller.Cli.csproj"

if (!(Test-Path $projectPath)) {
	Write-Host "Error: Project file not found at $projectPath" -ForegroundColor Red
	exit 1
}

# Run the CLI directly
Write-Host "Running: dotnet run --project $projectPath -- $($Arguments -join ' ')" -ForegroundColor Gray
dotnet run --project $projectPath --no-build -- @Arguments
