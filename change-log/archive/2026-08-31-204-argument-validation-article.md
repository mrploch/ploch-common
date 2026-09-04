# #204 — Argument validation article for the documentation site

**Type:** documentation
**Breaking changes:** none

## Summary

Added an authored article covering the `Ploch.Common.ArgumentChecking` namespace to the Articles section of the
documentation site, aimed at developers deciding *which* guard to use rather than simply listing the API.

## Details

New page `DocumentationSite/articles/argument-validation.md`, wired into `DocumentationSite/articles/toc.yml`.

It covers:

- the caller-error (`NotNull` → `ArgumentNullException`) versus invalid-internal-state
  (`RequiredNotNull` → `InvalidOperationException`) distinction, and why choosing the wrong family produces
  misleading exceptions;
- every guard `Guard` and `PathGuard` expose — `NotNull`, `NotNullOrEmpty`, `NotNullOrDefault`, `Positive`,
  `NotOutOfRange`, `RequiredTrue`/`RequiredFalse`, `RequiredNotNull`, `RequiredNotNullOrEmpty`, `IsValidPath`,
  `EnsureFileExists`, `RequiredIsValidPath`, `RequiredFileExists`, `RequireValidPath`;
- non-obvious behaviour: `NotNullOrEmpty` enumerating a deferred sequence (and so executing a LINQ query
  twice), `[Flags]` enum handling in `NotOutOfRange`, `NotNullOrDefault` rejecting `0`/`false`/`Guid.Empty`,
  and the automatically-captured expression/member/file/line default message on `RequiredTrue`;
- the `netstandard2.0` versus `net7.0+` API asymmetries, including the mandatory positional parameter name on
  the `netstandard2.0` target;
- migration away from the deprecated `Ploch.Common.DawnGuard` package.

Every code sample was compiled against the real `Ploch.Common` API, and the documented exception messages were
verified by executing them.

No changes to shipped library code — packages are unaffected.
