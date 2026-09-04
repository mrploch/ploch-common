# Manual NuGet.org publish escape hatch. The supported path is the release.yml workflow;
# this exists for the case where that pipeline cannot be used.
#
# The ./tests/ exclusion mirrors release.yml and build-dotnet.yml so all three agree on what
# is publishable. Directory.Build.props already keeps the tests/ tree unpackable (issue #279),
# so this filter is a second line of defence rather than the only one.
$loc = Get-Location
Set-Location $PSScriptRoot/..
try {
    $packages = Get-ChildItem -Recurse -Filter *.nupkg |
        Where-Object { $_.FullName -notmatch '[\\/]tests[\\/]' }

    if (-not $packages) {
        Write-Error 'No .nupkg files were found. Build the solution in Release first.'
        return
    }

    foreach ($package in $packages) {
        Write-Information "Publishing $($package.FullName)" -InformationAction Continue
        dotnet nuget push $package.FullName --api-key $env:NUGET_TOKEN --source https://api.nuget.org/v3/index.json --skip-duplicate
    }
}
finally {
    Set-Location $loc
}
