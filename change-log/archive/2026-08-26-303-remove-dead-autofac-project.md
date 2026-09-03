# Removed the dead `Plocch.Common.DependencyInjection.Autofac` project

**Type:** housekeeping. **No package-consumer impact** — the package was never published
to nuget.org or GitHub Packages, so no project can depend on it. Documentation readers
*were* affected: the published docs site listed it as an available library (see below).

`src/Common.DependencyInjection.Autofac/` contained a single `.csproj` and **no source
files at all**. It was absent from every checked-in solution file
(`Ploch.Common.slnx`, `Ploch.Common.Endpoints.slnx`, `Ploch.Common.LocalDev.slnx`,
`Ploch.Common.WebApi.Endpoints.slnx`) and no project referenced it, and no checked-in
script or workflow builds it directly — so no build produced a package from it. (It could
still be compiled by pointing `dotnet build` at the `.csproj` by hand, though no build
script or workflow did so.) One piece of automation *did* touch it: `DocumentationSite/docfx.json`
globs `../src/**/*.csproj` from the **filesystem** rather than reading a solution, so DocFX
ran MSBuild over this project on every docs build and logged
`does not contain any documents`. Deleting it therefore also removes a standing DocFX
warning (see #290). Its project file also carried a typo'd prefix
(double `c`), which the package ID would have inherited had it ever shipped.

Removed, along with the documentation that advertised it:

- `src/Common.DependencyInjection.Autofac/` (project deleted)
- `docs/libraries/common-dependency-injection-autofac.md` (page deleted — it described an
  API that did not exist, flagged in its own text as "aspirational/planned")
- Entries removed from `docs/INDEX.md`, `docs/toc.yml`, `DocumentationSite/index.md`, and
  the "see also" links in `common-dependency-injection.md` and
  `common-dependency-injection-hosting.md`
- The stale `<File>` entry in `Ploch.Common.slnx` — that entry listed the **docs page**
  as a solution item, not the project; the `.csproj` itself was never in any solution

The published documentation site listed `Ploch.Common.DependencyInjection.Autofac` among
the libraries, so readers could reasonably have believed the package existed. That listing
is now gone.

Autofac support is not implemented. If it is wanted later, it should be added as a
correctly-named project that is registered in the solution from its first commit.

The shared `Autofac.Extensions.DependencyInjection` version pin in
`mrploch-development/dependencies/Common.Packages.props` is **left untouched** — it is
shared with other repositories.

Refs: #303
