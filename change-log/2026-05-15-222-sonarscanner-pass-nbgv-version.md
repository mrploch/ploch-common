## CI: Pass NBGV version to SonarScanner so SonarCloud's "Previous version" new-code window works

### CI

- **`.github/workflows/build-dotnet.yml`** now captures NBGV's `NuGetPackageVersion` into a step output and forwards it to `dotnet sonarscanner begin` via the `/v:` parameter. Previously `sonar.projectVersion` was never supplied, which left SonarCloud with no version history to diff against. With the New Code definition set to **"Previous version"**, that caused the entire baseline (including 432 legacy code smells dating back to 2020 and 10 legacy security hotspots) to register as "new code" on every analysis, permanently failing the quality gate.
- With this change, each master commit produces a unique `NuGetPackageVersion` (e.g. `3.1.43-prerelease`) via NBGV's commit-height counter, and each stable release jumps to its tagged version (e.g. `3.2.0`). SonarCloud's "Previous version" comparison now diffs against the prior analysis, so the new-code window reflects what each PR or commit actually touched rather than the entire history.
- The release workflow (`release.yml`) does not invoke SonarScanner, so no change is required there.

### Follow-up

After this change ships, the existing 432 legacy smells and 10 hotspots will still appear in the new-code window on the *first* post-fix master analysis (because SonarCloud has no recorded prior version). A one-time admin step in SonarCloud will rebase the new-code window: switch **New Code → Specific analysis** to the first post-fix build, then switch back to **Previous version**. The 10 security hotspots will still need a manual triage in the Security Hotspots tab.

### Refs

- #222 (ci: pass NBGV version to SonarScanner)
