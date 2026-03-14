# Ploch.TestingSupport.Dependencies.MetaPackages

> Repository-internal meta-package definition files for bundling common testing dependencies into a single NuGet reference.

## Overview

`Ploch.TestingSupport.Dependencies.MetaPackages` is not a source library — it contains no compilable C# code. Instead, it holds the NuGet `.nuspec` manifest files and supporting configuration (such as `xunit.runner.json`) that are used to build meta-packages for testing dependencies.

A meta-package is a NuGet package that carries no assembly of its own. Its sole purpose is to declare a curated set of package dependencies so that a test project can obtain all required packages with a single `<PackageReference>`. This eliminates the need for every test project to individually enumerate the same list of testing libraries and keep their versions in sync.

Currently the directory holds the manifest for the xUnit v3 dependency bundle. For the fully functional, buildable meta-package project see [Ploch.TestingSupport.XUnit3.Dependencies](testing-support-xunit3-dependencies.md).

## Contents

| File | Description |
|---|---|
| `Ploch.TestingSupport.XUnit3.Dependencies.nuspec` | NuGet package manifest declaring all xUnit v3 testing dependencies. |
| `xunit.runner.json` | xUnit v3 runner configuration file included as package content; copied to the test project output directory on build. |

## Related Libraries

- [Ploch.TestingSupport.XUnit3.Dependencies](testing-support-xunit3-dependencies.md) — The active, buildable meta-package project for xUnit v3 test dependencies
- [Ploch.TestingSupport.XUnit2.Dependencies](testing-support-xunit2-dependencies.md) — The equivalent meta-package for xUnit v2 (legacy)
