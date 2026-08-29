@echo off
cd /d "%~dp0"

echo Serving dist at http://localhost:8765/
echo Press Ctrl+C to stop.
echo.

start "" http://localhost:8765/

python -m http.server 8765 --directory dist