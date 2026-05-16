## CI: Move Qodana to the stable `2026.1` Community tag, deduplicate workflows, and re-enable auto-triggers

### CI

- **`qodana.yaml`** — linter pin moves from `jetbrains/qodana-cdnet:2026.1-eap` to the new stable **`jetbrains/qodana-cdnet:2026.1`** tag that JetBrains published as a Docker Community release. The cdnet image had historically shipped EAP-only, which forced #194 to disable auto-triggers because each pinned EAP tag expired within 4-7 weeks and broke CI on a rolling cycle. The stable tag ends the expiry treadmill.
- **`.github/workflows/qodana_code_quality.yml`** — action bumped from `JetBrains/qodana-action@v2025.3.2` to **`@v2026.1.0`** (Node.js v24, ends the deprecated-Node warning surfaced in recent runs), and auto-triggers re-enabled for `pull_request` and `push` to `master` and `releases/*`. The `--baseline,qodana.sarif.json,--solution,Ploch.Common.slnx` argument set and the `mrploch-development` shared-config clone are unchanged.
- **`.github/workflows/code_quality.yml`** — **deleted**. This was the duplicate workflow flagged in #198. Its config was incomplete (no `--solution` arg, no `mrploch-development` clone) so every manual dispatch failed with `solution/project relative file path is not specified`. The remaining `qodana_code_quality.yml` already handled `workflow_dispatch` plus PR/push triggers, so nothing is lost.
- **`README.md`** — Qodana badge URL repointed from `code_quality.yml` (deleted) to `qodana_code_quality.yml`. The badge will go green on the first successful auto-triggered run.

### Issues closed

- **#194** — Qodana: structural fix for recurring EAP licence expiry. Resolved by the stable cdnet tag.
- **#198** — ci(qodana): deduplicate Qodana workflows. Resolved by deleting `code_quality.yml`.

### Refs

- Closes #194, #198
- Builds on #193 (the EAP stopgap that preceded this structural fix)
