$loc = Get-Location
cd $PSScriptRoot/..
dotnet nuget push **/*.nupkg --api-key $env:NUGET_TOKEN --source https://api.nuget.org/v3/index.json
Set-Location $loc