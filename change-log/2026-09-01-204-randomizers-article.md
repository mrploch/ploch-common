# #204 — Randomizers article for the documentation site

**Type:** documentation
**Breaking changes:** none

## Summary

Added an authored article covering the `Ploch.Common.Randomizers` namespace to the Articles section of the
documentation site, written around the traps a developer actually hits rather than as a restatement of the
type list.

## Details

New page `DocumentationSite/articles/randomizers.md`, wired into `DocumentationSite/articles/toc.yml`.

It covers:

- **the namespace ships no DI registrations** — injecting `IRandomizer<T>` compiles and then throws
  `InvalidOperationException: No service for type 'Ploch.Common.Randomizers.IRandomizer\`1[System.Int32]' has
  been registered.` The static `Randomizer.GetRandomizer<T>()` factory is the entry point; registering the
  concrete implementations is the consumer's job, and a registration against `IRangedRandomizer<T>` does not
  satisfy a request for `IRandomizer<T>`;
- the closed five-type factory `switch` and its exact `NotSupportedException` messages, including the
  unguarded `null` path that reports an empty type name;
- **the upper bound is exclusive everywhere** — `int`, the `string` character bounds and the date day count
  all delegate to `Random.Next`, contradicting the inherited `BaseRandomizer<TValue>` documentation, which
  describes both bounds as inclusive;
- `BooleanRandomizer` treating its range as an equality check rather than an ordering, so `(true, false)` is
  still a coin toss;
- `StringRandomizer`'s three overloads: the ranged one uses only the first character of each argument, the
  length-taking one lives on the concrete class, and its default `'0'`–`'Z'` bounds include the ASCII
  punctuation between the digits and the letters;
- the date randomizers' **day resolution** — the time of day is copied from `minValue`, a sub-day range
  collapses silently to `minValue`, the top day is unreachable, and a reversed sub-day range returns the
  minimum without throwing while a reversed multi-day range throws;
- **no seeding and therefore no reproducibility**, with a `BaseRandomizer<T>`-derived seeded implementation
  and a `Guid` randomizer as the extension-point examples;
- threading (each randomizer owns a private unsynchronised `Random`, so `AddTransient`/`AddScoped` over
  `AddSingleton`), and a **target-framework difference**: on `net48` two randomizers created within one clock
  tick produce identical sequences, which makes calling the factory inside a loop a real bug on .NET
  Framework consumers of the `netstandard2.0` asset;
- a worked staging-catalogue seeder that takes the interfaces rather than calling the factory.

Every code sample was compiled against the real `Ploch.Common` API (0 warnings, 0 errors) and executed; every
quoted exception message, distribution count and output value in the article is real program output. The
invariance of `IRangedRandomizer<TValue>` and the identical-sequence behaviour on `net48` were both confirmed
by separate compile/run probes.

No changes to shipped library code — packages are unaffected.
