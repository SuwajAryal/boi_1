$ErrorActionPreference = 'Stop'
& "$PSScriptRoot\build.ps1"
$bundleRoot = Join-Path $PSScriptRoot 'dist/bunny'
@'
boi_1 - animated bunny Windows test build

Extract the entire ZIP to a folder, then double-click boi_1.exe.
Keep the assets folder alongside the executable.

Click the bunny: timer and tasks. Right-click: quick controls.
Ctrl+Alt+B or Open-boi-tools.exe: open tools directly.
Bunny & settings: expression previews, sound, reduced motion, pause roaming.
Quit from tools or the tray. No startup entry is installed.

This is an unsigned local test build. .NET Framework 4.8 on Windows is required.
19 core checks and native-form smoke checks pass. Live desktop integration
checks remain incomplete. Please try it with fullscreen apps minimized or closed.
Roaming is confined to the rightmost 15% above the system tray.
Quit older boi_1 versions from their tray before running this build.

Tasks and settings stay under %LOCALAPPDATA%/boi_1/. A timer cannot alert while
the app is closed. No account, telemetry or background network calls.

Source, limitations and credits: https://github.com/SuwajAryal/boi_1
'@ | Set-Content -LiteralPath "$bundleRoot\START-HERE.txt" -Encoding UTF8
$packageFiles = @("$bundleRoot\boi_1.exe", "$bundleRoot\Open-boi-tools.exe", "$bundleRoot\assets", "$bundleRoot\CREDITS.md", "$bundleRoot\START-HERE.txt")
Compress-Archive -LiteralPath $packageFiles -DestinationPath "$PSScriptRoot\dist\boi_1-bunny-test.zip" -Force
Get-FileHash -LiteralPath "$PSScriptRoot\dist\boi_1-bunny-test.zip" -Algorithm SHA256 | Format-List
