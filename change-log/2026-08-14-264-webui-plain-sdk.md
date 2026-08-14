# chore(webui): Switch Ploch.Common.WebUI to the plain Microsoft.NET.Sdk

- **Issue:** [#264](https://github.com/mrploch/ploch-common/issues/264)
- **Type:** Internal build change — no consumer-facing impact

## Summary

`Ploch.Common.WebUI` now uses `Microsoft.NET.Sdk` with an explicit
`<FrameworkReference Include="Microsoft.AspNetCore.App" />` instead of
`Microsoft.NET.Sdk.Web`. The project is a class library with no Razor files, so per
[Microsoft's guidance](https://learn.microsoft.com/aspnet/core/fundamentals/target-aspnetcore)
the plain SDK (not `Microsoft.NET.Sdk.Razor`, which is only for libraries containing
Razor files) is the correct choice. The now-redundant `IsPackable` and `OutputType`
overrides and the runnable-app `launchSettings.json` were removed.

## Consumer impact

None. The produced package was diffed against a pre-change baseline: contents and
nuspec (dependencies, `Microsoft.AspNetCore.App` framework reference, packed README)
are identical.
