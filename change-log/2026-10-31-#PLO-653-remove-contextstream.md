# Remove the retired ContextStream agent configuration (PLO-653)

Contributor and AI-agent guidance only; there is no build, API or runtime behaviour change.

- `GEMINI.md` and `.cursorrules` were entirely a generated ContextStream block. Each is now a short pointer to
  `CLAUDE.md` and `docs/ai/`.
- `.github/copilot-instructions.md` keeps its repository overview; the generated block before it is replaced by a
  pointer to `CLAUDE.md` and `docs/ai/`.
- Removed `.windsurf/rules/contextstream.md` and `opencode.json`, which only configured ContextStream.
- `Ploch.Common.slnx` no longer lists the missing `CLAUDE.md-pre-contextstream` solution item.
- `docs/ai/AGENT-CONFIG.md` records decision 1 (ContextStream is retired) and the files it affected.
