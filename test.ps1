$ErrorActionPreference = 'Stop'
$compiler = Join-Path $env:WINDIR 'Microsoft.NET/Framework64/v4.0.30319/csc.exe'
New-Item -ItemType Directory -Force -Path "$PSScriptRoot\dist" | Out-Null
& $compiler /nologo /target:exe /out:"$PSScriptRoot\dist\CoreTests.exe" /r:System.dll /r:System.Core.dll /r:System.Xml.dll "$PSScriptRoot\src\Core.cs" "$PSScriptRoot\tests\CoreTests.cs"
if ($LASTEXITCODE -ne 0) { throw 'Test compile failed.' }
& "$PSScriptRoot\dist\CoreTests.exe"
if ($LASTEXITCODE -ne 0) { throw 'Tests failed.' }
