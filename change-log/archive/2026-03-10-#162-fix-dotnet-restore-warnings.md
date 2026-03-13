# Fix dotnet restore warnings

## Changes

- **Removed redundant NuGet packages from `Common.WebUI`**: Packages already provided by the `Microsoft.AspNetCore.App` shared framework (`Microsoft.AspNetCore.Authorization`, `Microsoft.AspNetCore.Mvc.ViewFeatures`, and 13 `Microsoft.Extensions.*` packages) were removed to resolve NU1510 warnings.
- **Removed `Common.Net6.Tests` from solution**: The project targeted `net6.0`, which is out of support (NETSDK1138). Tests are already covered by `Common.Tests` and `Common.Net9.Tests`.
- **Removed `TestingSupport.XUnit2.Dependencies` from solution**: Only referenced by `Common.Net6.Tests`; no longer needed. This also resolves the NU1701 warning from `xunit.runner.visualstudio` incompatibility.
- **Suppressed NU1603 warning**: Added to `NoWarn` in `Directory.Build.props`. This is a benign transitive dependency resolution warning from `Castle.Core` depending on an exact version of `System.ComponentModel.TypeConverter`.

## Impact

- No breaking changes to public API.
- All existing tests continue to pass.
