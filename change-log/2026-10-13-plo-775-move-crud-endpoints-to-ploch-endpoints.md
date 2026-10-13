# Move the CRUD endpoints libraries to ploch-endpoints (PLO-775)

The generic CRUD Web API endpoint libraries have left ploch-common. They now live in their own
open-source repository, [mrploch/ploch-endpoints](https://github.com/mrploch/ploch-endpoints),
and will be published as `Ploch.Endpoints.*`.

## What was removed

- `src/Common.WebApi/`, which held seven projects: `Ploch.Common.WebApi`,
  `Ploch.Common.WebApi.Endpoints`, `...Endpoints.CrudEndpoints`,
  `...CrudEndpoints.MinimalApiEndpoints`, `...CrudEndpoints.FastEndpoints`, and their two test
  projects.
- `tests/Common.WebApi.Tests/`.
- `src/Common.Ardalis.Result/`. Its only consumer was the FastEndpoints project.
- The secondary solutions `Ploch.Common.WebApi.Endpoints.slnx` and `Ploch.Common.Endpoints.slnx`.
- The documentation pages `common-webapi.md` and `common-ardalis-result.md`, together with their
  TOC, index and DocFX entries.

## Impact on consumers

**None in practice.** None of these projects was ever published to nuget.org:

- `Ploch.Common.WebApi` was explicitly `IsPackable=false`.
- The other projects were not in `Ploch.Common.slnx`, so CI never packed them.

The API reference site no longer shows `Ploch.Common.WebApi`, `Ploch.Common.WebApi.Endpoints` or
`Ploch.Common.Ardalis.Result`. Their documentation moves to the new repository.
