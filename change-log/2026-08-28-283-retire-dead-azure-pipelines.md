## Retire the dead Azure Pipelines definitions

The repository moved to GitHub Actions years ago. Three Azure DevOps pipeline definitions survived
that migration untouched. None of them can run successfully against the current repository, and
their continued presence misleads anyone reading the repository's CI setup into thinking Azure
DevOps is part of it. All three are removed.

### Removed

| File | Last meaningful commit | Why it is dead |
|---|---|---|
| `azure-pipelines.yml` | `988e168`, 2022-12-10 | Restores, builds and packs `./Ploch.Common.sln`, which does not exist — the repository uses `Ploch.Common.slnx`. It also pins .NET SDK 7.x (the repository targets `netstandard2.0`, `net8.0` and `net10.0`), uses the stale SonarCloud key `ploch_common` and organisation `ploch` rather than `mrploch_ploch-common`, and versions packages from a `PackageVersion` environment variable that Nerdbank.GitVersioning replaced. It pushed to NuGet.org on every trigger, contradicting the deliberate manual-dispatch-only flow in `.github/workflows/release.yml`. |
| `pipelines/original-sln-pipeline.yml` | `80a7701`, 2020-12-27 | A superseded earlier draft of the same pipeline, referenced by nothing. It uses `reportgenerator@4` and `PublishCodeCoverageResults@1`, both long deprecated. Removing it empties the `pipelines/` directory, which git therefore drops. |
| `src/Common/azure-pipeline.yml` | `bc48477`, 2020-12-19 | A three-byte file containing nothing but a UTF-8 byte-order mark. It has been empty since the initial commit. |

### Verification performed

No workflow, script, documentation page, or solution file references any of the three. The two
solution-item references to an `azure-pipelines.yml` (in `Ploch.Common.Endpoints.slnx` and
`Ploch.Common.WebApi.Endpoints.slnx`) point at `../ploch-data/azure-pipelines.yml` — a file in the
sibling `ploch-data` repository, not in this one — and are therefore unaffected.
`Ploch.Common.slnx` lists none of the removed files as solution items. The only other mention is
the historical `change-log/2026-08-22-282-...` entry, which records what was true at the time and
is left as-is.

### Not included

Whether an Azure DevOps project still points at this repository is an owner-side check that cannot
be answered from the git tree. If one exists it has been failing on every `master` push for years
and should be disabled at the Azure DevOps end too.

`my-schema.json` at the repository root is the Azure Pipelines VS Code JSON schema and is now
similarly unused, but removing it is outside this change's scope.

### Refs

- Closes #283
- Follows #282 (rename default branch from master to main), phase 1, which surfaced the file
