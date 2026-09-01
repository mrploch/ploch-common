# #204 — Expressions and owned property info article for the documentation site

**Type:** documentation
**Breaking changes:** none

## Summary

Added an authored article covering the `Ploch.Common.Linq` namespace to the Articles section of the
documentation site, aimed at developers deciding *when* a lambda selector beats a string literal rather than
simply listing the API.

## Details

New page `DocumentationSite/articles/expressions-and-owned-properties.md`, wired into
`DocumentationSite/articles/toc.yml`.

It covers:

- when **not** to use `ExpressionExtensions.GetMemberName` — `nameof` wins wherever the member is known at the
  call site; the expression overloads earn their keep inside generic methods and fluent builders;
- all three `GetMemberName` overloads, and why the two-type-parameter form is the one for library code: it is
  the only overload that unwraps the `UnaryExpression` produced when a value-type member is boxed into an
  `Expression<Func<TEntity, object>>` selector;
- `GetProperty` and `IOwnedPropertyInfo`/`OwnedPropertyInfo` — a `PropertyInfo` bound to its owning instance,
  the typed versus non-generic interfaces, and `Owner` typed as `TType`;
- production-shaped scenarios: a rename-safe sortable-field whitelist, a change-tracking snapshot, and
  validation messages whose property name cannot drift from the property checked;
- non-obvious behaviour: selectors must read a property *directly off the lambda parameter* — a nested
  selector (`c => c.Address.City`) and a selector closing over another instance (`c => other.Id`) both
  construct cleanly and then throw `TargetException` or silently read the wrong owner; indexer selectors
  resolve to `Item` and discard the index, so reads and writes need the index-taking overloads; and
  `yield`-based change enumeration reads values at enumeration time, not at call time;
- the failure modes and their exact exception messages, plus the cost/caching trade-off.

Every code sample was compiled against the real `Ploch.Common` API, and the documented exception messages and
return values were verified by executing them.

No changes to shipped library code — packages are unaffected.
