# Ploch.Common.WebUI now ships as a NuGet package

**Issue:** [#250](https://github.com/mrploch/ploch-common/issues/250)

## New

- `Ploch.Common.WebUI` is now published as a NuGet package. The project always had packaging
  intent (a package README), but `Microsoft.NET.Sdk.Web` defaults `IsPackable` to `false`,
  which prevented the package from being produced (surfacing only as a pack warning on every
  build). `IsPackable` is now explicitly enabled, so the package (containing `AppPage`,
  `SelectListHelper`, and related Razor/MVC page utilities) ships alongside the other
  `Ploch.*` packages from this release onwards.

## Fixed

- The `NuGet.Build.Tasks.Pack` warning ("This project cannot be packaged because packaging
  has been disabled") emitted on every build of the solution is resolved.
