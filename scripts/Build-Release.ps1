param([Parameter(Mandatory)][string]$DalamudPath)
$ErrorActionPreference = 'Stop'
$repoRoot = Split-Path $PSScriptRoot -Parent
Push-Location $repoRoot
try {
    $references = (Resolve-Path -LiteralPath $DalamudPath).Path
    dotnet build Accountant/Accountant.csproj -c Release "-p:DalamudLibPath=$references" -p:RestoreLockedMode=true
    if ($LASTEXITCODE -ne 0) { throw 'Plugin build failed' }
    $dist = Join-Path $repoRoot 'dist'
    New-Item -ItemType Directory -Path $dist -Force | Out-Null
    Copy-Item -LiteralPath Accountant/bin/Release/Accountant/latest.zip -Destination (Join-Path $dist 'latest.zip') -Force
    Copy-Item -LiteralPath Accountant/bin/Release/Accountant.json -Destination (Join-Path $dist 'Accountant.json') -Force
    Compress-Archive -LiteralPath LICENSE,RELEASE_NOTES.txt -DestinationPath (Join-Path $dist 'latest.zip') -Update
    $hash = (Get-FileHash (Join-Path $dist 'latest.zip') -Algorithm SHA256).Hash.ToLowerInvariant()
    [IO.File]::WriteAllText((Join-Path $dist 'SHA256SUMS.txt'), "$hash  latest.zip`n", [Text.UTF8Encoding]::new($false))
} finally { Pop-Location }
