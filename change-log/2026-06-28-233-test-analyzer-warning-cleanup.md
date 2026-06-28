## Test projects: clean up pre-existing analyzer warnings

Removed the entire backlog of pre-existing analyzer warnings across the test
projects so that future regressions stand out in the build output. The
test-project warning count went from **~814 to 0**; the build is now clean for
every test assembly.

This is a **test-only** change — no shipping-library (`src/`) code was modified,
so there is no consumer-visible behaviour change. The equivalent cleanup for the
shipping libraries is tracked separately in **#242**.

### Approach (per the issue's "balanced" policy)

- **Real in-code fixes** were preferred over suppressions. Examples: genuine bugs
  surfaced by the analysers were fixed — a malformed `#pragma` (CS1696), a
  `FluentAssertions` argument-count mismatch (CA2241), a reference-vs-value
  comparison that should have been a string cast (CS0252), a redundant
  `== true` (S1125), and a duplicate test method (S4144). Other fixes were
  mechanical: `nameof(...)` over string literals, `var` usage, `string.Empty`
  over `""`, member-group conversions, ordering, comment spacing, dead-code and
  redundant-local removal, and consistent collection-initialiser indentation.
- **Analyzer config scoping** (in `tests/.editorconfig`, with justifying
  comments) was used **only** for rules that are inherently inapplicable to
  non-shipping test assemblies — e.g. XML-doc rules, argument-validation rules,
  async-suffix/threading rules, and a small set of rules that fire as false
  positives on deliberately-shaped reflection/serialization test fixtures (whose
  "smells" are the subject under test, not accidental misuse). The scoping lives
  in the nested `tests/.editorconfig` so it never affects `src/` analysis.

### Refs

- #233
