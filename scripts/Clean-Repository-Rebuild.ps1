[CmdletBinding()]
param(
    [Alias("no-build")]
    [switch]$NoBuild
)

& "$PSScriptRoot/Clean-Repository.ps1"
Set-Location "$PSScriptRoot/.."
dotnet restore ./Ploch.Common.slnx

if (-not $NoBuild) {
    dotnet build ./Ploch.Common.slnx
}
