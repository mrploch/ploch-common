# Agent configuration — ploch-common

| Field | Value |
|---|---|
| Last verified | 2026-10-10, against commit `1f1764b166dabfad4a01b166b94a1115b88e7c6d` |
| Verified by | Listing every agent-related file in the committed tree with its size and content (first lines of each, not names); diffing the committed rules against the maintainer's wider rule set; checking every tool, skill and command named by a rule against what exists; scanning the committed files for credentials (none found; git history was not scanned) |
| Entry point | [`CLAUDE.md`](../../CLAUDE.md) |

Sizes are bytes; tokens are estimated as bytes ÷ 4. "Measured" figures come from the files; anything read off a session
listing is labelled an estimate. **Section 6 is proposals only; nothing here has been applied.** The proposals come from
a static `agent-config-optimizer plan` pass. Usage mining and runtime probes were **not** run (they read session
transcripts and start Claude Code, which runs the repository's hooks), so usage evidence is `UNVERIFIED` and every
"remove" or "disable" proposal is weaker than it would be with it. This repository is public: machine-level detail (a
developer's settings, permission lists, plugin set, local paths) is deliberately not recorded here.

## 1. Scope and layers

| Layer | What it holds | Where it is maintained |
|---|---|---|
| Repository, committed | `CLAUDE.md`, two rules, two hook scripts, Copilot, Gemini, Cursor, Windsurf, Junie, Qodo and OpenCode files | This repository |
| Repository, git-ignored but deployed locally | The rest of the rules, `skills/`, `agents/`, `settings.json`, `settings.local.json`, `.mcp.json` | Copied in by the maintainer's configuration tooling; absent from a clean clone |
| Ancestor (workspace) folder | Its own `CLAUDE.md`, rules, skills, MCP servers and settings | The maintainer's workspace; not part of this repository |
| User profile | Plugins, user skills, hooks, settings | The individual developer |
| Managed or system | Organisation policy | No file-based managed layer was found on the audited machine; device-management policy could not be inspected, so verdicts about permissions, hooks, plugins and servers are `UNVERIFIED` against it |

The repository's `.gitignore` states the intent: `.claude/rules/*` is ignored except `change-log.md` and
`todo-tasks-execution.md`, and `.claude/skills/`, `.claude/agents`, `.github/skills/`, `.claude/settings.json`,
`.claude/settings.local.json` and `/.mcp.json` are ignored (`.gitignore:411-444`). A fresh clone therefore receives only
two rules; everything else is deployed per machine.

## 2. Committed in this repository

Verdicts follow the rubric of the `agent-config-optimizer` skill. "Loads" means Claude Code reads it automatically.

| Path | Size | Content | Read by | Verdict |
|---|---:|---|---|---|
| `CLAUDE.md` | 38,665 → 7,045 | Before: lines 1–206 (25,077 B) were a generated ContextStream "startup" block for tools no configuration registers; the rest was real repository knowledge. After this change: a 104-line entry point and index | Claude Code, every session; loads | **Fix** — done in this change |
| `AGENTS.md` | 14,372 → 1,125 | Before: a verbatim copy of the old `CLAUDE.md` knowledge under a "Codex" heading, preceded by a generated block that linked to Windows absolute paths. After this change: a short, portable pointer to `CLAUDE.md` and `docs/ai/` | Codex and other `AGENTS.md` readers | **Deduplicate** — done in this change |
| `GEMINI.md` | 33,958 | 100% generated ContextStream block; no repository knowledge | Gemini CLI | **Fix or Remove** (decision 1) |
| `.cursorrules` | 22,720 | 100% generated ContextStream block | Cursor (legacy rules file) | **Fix or Remove** (decision 1) |
| `.windsurf/rules/contextstream.md` | 23,264 | 100% generated block, `trigger: always_on` | Windsurf | **Fix or Remove** (decision 1) |
| `.github/copilot-instructions.md` | 35,319 | 33,477 B generated block, then 1,842 B of overview that names `net9.0` as a target (the build files say net10.0) | Copilot repository instructions | **Fix** — replace with a pointer plus the corrected overview |
| `opencode.json` | 168 | A single remote ContextStream server | OpenCode | **Fix or Remove** (decision 1) |
| `.github/agents/*.agent.md` (5) | 10,602 | Repository-specific pull-request pipeline agents for the Copilot cloud agent; pin model names | Copilot cloud agent only | **Keep**; review the pinned model names when they age |
| `.junie/guidelines.md`, `.junie/mcp/mcp.json` | 8,569 / 500 | Test guidelines; three stdio servers started with unpinned `npx -y`/`uvx` | JetBrains Junie | **Keep**; pin the server versions |
| `.cursor/rules/sonarqube_mcp_instructions.mdc` | 1,935 | Vendor-generated SonarQube usage text, `alwaysApply: true` | Cursor | **Scope** or Keep |
| `.ai_config.toml`, `tests/Common.Tests/.ai_config.toml`, `qodo-gen.config.yaml` (×2), `.qodo/testConfig.toml` | 487 / 710 / 910 ×2 / 38 | Test-writing hints; the Qodo file is duplicated | Qodo and similar | **Deduplicate** the Qodo copy |
| `.swm/swimm.json`, `.ai/mcp/mcp.json` | 74 / 0 | A Swimm id; an empty file | — | **Remove** the empty file |
| `.claude/rules/change-log.md` | 655 | Real rule; names a `#taskid` file format | Claude Code; loads | **Fix** (the practised format has no `#`) |
| `.claude/rules/todo-tasks-execution.md` | 1,467 | Mostly pointers; tells the agent to file follow-ups as GitHub issues and to use `/execute-todo`, a skill that does not exist | Claude Code; loads | **Fix** (stale tracker vocabulary, dead reference) |
| `.claude/hooks/sonar-secrets/build-scripts/{pretool,prompt}-secrets.ps1` | 179 / 180 | Secret-scanning hook scripts | See proposal 7 | **Fix** (proposal 7) |
| `.claude/.notion-current-task.json` | 518 | Hook-written session state | Nothing | **Remove** from the index and ignore it |
| `.claude/worktrees/*` (2 gitlinks) | — | Commit pointers to agent worktrees with no `.gitmodules` | — | **Remove** from the index and ignore them |

Total of the committed agent-facing files: about 196,200 B (about 49k tokens), of which **Claude Code loads
automatically only `CLAUDE.md` and the two rules**: 40,787 B (about 10.2k tokens) before this change, 9,167 B (about
2.3k tokens) after it. About 138,500 B of the 196,200 are the generated ContextStream block, spread over five files.

**Not committed:** `settings.json`, `settings.local.json`, `.mcp.json`, agent definitions and skills. Permissions,
hooks, plugins and MCP servers therefore cannot be reviewed from the repository at all; they exist only as per-machine
state. No literal credential was found in any committed agent or configuration file.

## 3. Rules

**Loads unconditionally.** In a clean clone: the two committed rules (2,122 B, about 0.5k tokens). In a developer
checkout where the maintainer's rule set has been deployed into `.claude/rules/`: 32 files, 195,796 B (about 48.9k
tokens), measured, **all unconditional** because the deployed copies lack the `paths:` frontmatter that the workspace
originals carry.

| Relevance here | Rules | Size |
|---|---|---:|
| Applies to a library repository | process rules (agent behaviour, code quality, commits, branch naming, pull-request gate, secret protection, documentation, writing tests, dependencies, naming, change log) | the majority; the largest single files are the naming rule (32,294 B), the external-review rule (23,143 B) and the pull-request gate (16,389 B) |
| Probably irrelevant here | data access (15,913), data project (8,102), data-provider project (8,596), domain model (3,956), sample apps (1,437; no `samples/` folder exists), QA (994), and the application-layer parts of the naming and structure rules (the naming rule itself exempts library families) | about 96,000 B (about 24k tokens) |

Problems in the rules:

- **Loaded twice.** Five rules (agent behaviour, code quality, secret protection, response closing, response links) are
  byte-identical at repository and workspace scope, so both copies load: 24,761 B (about 6.2k tokens).
- **Two versions of one rule.** The deployed copies pre-date the workspace originals; 25 files differ, 22 of them only
  by the frontmatter. Two (the traceability rules) differ in substance and contradict each other.
- **Stale vocabulary** in the committed rules (GitHub-issue numbering, a non-existent skill) and Windows paths
  throughout the deployed ones.
- **Dead references.** The external-review rule names MCP tools (`codex-cli`, `gemini`, `gemini-cli`, `contextstream`)
  that no configuration registers, so its reviewer panel cannot run as written. The Antigravity and SonarCloud tools it
  names do exist.

## 4. Skills and tools

| Group | Copies | Used here |
|---|---|---|
| Process skills for implementing and finishing work (`implement-issue`, `dev-finishing-touches`, `dotnet-dev-finishing-touches`, `repo-review`, `dotnet-dev-practical`, `docfx-api-docs`, `repo-knowledge-base`, `agent-config-optimizer`) | deployed copies at repository scope, identical to the workspace originals apart from line endings | applicable: the repository has a DocFX site, .NET libraries and a CI gate |
| Personal and workspace utilities (`cleanup`, `session-review`, `record-ai-resource`, `prompt-lookup`, `composio`, `playwright-cli`) | same | not about this repository's code |
| Ten WinUI 3 skills (`winui3-*`) | same | **no**: the repository contains no WinUI project (about 57 KB of bodies, loaded only on invocation, plus their listing descriptions) |
| Four workspace skills relevant to the release process (NuGet Trusted Publishing, CI pipeline, merge preparation, session recording) | workspace scope only | not available to sessions started in this repository |

What this review needed: file reads, search, the `dotnet`, `git` and `gh` command lines, GitHub and nuget.org queries,
and a DocFX build. No IDE integration, language server or build-log tool was required.

## 5. Findings

Measured context cost at session start that is attributable to the repository (machine-specific contributors such as
plugin listings, connector instructions and the skill listing are not included here):

| # | Finding | Cost or risk |
|---|---|---|
| 1 | Generated ContextStream block in five instruction files; the tools it names are not registered anywhere | 138,500 B committed; 25,077 B of it in the file Claude Code loads every session (about 6.3k tokens) |
| 2 | Deployed rules are unscoped | about 48.9k tokens per session in a developer checkout, against 0.5k in a clean clone |
| 3 | Five rules load from two scopes | about 6.2k tokens |
| 4 | About 96,000 B of data-access, domain-model and application-layer rules in a library repository | about 24k tokens if unscoped |
| 5 | `AGENTS.md` is a copy of `CLAUDE.md` with Windows absolute links; the Copilot overview states `net9.0` | drift between entry files; dead links for every other user |
| 6 | Stale tracker vocabulary and a dead skill reference in the two committed rules | an agent follows instructions that cannot work |
| 7 | The committed secret-scan hooks are registered by nothing; a developer's own hook registration may use an interpreter that is not installed on every platform | no scanning from repository scope; silent failure |
| 8 | A hook-written state file and two agent-worktree gitlinks are tracked | session state in history; broken submodule-like entries in checkout |
| 9 | No committed settings file, so no shared allow-list, hook registration or verification command (this change adds the verification commands to `CLAUDE.md`) | every developer starts from an empty or hand-grown permission list |
| 10 | Tool-managed folders (`.agents/`, `.agentbridge/`) are neither committed nor ignored and can hold conversation databases | a blind `git add -A` would commit them |
| 11 | Process the platform does not enforce: code-owner review is required without a `CODEOWNERS` file; the Actions Build job is not a required check | review rules that look stronger than they are |

## 6. Proposed changes

Each proposal is reversible and scoped to the narrowest layer that fixes it. "Kept by" says where the capability
remains.

### Priority 1 — correctness and the largest savings

| # | Change | Where (layer) | Effect |
|---|---|---|---|
| 1 | Replace `CLAUDE.md` with the 104-line entry point and index (**done in this change**) | repository | −31.6 KB (about 7.9k tokens) per session; the real knowledge moved to `docs/ai/` and the ContextStream block is dropped. Kept by: `docs/ai/` and the index |
| 2 | After decision 1: reduce `GEMINI.md`, `.cursorrules`, `.windsurf/rules/contextstream.md` and the first 470 lines of `.github/copilot-instructions.md` to a pointer to `CLAUDE.md`, and remove the ContextStream entry from `opencode.json` if the service is retired | repository | −113 KB of dead instructions for other tools; removes calls to unregistered tools. Kept by: `CLAUDE.md` for every tool |
| 3 | Replace `AGENTS.md` with a short pointer (**done in this change**) and correct the `net9.0` line in the Copilot overview | repository | removes drift and Windows-only links. Kept by: `CLAUDE.md` |
| 4 | Reword `todo-tasks-execution.md` and `change-log.md` to the repository's practice; drop the `/execute-todo` pointer | repository | stops contradicting practice. Kept by: the same files |
| 5 | Make the deployed rule copies keep their `paths:` frontmatter, or deploy only the rules a library repository needs | the maintainer's configuration source (workspace) | up to about 48.9k tokens per session in a developer checkout. Kept by: rules still load when matching files are read |
| 6 | Remove the five byte-identical rule copies from repository scope and keep the workspace originals | repository (ignored copies) | −24,761 B (about 6.2k tokens). Kept by: the workspace copies |

### Priority 2 — safety and day-to-day speed

| # | Change | Where (layer) | Effect |
|---|---|---|---|
| 7 | Commit a minimal `.claude/settings.json` that wires up the secret-scan hook scripts portably (or delete them), and that carries a read-only allow-list of inspection commands only (for example `git status`, `git diff`, `git log`, `gh pr view`, `gh pr checks`). Never allow `dotnet restore`, `dotnet build` or `dotnet test` (they run repository-controlled MSBuild targets and test code), `git push`, `git merge`, `gh api`, `curl`, `rm` or a whole command family | repository | the scan works from a clean clone; fewer prompts without a blanket allow. Kept by: unchanged interactive approval for anything that writes or executes repository code |
| 8 | Add a `CODEOWNERS` file, or drop the code-owner requirement; consider making the Actions Build job a required check | code host | the review rule becomes real. Kept by: the four existing required checks |
| 9 | Ignore `.claude/.notion-current-task.json`, `.claude/worktrees/`, `.agents/` and `.agentbridge/`, and untrack the first two | repository | no session state or conversation database can be committed. Kept by: the files stay on disk |
| 10 | Pin the Junie server versions and the model names in `.github/agents/*.agent.md` | repository | reproducible reviewer set-up. Kept by: the same files |

### Priority 3 — tidy-up

| # | Change | Where (layer) | Effect |
|---|---|---|---|
| 11 | Scope the data-access, data-project, data-provider, domain-model, sample-apps and QA rules (for example `paths:` on `**/*.Data*/**`), or keep them out of library repositories | the maintainer's configuration source | about 96,000 B (about 24k tokens) unconditional → loaded only where such projects exist. Kept by: the rules load in the repositories that have data and UI projects |
| 12 | Hide the ten `winui3-*` skills in this repository (`skillOverrides: "off"` in the local settings) | user or repository-local settings | smaller listing. Kept by: still installed, listed in repositories with a WinUI project |
| 13 | Add the Trusted Publishing and CI-pipeline workspace skills to this repository's scope, since the release procedure uses both ideas | the maintainer's configuration source | the release workflow is easier to maintain. Kept by: workspace originals |
| 14 | Delete the empty `.ai/mcp/mcp.json` and de-duplicate the Qodo config | repository | tidy-up |
| 15 | Remove stale root notes that agents may read as instructions: `last-session.md`, `TODO.md`, `temp.md`, `post--original-complehensive.md` | repository | no stale instructions. Kept by: git history |

Machine-level proposals (permission lists, hook interpreters, plugin overlap, duplicate MCP routes) are in the
maintainer's report, not in this file.

## 7. Decisions needed

1. Is ContextStream retired? The answer gates proposals 1–2: either remove every reference to it, or register the server
   and keep one generated block in one file.
2. Which entry file is canonical, and should the files for Gemini, Cursor, Windsurf, OpenCode and Copilot be committed
   at all or reduced to pointers?
3. Which layer owns the shared rules — the workspace or each repository — and should the deployer preserve `paths:`?
4. Which identifier scheme applies to new work in this repository: the GitHub numbers its history uses, or the
   maintainer's newer tracker identifiers (see [`CONVENTIONS.md`](CONVENTIONS.md) section 10)?
5. Should a project-level settings file (hook registration and a read-only allow-list) be committed, or stay per
   machine?
6. Add `CODEOWNERS`, or drop the code-owner requirement from branch protection?
7. May usage be mined from session transcripts (`agent-config-optimizer`, phase 2)? Without it every disable proposal
   here rests on configuration evidence alone.
