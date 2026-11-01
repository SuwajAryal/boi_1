param([string]$OutputFolder = 'dist/cat')
$ErrorActionPreference = 'Stop'
$projectRoot = $PSScriptRoot
$compiler = Join-Path $env:WINDIR 'Microsoft.NET/Framework64/v4.0.30319/csc.exe'
if (!(Test-Path -LiteralPath $compiler)) { throw 'Windows .NET Framework 4.x compiler is required.' }
$outputRoot = Join-Path $projectRoot $OutputFolder
New-Item -ItemType Directory -Force -Path (Join-Path $outputRoot 'assets') | Out-Null
& $compiler /nologo /target:winexe /optimize+ /platform:anycpu /out:"$outputRoot\boi_1.exe" /r:System.dll /r:System.Core.dll /r:System.Drawing.dll /r:System.Windows.Forms.dll /r:System.Xml.dll "$projectRoot\src\Core.cs" "$projectRoot\src\Native.cs" "$projectRoot\src\Pet.cs" "$projectRoot\src\Tools.cs"
if ($LASTEXITCODE -ne 0) { throw 'Build failed.' }
& $compiler /nologo /target:winexe /out:"$outputRoot\Open-boi-tools.exe" "$projectRoot\src\OpenTools.cs"
if ($LASTEXITCODE -ne 0) { throw 'Tools launcher build failed.' }
Copy-Item -LiteralPath "$projectRoot/assets/cat-reaction-loops.png" -Destination "$outputRoot/assets" -Force
Copy-Item -LiteralPath "$projectRoot/assets/cat-idle-loop-v2.png" -Destination "$outputRoot/assets/cat-idle-loop.png" -Force
Copy-Item -LiteralPath "$projectRoot/assets/cat-walk-loop-v2.png" -Destination "$outputRoot/assets/cat-walk-loop.png" -Force
Copy-Item -LiteralPath "$projectRoot/assets/cat-extra-expressions-v2.png" -Destination "$outputRoot/assets/cat-extra-expressions.png" -Force
Copy-Item -LiteralPath "$projectRoot/assets/cat-expressions-v2.png" -Destination "$outputRoot/assets/cat-expressions.png" -Force
Copy-Item -LiteralPath "$projectRoot/assets/cloud-panel.png","$projectRoot/assets/fa-solid-900.ttf" -Destination "$outputRoot/assets" -Force
New-Item -ItemType Directory -Force -Path "$outputRoot/assets/licenses" | Out-Null
Copy-Item -LiteralPath "$projectRoot/assets/licenses/font-awesome.txt" -Destination "$outputRoot/assets/licenses/font-awesome.txt" -Force
Copy-Item -LiteralPath "$projectRoot/CREDITS.md" -Destination "$outputRoot/CREDITS.md" -Force
Write-Output "Built $outputRoot/boi_1.exe. Keep its assets folder beside it."
