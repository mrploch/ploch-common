# Removed the dead `Plocch.Common.DependencyInjection.Autofac` project

**Type:** housekeeping. **No consumer impact** — the package was never published to
nuget.org or GitHub Packages, so nothing can depend on it.

`src/Common.DependencyInjection.Autofac/` contained a single `.csproj` and **no source
files at all**. It was never listed in `Ploch.Common.slnx` and nothing referenced it, so it
was never compiled by any build — local or CI — and produced no package. Its project file
also carried a typo'd prefix (`Plocch`, double `c`), which the package ID would have
inherited had it ever shipped.

Removed, along with the documentation that advertised it:

- `src/Common.DependencyInjection.Autofac/` (project deleted)
- `docs/libraries/common-dependency-injection-autofac.md` (page deleted — it described an
  API that did not exist, flagged in its own text as "aspirational/planned")
- Entries removed from `docs/INDEX.md`, `docs/toc.yml`, `DocumentationSite/index.md`, and
  the "see also" links in `common-dependency-injection.md` and
  `common-dependency-injection-hosting.md`
- The stale `<File>` entry in `Ploch.Common.slnx`

The published documentation site listed `Ploch.Common.DependencyInjection.Autofac` among
the libraries, so readers could reasonably have believed the package existed. That listing
is now gone.

Autofac support is not implemented. If it is wanted later, it should be added as a
correctly-named project that is registered in the solution from its first commit.

The shared `Autofac.Extensions.DependencyInjection` version pin in
`mrploch-development/dependencies/Common.Packages.props` is **left untouched** — it is
shared with other repositories.

Refs: #303
