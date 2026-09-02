# Randomizers

Almost every non-trivial project ends up needing throwaway values: a seeded demo catalogue for a
staging environment, a load-test fixture that must not hammer the same primary key repeatedly, a
test-data builder that fills in the fields a given test does not care about, a sampled subset of a
large batch for a spot check.

The obvious implementation — `new Random()` at each call site — scatters a stateful,
non-deterministic dependency through the codebase and makes the surrounding code awkward to test.
`Ploch.Common.Randomizers` turns "give me a value of type `T`" into an interface, so the production
path can use a real generator while a test substitutes a stub that returns exactly the value the
assertion needs.

| Type | Purpose |
|------|---------|
| <xref:Ploch.Common.Randomizers.Randomizer> | Static factory. `GetRandomizer<TValue>()` and `GetRandomizer(Type)`. |
| <xref:Ploch.Common.Randomizers.IRandomizer> / <xref:Ploch.Common.Randomizers.IRandomizer`1> | "Produce a value." The non-generic form exists for reflection-driven callers. |
| <xref:Ploch.Common.Randomizers.IRangeRandomizer> / <xref:Ploch.Common.Randomizers.IRangedRandomizer`1> | Adds `GetRandomValue(min, max)`. |
| <xref:Ploch.Common.Randomizers.BaseRandomizer`1> | Abstract base implementing the non-generic plumbing. The extension point for your own types. |
| <xref:Ploch.Common.Randomizers.IntRandomizer>, <xref:Ploch.Common.Randomizers.BooleanRandomizer>, <xref:Ploch.Common.Randomizers.StringRandomizer>, <xref:Ploch.Common.Randomizers.DateTimeRandomizer>, <xref:Ploch.Common.Randomizers.DateTimeOffsetRandomizer> | The five shipped implementations. |

For a wider tour of the package, see the [Ploch.Common library guide](../../docs/libraries/common.md).

## This is not a security facility

The randomizers wrap [`System.Random`](https://learn.microsoft.com/dotnet/api/system.random). The namespace carries an assembly-level suppression of
`CA5394: Do not use insecure randomness`, and that suppression is a statement of intent rather than
an oversight — these types exist to produce sample data. Anything that must be unguessable by an
attacker (tokens, password resets, nonces, session identifiers) belongs in
[`RandomNumberGenerator`](https://learn.microsoft.com/dotnet/api/system.security.cryptography.randomnumbergenerator), not here.

## Start with the factory — there are no DI registrations

**This is the trap to know about before anything else.** The namespace ships no
`IServiceCollection` extension method and no `ServicesBundle`. Injecting `IRandomizer<T>` without
registering a concrete implementation yourself compiles perfectly and fails at resolution time:

```csharp
var provider = new ServiceCollection().BuildServiceProvider();

provider.GetService<IRandomizer<int>>();          // null
provider.GetRequiredService<IRandomizer<int>>();  // throws
```

```text
System.InvalidOperationException: No service for type
'Ploch.Common.Randomizers.IRandomizer`1[System.Int32]' has been registered.
```

The supported entry point is the static factory:

```csharp
IRangedRandomizer<int> quantities = Randomizer.GetRandomizer<int>();

var quantity = quantities.GetRandomValue(1, 50);
```

If you would rather inject the abstraction — and for anything that needs to be stubbed in a test,
you should — registration is your responsibility. One line per type actually used:

```csharp
services.AddTransient<IRangedRandomizer<int>, IntRandomizer>();
services.AddTransient<IRangedRandomizer<string>, StringRandomizer>();
```

Register `IRangedRandomizer<T>` rather than `IRandomizer<T>` unless no consumer will ever want a
bounded value: `IRangedRandomizer<T>` derives from `IRandomizer<T>`, but a registration against one
interface does not satisfy a request for the other. Add a forwarding registration when both are
wanted:

```csharp
services.AddTransient<IntRandomizer>();
services.AddTransient<IRangedRandomizer<int>>(sp => sp.GetRequiredService<IntRandomizer>());
services.AddTransient<IRandomizer<int>>(sp => sp.GetRequiredService<IntRandomizer>());
```

Why `AddTransient` rather than `AddSingleton` is covered in [Threading](#threading) below.

## What the factory supports

The type-to-implementation map is a closed `switch` over five types. Anything else — including
`long`, `decimal`, `Guid`, and the `Nullable<T>` form of a supported type — is rejected at runtime,
not at compile time:

| Call | Result |
|------|--------|
| `Randomizer.GetRandomizer<int>()` | `IntRandomizer` |
| `Randomizer.GetRandomizer<bool>()` | `BooleanRandomizer` |
| `Randomizer.GetRandomizer<string>()` | `StringRandomizer` |
| `Randomizer.GetRandomizer<DateTime>()` | `DateTimeRandomizer` |
| `Randomizer.GetRandomizer<DateTimeOffset>()` | `DateTimeOffsetRandomizer` |
| `Randomizer.GetRandomizer<long>()` | `NotSupportedException`: `Randomizer for type System.Int64 is not supported.` |
| `Randomizer.GetRandomizer<int?>()` | `NotSupportedException`: <code>Randomizer for type System.Nullable\`1[System.Int32] is not supported.</code> |
| `Randomizer.GetRandomizer(null!)` | `NotSupportedException`: `Randomizer for type  is not supported.` — note the empty type name; there is no `ArgumentNullException` guard on this path. |

Two consequences are worth internalising. First, a generic method that calls
`Randomizer.GetRandomizer<T>()` for a caller-supplied `T` has no compile-time protection at all;
constrain what the API accepts, or catch `NotSupportedException` and say something more useful than
the framework message. Second, **the factory allocates a fresh instance on every call** — nothing is
cached:

```csharp
var first = Randomizer.GetRandomizer<int>();
var second = Randomizer.GetRandomizer<int>();

ReferenceEquals(first, second);   // false
```

That matters more than it appears to; see
[target-framework differences](#target-framework-differences-matter-here).

`GetRandomizer<TValue>()` returns `IRangedRandomizer<TValue>`, so the ranged overload is available
without a cast. `GetRandomizer(Type)` returns the non-generic `IRandomizer`, whose `GetRandomValue`
takes and returns `object`; passing a boxed value of the wrong type throws `InvalidCastException`
from the cast inside `BaseRandomizer<TValue>`:

```csharp
IRandomizer untyped = Randomizer.GetRandomizer(typeof(int));

untyped.GetRandomValue(5, 10);   // a boxed int
untyped.GetRandomValue("a", "z");
// InvalidCastException: Unable to cast object of type 'System.String' to type 'System.Int32'.
```

`IRandomizer<out TValue>` is covariant, so an `IRandomizer<string>` can be held as an
`IRandomizer<object>`. `IRangedRandomizer<TValue>` is *invariant*, because its `TValue` appears in a
parameter position, so the same assignment does not compile for the ranged interface.

Covariance is of less help here than it first appears, though, because **variance conversions apply
only to reference types** and four of the five supported types are value types:

```csharp
IRandomizer<object> ok = Randomizer.GetRandomizer<string>();   // fine — string is a reference type
IRandomizer<object> no = Randomizer.GetRandomizer<int>();
// error CS0266: Cannot implicitly convert type 'IRangedRandomizer<int>' to 'IRandomizer<object>'
```

For a collection holding generators for a mix of types, use the **non-generic** `IRandomizer`, which
is what `GetRandomizer(Type)` returns:

```csharp
var generators = new Dictionary<Type, IRandomizer>
{
    [typeof(int)] = Randomizer.GetRandomizer(typeof(int)),
    [typeof(string)] = Randomizer.GetRandomizer(typeof(string)),
    [typeof(bool)] = Randomizer.GetRandomizer(typeof(bool))
};

foreach (var (type, generator) in generators)
{
    Console.WriteLine($"{type.Name}: {generator.GetRandomValue()}");
}
// Int32: 273794950
// String: 905Zt27s
// Boolean: False
```

That is the shape to reach for when filling an object's properties by reflection — a generic
test-data builder, for instance — which is exactly why the non-generic interface exists.

## `int` — the upper bound is exclusive

<xref:Ploch.Common.Randomizers.IntRandomizer> delegates straight to `Random.Next`, which means the
range is **inclusive of `minValue` and exclusive of `maxValue`** — despite the inherited
`BaseRandomizer<TValue>` documentation describing both bounds as inclusive. Over 100,000 draws of
`GetRandomValue(0, 3)` the observed counts were 33,308 / 33,425 / 33,267 for 0, 1 and 2; the value 3
never appeared.

```csharp
var numbers = Randomizer.GetRandomizer<int>();

numbers.GetRandomValue(7, 8);   // always 7 — a one-element range
numbers.GetRandomValue(7, 7);   // always 7 — an empty range, and not an error
numbers.GetRandomValue(1, 7);   // a die roll: 1..6
```

So a "1 to 6 inclusive" die is `GetRandomValue(1, 7)`. Getting this wrong is silent: the code runs,
the tests pass, and one face of the die simply never comes up.

Reversing the bounds throws from `Random.Next` rather than from `Ploch.Common`:

```csharp
numbers.GetRandomValue(10, 5);
// ArgumentOutOfRangeException: 'minValue' cannot be greater than maxValue. (Parameter 'minValue')
```

The parameterless `GetRandomValue()` is `Random.Next()` — a non-negative `int`. Across 100,000 draws
the observed minimum was 9,897 and the maximum 2,147,465,439; negative values are never produced, so
it is not a general-purpose "any `int`" generator. Ask for the range actually wanted.

## `bool` — the range constrains only when both bounds are equal

<xref:Ploch.Common.Randomizers.BooleanRandomizer> pins the result when `minValue == maxValue` and
otherwise tosses a fair coin. It does not interpret the pair as an ordered range, so *both* mixed
pairs are coin tosses:

```csharp
var flags = Randomizer.GetRandomizer<bool>();

flags.GetRandomValue(true, true);    // always true
flags.GetRandomValue(false, false);  // always false
flags.GetRandomValue(false, true);   // 50/50
flags.GetRandomValue(true, false);   // also 50/50 — not an error, not an empty range
```

Over 10,000 draws each, `(true, false)` produced 5,018 `true` values and `(false, true)` produced
4,962. Use the equal-bounds form deliberately — it is the neat way to pin one field of a generated
record while the rest stays random — and never rely on argument order meaning anything.

## `string` — three overloads, and only two are on the interface

The parameterless call returns eight characters drawn from `A–Z`, `a–z` and `0–9`, which is the
right default for a display name, a slug or an idempotency key in sample data:

```csharp
var text = Randomizer.GetRandomizer<string>();

text.GetRandomValue();   // e.g. "ZWDXxAY0", "cB2E6isa", "xLO4UFqy"
```

The ranged overload is the surprising one. It satisfies `IRangedRandomizer<string>` by taking two
`string` arguments, but **only the first character of each is used** — they are character bounds
rather than string bounds, and the length is fixed at eight:

```csharp
text.GetRandomValue("a", "z");       // 8 characters from 'a'..'y'
text.GetRandomValue("abc", "zzz");   // identical behaviour — "bc" and "zz" are ignored
```

The upper character bound is exclusive here too, for the same `Random.Next` reason. Accumulating the
distinct characters seen across 20,000 calls to `GetRandomValue("a", "z")` yields
`abcdefghijklmnopqrstuvwxy` — 25 letters. `z` never appears. Pass `"{"`, the character immediately
after `z`, as the upper bound to get the whole lower-case alphabet.

The third overload takes an explicit length, and lives on
<xref:Ploch.Common.Randomizers.StringRandomizer> rather than on the interface, so the factory result
needs a cast to reach it:

```csharp
var randomizer = (StringRandomizer)Randomizer.GetRandomizer<string>();

randomizer.GetRandomValue(16, 'a', '{');   // 16 lower-case letters
randomizer.GetRandomValue(0);              // "" — a zero length is allowed
```

Its default character bounds, `'0'` to `'Z'`, are a contiguous ASCII span rather than an
alphanumeric set, so the punctuation sitting between the digits and the letters is included. The
distinct characters observed across 20,000 calls to `GetRandomValue(6)` were:

```text
0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXY
```

Forty-two characters — the ten digits, seven punctuation marks, and `A` through `Y`. If the value
will end up in a URL, a filename or a CSV, pass explicit bounds rather than taking the default.

The failure modes differ by argument:

| Call | Result |
|------|--------|
| `text.GetRandomValue(null!, "z")` | `ArgumentNullException`: `Value cannot be null. (Parameter 'minChar')` |
| `text.GetRandomValue("", "z")` | `IndexOutOfRangeException` — the guard checks for `null`, not for empty, and then indexes `[0]` |
| `text.GetRandomValue("z", "a")` | `ArgumentOutOfRangeException`: `'minValue' cannot be greater than maxValue. (Parameter 'minValue')` |

## `DateTime` and `DateTimeOffset` — day resolution, and a floor that surprises

Both date randomizers compute `(maxValue - minValue).Days` and add a random number of **whole days**
to `minValue`. Three consequences follow, and all three bite in practice.

**The time of day is copied from `minValue`, never randomised.** The parameterless call spans
`MinValue` to `MaxValue`, so every value it returns is at midnight with a `Kind` of `Unspecified`:

```csharp
var dates = Randomizer.GetRandomizer<DateTime>();

dates.GetRandomValue();
// e.g. 0268-10-14T00:00:00.0000000, 7116-06-01T00:00:00.0000000 — always midnight
```

That is also worth flagging against the XML summary on `DateTimeRandomizer`, which describes it as
generating "random past DateTime values (up until now)". It does not: the default range is the full
`DateTime` domain, and years far in the future are routine. Pass an explicit range whenever the
values will be persisted or displayed.

**A range shorter than a day collapses to `minValue`.** `(maxValue - minValue).Days` truncates, so
an eight-hour window has a `Days` of zero, `Random.Next(0)` returns zero, and every draw is
identical:

```csharp
var openingTime = new DateTime(2024, 1, 1, 9, 30, 0, DateTimeKind.Utc);
var closingTime = new DateTime(2024, 1, 1, 17, 0, 0, DateTimeKind.Utc);

dates.GetRandomValue(openingTime, closingTime);
// 2024-01-01T09:30:00.0000000Z — every single time
```

No exception, no warning: a fixture meaning to scatter appointments across a working day produces
the same instant for every record. Where sub-day resolution is needed, randomise the offset directly
with an `int` randomizer over minutes or ticks and add it.

**The top of the range is unreachable.** `Random.Next(range)` is exclusive, so the highest day the
generator can return is `minValue` plus `Days - 1` days. Over 100,000 draws of the range
`2024-01-01 09:30` to `2024-01-08 09:30`, the largest value observed was `2024-01-07T09:30:00Z` —
the 8th never occurred, and neither did any time after 09:30 on the 7th.

Reversing the bounds surfaces as an error about the internal day count rather than about the
arguments passed:

```csharp
var weekStart = new DateTime(2024, 1, 1, 9, 30, 0, DateTimeKind.Utc);
var weekEnd = new DateTime(2024, 1, 8, 9, 30, 0, DateTimeKind.Utc);

dates.GetRandomValue(weekEnd, weekStart);
// ArgumentOutOfRangeException: maxValue ('-7') must be a non-negative value. (Parameter 'maxValue')
```

Reversing a *sub-day* pair does not even do that. `(openingTime - closingTime).Days` truncates a
negative fraction to zero, so `dates.GetRandomValue(closingTime, openingTime)` quietly returns
`2024-01-01T17:00:00Z` — the argument passed as the *minimum*. Bad arguments are only sometimes an
exception here; validate the range before handing it over.

<xref:Ploch.Common.Randomizers.DateTimeOffsetRandomizer> behaves identically and **preserves the
offset of `minValue`**, which is the useful half of the day-resolution behaviour — a range anchored
at `+02:00` yields `+02:00` values throughout:

```csharp
var from = new DateTimeOffset(2024, 3, 1, 8, 0, 0, TimeSpan.FromHours(2));
var to = from.AddDays(30);

Randomizer.GetRandomizer<DateTimeOffset>().GetRandomValue(from, to);
// e.g. 2024-03-18T08:00:00.0000000+02:00
```

Its parameterless call spans the whole `DateTimeOffset` domain and therefore returns `+00:00`, since
`DateTimeOffset.MinValue` has a zero offset.

## There is no seeding, and therefore no reproducibility

Every shipped randomizer holds a `private readonly Random _random = new();`. There is no constructor
taking a seed, no `Seed` property and no way to reach the underlying `Random`. A failing test that
depends on generated data cannot be replayed, and a reproducible demo dataset is not achievable
through this API.

Where reproducibility matters, implement the interface over a `Random` under your own control.
<xref:Ploch.Common.Randomizers.BaseRandomizer`1> exists for exactly this: derive from it, implement
the two abstract methods, and the non-generic `IRandomizer` plumbing comes for free.

```csharp
public sealed class SeededIntRandomizer(int seed) : BaseRandomizer<int>
{
    private readonly Random _random = new(seed);

    public override int GetRandomValue() => _random.Next();

    public override int GetRandomValue(int minValue, int maxValue) => _random.Next(minValue, maxValue);
}
```

```csharp
var runOne = new SeededIntRandomizer(20260901);
var runTwo = new SeededIntRandomizer(20260901);

Enumerable.Range(0, 5).Select(_ => runOne.GetRandomValue(1, 100));   // 30, 38, 25, 64, 9
Enumerable.Range(0, 5).Select(_ => runTwo.GetRandomValue(1, 100));   // 30, 38, 25, 64, 9 — identical
```

The same pattern covers a type the factory does not know about. A `Guid` randomizer is a plausible
addition to a seeding tool, and only the ranged method needs thought — there is no sensible ordering
on `Guid`, so rejecting the call is more honest than inventing one:

```csharp
public sealed class GuidRandomizer : BaseRandomizer<Guid>
{
    public override Guid GetRandomValue() => Guid.NewGuid();

    public override Guid GetRandomValue(Guid minValue, Guid maxValue) =>
        throw new NotSupportedException("Guid values are not ordered, so a range is meaningless.");
}
```

`Randomizer.GetRandomizer<Guid>()` will still throw — the factory's `switch` is not extensible.
Register the implementation in the container, or reference the concrete type directly.

## Threading

Each randomizer owns a private, unsynchronised `Random`. `Random` documents no thread-safety
guarantee, and although a shared `IntRandomizer` driven from eight threads for 1.6 million draws of
`GetRandomValue(1, 1000)` produced no out-of-range values on .NET 8, the quality of the sequence
under concurrent access is not something the API promises.

The safe pattern is one randomizer per thread or per unit of work:

```csharp
Parallel.ForEach(batches, batch =>
{
    var numbers = Randomizer.GetRandomizer<int>();   // owned by this iteration

    foreach (var record in batch)
    {
        record.Quantity = numbers.GetRandomValue(1, 50);
    }
});
```

It also means an `AddSingleton` registration hands one unsynchronised instance to every concurrent
request. `AddTransient` — or `AddScoped` — is the safer default in a web application, at the cost of
one small allocation per resolution.

## Target-framework differences matter here

`Ploch.Common` targets `netstandard2.0` and `net8.0`. The parameterless `Random` constructor behaves
differently across those worlds, and because the factory allocates a new randomizer on **every**
call, the difference is easy to hit.

On .NET 8, instances are seeded independently, and two randomizers created back to back diverge
immediately:

```text
i1: 487,671,782,88
i2: 566,837,273,237
```

On .NET Framework — which is what consumes the `netstandard2.0` asset — `new Random()` is seeded
from the system clock. Two randomizers created within the same clock tick share a seed and produce
the **identical** sequence. Running the same code on `net48` three times in one process:

```text
i1: 877,89,212,963,183    i2: 877,89,212,963,183    identical=True
i1: 235,784,395,378,407   i2: 235,784,395,378,407   identical=True
i1: 235,784,395,378,407   i2: 235,784,395,378,407   identical=True
```

Two of those three runs are identical to each other as well, because the whole process ran inside a
single clock tick.

The practical rule is the same on both frameworks and mandatory on .NET Framework: **hoist the
randomizer out of the loop.** Calling the factory per item is the bug.

```csharp
// Wrong on .NET Framework: consecutive items can share a seed and repeat the same value.
foreach (var record in records)
{
    record.Quantity = Randomizer.GetRandomizer<int>().GetRandomValue(1, 50);
}

// Right everywhere: one generator, many draws.
var quantities = Randomizer.GetRandomizer<int>();
foreach (var record in records)
{
    record.Quantity = quantities.GetRandomValue(1, 50);
}
```

## A worked example: seeding a staging catalogue

Pulling the pieces together — a seeder that fills an empty staging database with plausible products.
Note the injected randomizers rather than factory calls, the explicit ranges with exclusive upper
bounds, the `"{"` upper character bound so `z` can actually appear in a name, and the `AcceptsOrders`
flag pinned through the equal-bounds trick while everything else varies.

```csharp
public sealed class CatalogueSeeder(
    IRangedRandomizer<int> numbers,
    IRangedRandomizer<string> names,
    IRangedRandomizer<DateTimeOffset> dates,
    IRangedRandomizer<bool> flags)
{
    public IReadOnlyList<Product> Generate(int count, DateTimeOffset earliestListing)
    {
        var latestListing = earliestListing.AddDays(365);
        var products = new List<Product>(count);

        for (var i = 0; i < count; i++)
        {
            products.Add(new Product
                         {
                             Sku = $"STG-{numbers.GetRandomValue(100_000, 1_000_000)}",
                             Name = names.GetRandomValue("a", "{"),
                             StockOnHand = numbers.GetRandomValue(0, 501),
                             ListedOn = dates.GetRandomValue(earliestListing, latestListing),
                             AcceptsOrders = flags.GetRandomValue(true, true)
                         });
        }

        return products;
    }
}
```

Because the seeder takes interfaces rather than calling the factory, a test asserting on the
mapping — rather than on the randomness — can hand it stubs and get a fully determined result. That
is the whole reason the abstraction exists.

## See also

- <xref:Ploch.Common.Randomizers> — the full API reference for the namespace.
- [Ploch.Common library guide](../../docs/libraries/common.md) — the other namespaces in the package.
- [Collections samples](./collections-samples.md) — `Shuffle` and `TakeRandom`, the other places
  randomness shows up in this package.
