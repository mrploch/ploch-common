## TestingSupport.FluentAssertions: NullEmptyCollectionEquivalencyStep, expanded README, and solution doc wiring

### Ploch.TestingSupport.FluentAssertions

- **New equivalency step:** `NullEmptyCollectionEquivalencyStep` (`IEquivalencyStep`) that treats a `null` collection as equivalent to an empty collection (and vice versa) during `Should().BeEquivalentTo(...)` comparisons. The step intentionally does **not** equate `null` strings with empty strings (strings implement `IEnumerable`, so an explicit guard prevents that footgun). Falls through to the rest of the pipeline for all other cases, preserving configured options like `DateTimeOffset` tolerance and cyclic-reference handling.
- Use case: object graphs where one side leaves a collection property `null` and the other initialises it to an empty collection — common with EF Core navigation collections that were not eager-loaded.
- Comprehensive XML documentation with `<remarks>` and `<example>`.
- 14 unit tests covering: both null/empty directions, both-null, both-empty, populated-vs-different, populated-vs-equal, the explicit string guard, arrays, dictionaries, plus three direct `Handle()` unit tests.

### Documentation

- Expanded `src/TestingSupport.FluentAssertions/README.md` from a one-line stub to a proper reference with feature descriptions, usage examples, and the *why* behind each helper (`NullEmptyCollectionEquivalencyStep`, `StringAssertionExtensions.ContainAllEquivalentOf`, `PropertyInfoCollectionExtensions.ContainProperty/ContainProperties`).
- Wired `docs/` and `docs/libraries/` (35+ per-library Markdown pages) into `Ploch.Common.slnx` as solution folders so the IDE surfaces them alongside source.

### Tooling

- Added a Copilot Cloud Agent PR pipeline: `repo-investigator` → `pr-review-planner` → `plan-critic` → `pr-remediation`, plus an optional `pr-pipeline-orchestrator` for one-entry use. Implemented as `.github/agents/*.agent.md` agent definitions plus `docs/copilot-cloud-agent-pipeline.md` design doc and `docs/copilot-cloud-agent-mcp.example.json` MCP config example.
- Refreshed cross-tool agent instruction surfaces (`.github/copilot-instructions.md`, `.cursorrules`, `.windsurf/rules/contextstream.md`, `CLAUDE.md`, `GEMINI.md`, `.claude/rules/todo-tasks-execution.md`) to follow the same ContextStream + plan-mode + project-scope discipline.

### Refs

- #123 (umbrella documentation overhaul) — per-library README workstream for `Ploch.TestingSupport.FluentAssertions`.
