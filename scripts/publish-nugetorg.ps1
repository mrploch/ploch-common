# Manual NuGet.org publish escape hatch. The supported path is the release.yml workflow;
# this exists for the case where that pipeline cannot be used.
#
# Package selection mirrors release.yml and build-dotnet.yml so all three publishers agree
# on what is publishable. Directory.Build.props already keeps the tests/ tree unpackable
# (issue #279), so this filter is a second line of defence rather than the only one.
#
# The exclusion is anchored to the repository-root tests/ directory and includes the
# trailing separator, so it matches the workflows' './tests/*' exactly: a project under
# src/tests/ is still published (as the workflows would), and a sibling directory such as
# tests-integration/ is not excluded by accident.
[CmdletBinding()]
param()

$ErrorActionPreference = 'Stop'

$repoRoot = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
$testsPrefix = (Join-Path $repoRoot 'tests') + [System.IO.Path]::DirectorySeparatorChar

# Pushes one package set and throws on the first failure. dotnet is a native command, so
# its outcome must be read from $LASTEXITCODE - a non-zero exit does not raise a
# PowerShell error, and without this check a rejected package would be followed by a
# successful-looking script exit.
function Publish-PackageSet {
    param(
        [Parameter(Mandatory)][System.IO.FileInfo[]] $Packages,
        [Parameter(Mandatory)][string] $Description
    )

    foreach ($package in $Packages) {
        Write-Information "Publishing $Description $($package.FullName)" -InformationAction Continue
        dotnet nuget push $package.FullName --api-key $env:NUGET_TOKEN --source https://api.nuget.org/v3/index.json --skip-duplicate
        if ($LASTEXITCODE -ne 0) {
            throw "dotnet nuget push failed with exit code $LASTEXITCODE for $($package.FullName)."
        }
    }
}

function Get-PublishablePackage {
    param([Parameter(Mandatory)][string] $Extension)

    Get-ChildItem -Path $repoRoot -Recurse -Filter "*.$Extension" -File |
        Where-Object { -not $_.FullName.StartsWith($testsPrefix, [System.StringComparison]::OrdinalIgnoreCase) }
}

Push-Location $repoRoot
try {
    $packages = @(Get-PublishablePackage -Extension 'nupkg')
    if ($packages.Count -eq 0) {
        throw 'No .nupkg files were found. Build the solution in Release configuration first.'
    }

    Publish-PackageSet -Packages $packages -Description 'package'

    # Symbol packages are optional - a build without SourceLink symbols produces none - but
    # a symbol push that is attempted and fails is still a failure, matching release.yml.
    $symbols = @(Get-PublishablePackage -Extension 'snupkg')
    if ($symbols.Count -gt 0) {
        Publish-PackageSet -Packages $symbols -Description 'symbols'
    }
}
finally {
    Pop-Location
}
