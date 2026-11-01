$ErrorActionPreference = 'Stop'
& "$PSScriptRoot\build.ps1"
@'
boi_1 - first Windows test build

Extract the entire ZIP to a folder, then double-click boi_1.exe.
Keep the assets folder alongside the executable.

Click the cat: timer and tasks. Right-click: quick controls.
Ctrl+Alt+B or Open-boi-tools.exe: open tools directly.
Cat & settings: expression previews, sound, reduced motion, pause roaming.
Quit from tools or the tray. No startup entry is installed.

This is an unsigned local test build. .NET Framework 4.8 on Windows is required.
13 core checks and native-form smoke checks pass. Live desktop interaction
verification remains incomplete because a fullscreen app covered the testing
window. Please try it with fullscreen apps minimized or closed.

Tasks and settings stay under %LOCALAPPDATA%/boi_1/. A timer cannot alert while
the app is closed. No account, telemetry or background network calls.

Source, limitations and credits: https://github.com/SuwajAryal/boi_1
'@ | Set-Content -LiteralPath "$PSScriptRoot\dist\START-HERE.txt" -Encoding UTF8
$packageFiles = @("$PSScriptRoot\dist\boi_1.exe", "$PSScriptRoot\dist\Open-boi-tools.exe", "$PSScriptRoot\dist\assets", "$PSScriptRoot\dist\CREDITS.md", "$PSScriptRoot\dist\START-HERE.txt")
Compress-Archive -LiteralPath $packageFiles -DestinationPath "$PSScriptRoot\dist\boi_1-windows-test.zip" -Force
Get-FileHash -LiteralPath "$PSScriptRoot\dist\boi_1-windows-test.zip" -Algorithm SHA256 | Format-List
