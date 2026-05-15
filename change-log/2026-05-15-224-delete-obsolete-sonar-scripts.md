## Chore: Delete obsolete local SonarScanner helper scripts

### Removed

- `scripts/run-sonar-build-test.ps1`
- `scripts/build-test-sonar.ps1`

Both scripts referenced `Ploch.Common.sln`, which has been replaced by `Ploch.Common.slnx`. Neither script could run as-is — `dotnet restore` failed before SonarScanner was even invoked. Both also lacked the `/v:` (`sonar.projectVersion`) argument that #222 added to the GitHub Actions workflow, which meant any local analysis they did manage to upload would have polluted SonarCloud's `sonar.projectVersion` history with empty values.

The decision (recorded on issue #224) is to **delete** rather than fix: the GitHub Actions workflow (`.github/workflows/build-dotnet.yml`) is the canonical SonarCloud entry point. Manual local runs against the production project are discouraged because they affect shared SonarCloud state (issue creation dates, new-code window, quality gate history).

### Solution file updates

Removed the corresponding `<File Path="run-sonar-build-test.ps1" />` solution-items entries from:

- `Ploch.Common.slnx`
- `Ploch.Common.Endpoints.slnx`
- `Ploch.Common.LocalDev.slnx`
- `Ploch.Common.WebApi.Endpoints.slnx`

### Refs

- #224 (chore: delete obsolete sonar helper scripts)
- Follow-up to #222 / #223 (CI SonarScanner version fix)
