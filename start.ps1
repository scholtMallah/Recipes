# PowerShell Startup Script for Recipes

Write-Host "Starting .NET backend on port 8080..."
Start-Process "dotnet" "run --urls=http://localhost:8080" -WorkingDirectory "./Recipes" -NoNewWindow