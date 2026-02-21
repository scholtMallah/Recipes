#!/bin/bash

# Gracefully stop both backend and frontend on Ctrl+C
cleanup() {
  echo ""
  echo "Stopping backend and frontend..."
  kill "$DOTNET_PID" "$NG_PID"
  wait "$DOTNET_PID" "$NG_PID" 2>/dev/null
  echo "Stopped."
  exit 0
}
trap cleanup SIGINT SIGTERM

echo "Starting .NET backend on port 8080..."
cd Recipes || exit 1
dotnet run --urls=http://localhost:8080 &
DOTNET_PID=$!
cd ..

echo ""
echo "backend starting..."
echo "You can access the app at: http://localhost:8080"
echo "Press Ctrl+C to stop both."

# Keep script alive to allow trap to work
wait "$DOTNET_PID" "$NG_PID"

