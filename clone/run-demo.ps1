# Starts the API and desktop app together for a demo.
# Run from the `clone/` directory: .\run-demo.ps1

$ErrorActionPreference = "Stop"
$root = $PSScriptRoot

Write-Host "Starting API on http://localhost:5299 ..."
Start-Process -FilePath "dotnet" -ArgumentList "run --urls http://localhost:5299" `
    -WorkingDirectory (Join-Path $root "src\Backend\Innovation.Api")

Write-Host "Waiting for API to come up..."
Start-Sleep -Seconds 5

Write-Host "Starting desktop app..."
Start-Process -FilePath "dotnet" -ArgumentList "run" `
    -WorkingDirectory (Join-Path $root "src\Desktop\Innovation.TotalWeight_PLC")

Write-Host "Both processes launched in separate windows. See docs/DEMO_SCRIPT.md for the walkthrough."
