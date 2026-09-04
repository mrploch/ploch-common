# Deprecate Ploch.Common.DawnGuard (#342)

`Ploch.Common.DawnGuard` is deprecated as of this release. It wraps the third-party
[Dawn.Guard](https://www.nuget.org/packages/Dawn.Guard/) library to provide type guards;
argument validation across the rest of the library has moved to
`Ploch.Common.ArgumentChecking`, which ships inside the core `Ploch.Common` package and
carries no external dependency.

The `TypeGuards` API was already marked `[Obsolete]`, so the compiler already pointed at the
replacement. What was missing was any signal to someone looking at the **package** rather than
the code: the nuget.org listing had no description at all, and the packaged README described
the library as though it were current.

## What changed

- **`README.md`** — this file is the `PackageReadmeFile`, so it is what a consumer sees on
  nuget.org. It now leads with a deprecation warning and carries migration guidance.
- **`Ploch.Common.DawnGuard.csproj`** — gains a `<Description>` that leads with
  `[DEPRECATED]` and names the replacement, plus `<PackageTags>`. Neither was set before, so
  the listing was silent on both counts.

No code changed. The package still builds and ships, so existing consumers are warned rather
than broken.

## `AssignableTo` has no direct replacement — be aware

Writing the migration guide surfaced something the deprecation notes had assumed away:
**`ArgumentChecking` does not cover everything this package does.**

`ArgumentChecking` provides `NotNull`, `NotNullOrEmpty`, `RequiredNotNull`,
`NotNullOrDefault`, `RequiredTrue`/`RequiredFalse`, `Positive`, `NotOutOfRange`, and the
`PathGuard` methods. It has **no type-assignability guard**, which is precisely and only what
`TypeGuards.AssignableTo` / `AssignableToOrNull` provide.

`Ploch.Common.Reflection.TypeExtensions.IsImplementing` is the closest existing API and is
**not** a drop-in substitute:

- it returns a `bool` rather than throwing `ArgumentException`, so it is a predicate and not a
  guard; and
- it returns `false` when the type *is* the target type, whereas `AssignableTo` succeeds in
  that case, because `Type.IsAssignableFrom` is reflexive.

The README therefore documents the hand-written equivalent rather than pointing at an API
that behaves differently. Adding a real `AssignableTo` guard to `ArgumentChecking` is tracked
separately; until then, the deprecation is honest about the gap.

## Follow-up outside this repository

Once the release carrying this ships, the package should be deprecated on nuget.org itself:
reason **Legacy**, alternate package **`Ploch.Common`**, with a message pointing at
`Ploch.Common.ArgumentChecking`. That is an account-level operation and cannot be expressed in
the repository.
