param([Parameter(Mandatory)][string]$Destination)
$ErrorActionPreference = 'Stop'
# CN stable 15.0.3.8, game build 2026.10.06.0000.0000. Deliberately pinned.
$distributionCommit = '51baf663aeb9f93cb0bfdddeb0956365c8ee4dfe'
$url = "https://raw.githubusercontent.com/ottercorp/dalamud-distrib/$distributionCommit/latest.7z"
New-Item -ItemType Directory -Path $Destination -Force | Out-Null
$archive = Join-Path $Destination 'dalamud.7z'
Invoke-WebRequest $url -OutFile $archive
if ((Get-FileHash $archive -Algorithm SHA256).Hash -ne '5C8E7F4516B7E1913FAAB13534C9EF4291DA635E433EBBDE631DFFEF4CC60D40') {
    throw 'CN reference archive checksum mismatch'
}
& 7z x $archive "-o$Destination" -y | Out-Null
if ($LASTEXITCODE -ne 0) { throw 'Failed extracting CN Dalamud' }
if (!(Test-Path (Join-Path $Destination 'Dalamud.dll'))) { throw 'Dalamud.dll missing from distribution' }
