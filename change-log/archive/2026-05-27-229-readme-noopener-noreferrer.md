## Docs: Add `rel="noopener noreferrer"` to Buy Me a Coffee anchor

### Changed

- `README.md` — the Buy Me a Coffee anchor in the `## Support the Project` section now sets `rel="noopener noreferrer"` alongside `target="_blank"`.

### Why

Anchors with `target="_blank"` should set `rel="noopener noreferrer"` to defend against reverse tabnabbing: without `noopener`, the opened page can access `window.opener` in the original tab and silently navigate it (e.g. to a phishing destination). Modern browsers default to `noopener` for `target="_blank"`, but the explicit attribute survives renderer differences across GitHub, NuGet, and other README renderers, and matches the safe-link pattern documented by MDN and OWASP.

Caught by `gemini-code-assist` while reviewing the equivalent change in `ploch-data` (mrploch/ploch-data#86 → mrploch/ploch-data#89). Brings the `ploch-common` README in line with `ploch-data`.

### Impact

Documentation-only. No code change, no behaviour change, no breaking change.

### Refs

- #229 (docs(readme): Add rel="noopener noreferrer" to Buy Me a Coffee anchor)
- Mirrors mrploch/ploch-data#89
