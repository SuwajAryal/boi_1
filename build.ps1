$ErrorActionPreference = 'Stop'
$projectRoot = $PSScriptRoot
$compiler = Join-Path $env:WINDIR 'Microsoft.NET/Framework64/v4.0.30319/csc.exe'
if (!(Test-Path -LiteralPath $compiler)) { throw 'Windows .NET Framework 4.x compiler is required.' }
New-Item -ItemType Directory -Force -Path (Join-Path $projectRoot 'dist/assets') | Out-Null
& $compiler /nologo /target:winexe /optimize+ /platform:anycpu /out:"$projectRoot\dist\boi_1.exe" /r:System.dll /r:System.Core.dll /r:System.Drawing.dll /r:System.Windows.Forms.dll /r:System.Xml.dll "$projectRoot\src\Core.cs" "$projectRoot\src\Native.cs" "$projectRoot\src\Pet.cs" "$projectRoot\src\Tools.cs"
if ($LASTEXITCODE -ne 0) { throw 'Build failed.' }
& $compiler /nologo /target:winexe /out:"$projectRoot\dist\Open-boi-tools.exe" "$projectRoot\src\OpenTools.cs"
if ($LASTEXITCODE -ne 0) { throw 'Tools launcher build failed.' }
Copy-Item -LiteralPath "$projectRoot/assets/cat-expressions-v2.png" -Destination "$projectRoot/dist/assets/cat-expressions.png" -Force
Copy-Item -LiteralPath "$projectRoot/assets/cat-walk.png" -Destination "$projectRoot/dist/assets/cat-walk.png" -Force
Copy-Item -LiteralPath "$projectRoot/CREDITS.md" -Destination "$projectRoot/dist/CREDITS.md" -Force
Write-Output 'Built dist/boi_1.exe. Keep the assets folder beside the executable.'
