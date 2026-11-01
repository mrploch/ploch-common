# Ignore machine-local Codex state (#357)

Repository housekeeping only; there is no build, API or runtime behaviour change.

- `.gitignore` now ignores `.codex/`. Locally that folder holds a `config.toml` with machine-local MCP servers
  (JetBrains IDE port, SonarQube command) and SonarQube's generated secret-scanning hooks, whose `hooks.json`
  hard-codes absolute paths for one machine. None of it was tracked; the rule stops it being committed by accident.
