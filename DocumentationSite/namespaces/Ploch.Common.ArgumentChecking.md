---
uid: Ploch.Common.ArgumentChecking
summary: *content
---

### Overview

`Ploch.Common.ArgumentChecking` provides fluent guard clauses that validate arguments and invariants at the top
of a method, so a violation is reported at the boundary that caused it rather than as a
`NullReferenceException` several frames deeper. `Guard` exposes the argument-facing checks — `NotNull`,
`NotNullOrEmpty`, `Positive`, `NotOutOfRange` — which throw from the `ArgumentException` family and capture the
parameter name automatically. `PathGuard` adds the equivalent checks for file-system paths.

The `Required*` variants exist for a deliberately different case: validating *state* rather than *input*. A null
argument is the caller's mistake and warrants an `ArgumentNullException`; a null field that should already have
been initialised is the object's own mistake and warrants an `InvalidOperationException`. Keeping the two apart
makes the resulting exception meaningful without anyone having to read the stack trace. This namespace
supersedes the deprecated `Ploch.Common.DawnGuard`, which wrapped the third-party Dawn.Guard library.

See the [Ploch.Common library guide](../../docs/libraries/common.md) for installation instructions and worked
examples.
