# AI knowledge base — ploch-common

This folder holds the verified, agent-oriented description of the repository: what the parts are, how it is built and
shipped, how work is done here, what is open, and what agent configuration exists. The entry point is
[`CLAUDE.md`](../../CLAUDE.md) at the repository root; read the document below that matches the task before changing
anything.

| Field | Value |
|---|---|
| Last verified | 2026-10-10, against commit `1f1764b166dabfad4a01b166b94a1115b88e7c6d` |
| Standard version | 1 |

## Documents

| File | Answers | Changes when |
|---|---|---|
| ARCHITECTURE.md | What are the parts and how do they interact? | Public surface or flow changes |
| TECH-STACK.md | What is it built with, and how is it built and shipped? | Dependencies, CI or release change |
| CONVENTIONS.md | How is code written, committed and reviewed here? | Process or style changes |
| BACKLOG.md | What is open, planned and owed? | Often; it is a snapshot |
| AGENT-CONFIG.md | What agent configuration exists and what should change? | Agent configuration changes |

## The standard

This set follows one standard layout, meant to be the same in every repository that adopts it, so an agent knows where
to look without searching.

- **Fixed names and location.** `CLAUDE.md` at the root, the five documents above in `docs/ai/`. A repository may add a
  document; it does not rename or drop one. A document that does not apply keeps its file and says so in one line.
- **Fixed sections.** Each of the five documents has its own list of numbered top-level sections, kept in the same order
  in every repository. An empty section says "None" rather than disappearing. `CLAUDE.md` and this index have fixed,
  unnumbered headings.
- **Verification stamp.** Every document in `docs/ai/` starts with the date and commit it was verified against.
  `CLAUDE.md` carries none, and neither does a stub: it says it was not produced instead.
- **Evidence over assertion.** A claim that is not obvious cites a file path, a command that was run, or a tracker link.
  Anything not checked is marked `UNVERIFIED`.
- **Describe what is, not what should be.** Where practice differs from a written rule, the document records the
  practice and names the difference.
- **No duplication of human documentation.** Link to `README.md`, the contribution guide and the guides instead of
  copying them.
- **Safe for its audience.** Never credentials or detail of an unfixed security weakness. In a public repository, also
  no personal paths, account identifiers or machine-specific ports.
- **Format.** Prose wrapped at 120 characters (table rows may be longer), British English.

This repository is public, so the documents here omit machine-level detail of agent configuration and anything that
comes from the maintainer's private tracker. The site build must not publish this folder: `DocumentationSite/docfx.json`
excludes `docs/ai/**`, and that exclusion has to stay.

## Keeping it current

| Kind of change | Document to update in the same pull request |
|---|---|
| A project is added, removed or renamed; a public type or flow changes; a solution gains or loses a project | `ARCHITECTURE.md` (sections 2–4), and `TECH-STACK.md` section 7 if the set of published packages changes |
| A package version, the SDK, a build property or an analyser changes; a workflow or branch-protection rule changes; a release is cut | `TECH-STACK.md` |
| The commit, branch, pull-request, test or documentation convention changes, or a written rule stops matching practice | `CONVENTIONS.md` (section 10 for any difference) |
| Issues are opened or closed in bulk, a release ships, or a review finds new debt | `BACKLOG.md` (it is a snapshot; refresh it rather than editing counts) |
| An instruction file, rule, skill, hook, permission or MCP server is added, removed or changed | `AGENT-CONFIG.md`, and `CLAUDE.md` if the essentials change |

`BACKLOG.md` ages fastest. A full refresh re-verifies every stamp and uses the `repo-knowledge-base` skill (`refresh`).
