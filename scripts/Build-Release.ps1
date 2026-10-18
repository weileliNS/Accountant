param([Parameter(Mandatory)][string]$DalamudPath)
$ErrorActionPreference = 'Stop'
$repoRoot = Split-Path $PSScriptRoot -Parent
Push-Location $repoRoot
try {
    $references = (Resolve-Path -LiteralPath $DalamudPath).Path
    dotnet run --project tests/Accountant.Migration.Tests -c Release "-p:DalamudLibPath=$references"
    if ($LASTEXITCODE -ne 0) { throw 'Configuration migration tests failed' }
    dotnet build Accountant/Accountant.csproj -c Release "-p:DalamudLibPath=$references" -p:RestoreLockedMode=true
    if ($LASTEXITCODE -ne 0) { throw 'Plugin build failed' }
    $dist = Join-Path $repoRoot 'dist'
    New-Item -ItemType Directory -Path $dist -Force | Out-Null
    $output = Join-Path $repoRoot 'Accountant/bin/Release'
    $packageFiles = @('AccountantCN.dll','AccountantCN.deps.json','AccountantCN.json','Accountant.GameData.dll','AddonWatcher.dll','OtterLoc.dll') |
        ForEach-Object { Join-Path $output $_ }
    $packageFiles += @((Join-Path $repoRoot 'LICENSE'), (Join-Path $repoRoot 'RELEASE_NOTES.txt'))
    Compress-Archive -LiteralPath $packageFiles -DestinationPath (Join-Path $dist 'latest.zip') -Force
    Copy-Item -LiteralPath (Join-Path $output 'AccountantCN.json') -Destination (Join-Path $dist 'Accountant.json') -Force
    $hash = (Get-FileHash (Join-Path $dist 'latest.zip') -Algorithm SHA256).Hash.ToLowerInvariant()
    [IO.File]::WriteAllText((Join-Path $dist 'SHA256SUMS.txt'), "$hash  latest.zip`n", [Text.UTF8Encoding]::new($false))
} finally { Pop-Location }
