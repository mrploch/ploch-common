<!-- codex-claude-source:start -->
## Claude source alignment

Before working in this repository, read [`CLAUDE.md`](CLAUDE.md) and the task-relevant rules under
[`.claude/rules/`](.claude/rules/). They are the source of truth for agent guidance here.
<!-- codex-claude-source:end -->

# AGENTS.md

Guidance for Codex and other agents that read `AGENTS.md`. It is deliberately short: the verified repository knowledge
lives in one place so it cannot drift between agent files.

- **Entry point:** [`CLAUDE.md`](CLAUDE.md) covers the build and test commands, the rules that break the build or the
  release, the behaviour that surprises people, and the workflow.
- **Detail, loaded on demand:** [`docs/ai/`](docs/ai/README.md) has `ARCHITECTURE.md`, `TECH-STACK.md`,
  `CONVENTIONS.md`, `BACKLOG.md` and `AGENT-CONFIG.md`.
- **Solution:** `Ploch.Common.slnx`. Build with `dotnet build Ploch.Common.slnx`; test with
  `dotnet test Ploch.Common.slnx`. The sibling `../mrploch-development` checkout is required (it holds the central
  package versions).

When this file and `CLAUDE.md` disagree, `CLAUDE.md` wins; fix this file.
