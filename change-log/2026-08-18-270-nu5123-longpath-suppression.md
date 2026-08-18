# chore(serialization): Suppress NU5123 long-path pack warnings for the ExtensionsDependencyInjection packages

- **Issue:** [#270](https://github.com/mrploch/ploch-common/issues/270)
- **Type:** Internal build change — no consumer-facing impact

## Summary

NuGet pack emitted `NU5123` ("path, name, or both are too long") for the
`lib/netstandard2.0` `.dll` and `.xml` of
`Ploch.Common.Serialization.SystemTextJson.ExtensionsDependencyInjection` and
`Ploch.Common.Serialization.NewtonsoftJson.ExtensionsDependencyInjection`. The warning
is now suppressed via a targeted, **PR-build-only** `<NoWarn>` in those two projects
(`Condition="'$(GITHUB_EVENT_NAME)' == 'pull_request'"`), with the rationale documented
in each project file. Master, release, and local packs keep `NU5123` live, so a genuine
overrun of shipped artefacts would still surface as a warning.

## Analysis

`NU5123` fires when the installed path `<package_id>/<version>/<file_path_in_package>`
reaches 200 characters. Both package ids are 71 characters and the dll/xml file names
are 75, giving:

| Version shape | Version length | Installed path | NU5123? |
|---|---|---|---|
| Stable release (`4.0.0`) | 5 | 172 | No |
| Master prerelease (`4.0.16-prerelease.gXXXXXXXXXX`) | 29 | 196 | No |
| PR build (`4.0.17-pr.268.chore-264-webui-plai.gXXXXXXXXXX`) | 46 | 213 | **Yes** |

Only PR-validation builds cross the threshold, because Nerdbank.GitVersioning embeds
the branch name in the version for non-public-release refs. Those packages are
ephemeral test artefacts published to GitHub Packages only — the packages shipped to
consumers (master prereleases and stable releases) never exceed the limit. Renaming
the packages to shorten paths would be a breaking change (as with the #263 rename)
and is disproportionate to a warning that never affects shipped artefacts.

## Consumer impact

None. Package ids, file names, and contents are unchanged; only the pack-time warning
for internal PR builds is suppressed.
