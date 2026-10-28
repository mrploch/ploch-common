# Add a verified agent knowledge base (#354)

Contributor and AI-agent guidance only; there is no build, API or runtime behaviour change.

- `CLAUDE.md` is now a short entry point. It replaces the generated ContextStream block and the stale project summary.
- New `docs/ai/` knowledge base, verified against the code: `ARCHITECTURE.md`, `TECH-STACK.md`, `CONVENTIONS.md`,
  `BACKLOG.md`, `AGENT-CONFIG.md`, and `README.md` (the standard and the refresh rules).
- `README.md` links the knowledge base from its Contributing section.
- `DocumentationSite/docfx.json` excludes `docs/ai/**`, so the knowledge base is not published on the documentation
  site.
- `AGENTS.md` is now a short, portable pointer to `CLAUDE.md` and `docs/ai/`, replacing the stale copy and its
  Windows-only links.
