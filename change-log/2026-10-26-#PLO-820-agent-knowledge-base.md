# Add a verified agent knowledge base (#354)

Mostly contributor and AI-agent guidance. The only build change is the `Microsoft.SourceLink.GitHub` pin; there is no
API or runtime behaviour change.

- `CLAUDE.md` is now a short entry point. It replaces the generated ContextStream block and the stale project summary.
- New `docs/ai/` knowledge base, verified against the code: `ARCHITECTURE.md`, `TECH-STACK.md`, `CONVENTIONS.md`,
  `BACKLOG.md`, `AGENT-CONFIG.md`, and `README.md` (the standard and the refresh rules).
- `README.md` links the knowledge base from its Contributing section.
- `DocumentationSite/docfx.json` excludes `docs/ai/**`, so the knowledge base is not published on the documentation
  site.
- `AGENTS.md` is now a short, portable pointer to `CLAUDE.md` and `docs/ai/`, replacing the stale copy and its
  Windows-only links.
- Build: `Microsoft.SourceLink.GitHub` moves from 8.0.0 to 10.0.114. The 8.0.0 pin brought in a `Microsoft.Build.Tasks.Git`
  version with a NuGet audit warning, which failed the Publish Docs warning guard on `main`. 10.0.114 is the newest
  release that clears the warning without adding a `System.IO.Hashing` dependency. Packages still embed SourceLink
  information as before.
