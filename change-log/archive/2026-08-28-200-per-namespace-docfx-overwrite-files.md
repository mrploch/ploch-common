# #200 — Per-namespace DocFX overwrite files for the priority namespaces

**Type:** documentation
**Breaking changes:** none

## Summary

The published API reference now opens each priority namespace with an authored **Overview** section explaining
*why* the namespace exists and when to reach for it, rather than dropping the reader straight into the
auto-generated type table.

## Details

Added DocFX overwrite files under `DocumentationSite/namespaces/` for thirteen namespaces:

- `Ploch.Common`
- `Ploch.Common.Collections`
- `Ploch.Common.ArgumentChecking`
- `Ploch.Common.Reflection`
- `Ploch.Common.Serialization`
- `Ploch.Common.DependencyInjection`
- `Ploch.Common.IO`
- `Ploch.Common.Randomizers`
- `Ploch.Common.Extensions.Configuration`
- `Ploch.Common.Apps.Model`
- `Ploch.TestingSupport`
- `Ploch.TestingSupport.XUnit3`
- `Ploch.TestingSupport.FluentAssertions`

Each file cross-links to the corresponding authored page under `docs/libraries/`.

The pre-existing `DocumentationSite/namespaces/Ploch.Common.md` stub declared
`uid: Ploch.Common.Data.Model`, a namespace that does not exist in this repository. DocFX silently discards an
overwrite whose `uid` matches nothing, so the stub had never contributed anything to the generated site. Its
content has been replaced with a genuine `Ploch.Common` overview.

No changes to shipped library code — packages are unaffected.
