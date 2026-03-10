[CmdletBinding()]
param(
    [Alias("no-build")]
    [switch]$NoBuild
)

$loc = Get-Location
& "$PSScriptRoot/Clean-Repository.ps1"
Set-Location "$PSScriptRoot/.."
dotnet restore ./Ploch.Common.slnx

if (-not $NoBuild) {
    dotnet build ./Ploch.Common.slnx
}

Set-Location $loc
