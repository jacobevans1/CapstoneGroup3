# Define directories
$solutionDir = Get-Location
$testResultsDir = "TestResults"
$coverageDestDir = "CoverageResults"
$reportDir = "CoverageReport"

# Ensure necessary directories exist
foreach ($dir in @($testResultsDir, $coverageDestDir, $reportDir)) {
    if (!(Test-Path $dir)) {
        New-Item -ItemType Directory -Path $dir | Out-Null
    }
}

# Find all test projects dynamically
$testProjects = Get-ChildItem -Path $solutionDir -Recurse -Filter "*.csproj" | Where-Object { $_.FullName -match "Test" }

if ($testProjects.Count -eq 0) {
    Write-Host "No test projects found!" -ForegroundColor Red
    exit 1
}

# Run tests and collect coverage for all test projects
foreach ($testProject in $testProjects) {
    Write-Host "Running tests for: $($testProject.FullName)" -ForegroundColor Cyan
    dotnet test $testProject.FullName --collect:"XPlat Code Coverage" --results-directory $testResultsDir

    if ($LASTEXITCODE -ne 0) {
        Write-Host "Tests failed for: $($testProject.FullName)" -ForegroundColor Red
        exit 1
    }
}

# Find all generated coverage files
$coverageFiles = Get-ChildItem -Path $testResultsDir -Recurse -Filter "coverage.cobertura.xml"

if ($coverageFiles.Count -eq 0) {
    Write-Host "No coverage reports found!" -ForegroundColor Red
    exit 1
}

$coverageFilePaths = @()

foreach ($file in $coverageFiles) {
    # Ensure each report has a unique filename
    $uniqueFileName = "$(Split-Path -Leaf $(Split-Path -Parent $file.FullName))_coverage.cobertura.xml"
    $destPath = Join-Path -Path $coverageDestDir -ChildPath $uniqueFileName

    Write-Host "Moving coverage file: $($file.FullName) -> $destPath" -ForegroundColor Yellow
    Move-Item -Path $file.FullName -Destination $destPath -Force
    $coverageFilePaths += $destPath
}

# Merge all coverage reports
$coverageFilesArg = $coverageFilePaths -join ";"

Write-Host "Generating coverage report..." -ForegroundColor Green
reportgenerator -reports:$coverageFilesArg -targetdir:$reportDir -reporttypes:Html `
    -assemblyfilters:"-System.Windows.Forms.*;-Microsoft.*" `
      -classfilters:"-AspNetCoreGeneratedDocument.*;-*.GeneratedCode.*;-*.Designer.*;-TicketAppWeb.Migrations.*;-Program;-TicketAppDesktop.Views.LoginForm*;-TicketAppDesktop.Views.TicketAppHome*;-TicketAppDesktop.Views.TicketDetailForm*;-TicketAppDesktop.Properties.Resources*;-TicketAppDesktop.ApplicationConfiguration*"


if ($LASTEXITCODE -eq 0) {
    Write-Host "Merged coverage report generated at: $reportDir/index.html" -ForegroundColor Green
    Start-Process "$reportDir/index.html"
} else {
    Write-Host "Report generation failed!" -ForegroundColor Red
    exit 1
}
