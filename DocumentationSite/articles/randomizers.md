# Randomizers

The `Ploch.Common.Randomizers` namespace provides a small abstraction over random value generation:
one interface family, one factory, and five built-in implementations.

```csharp
using Ploch.Common.Randomizers;
```

Its purpose is not to be a better `System.Random` — it is to let code that *needs a random value of
some type* be written without knowing which type, and without depending on a concrete generator. That
matters for test-data builders, property-based tests, demo-data seeders and anywhere randomisation is
a strategy that must be substitutable.

## The interface family

```csharp
public interface IRandomizer
{
    object GetRandomValue();
    object GetRandomValue(object minValue, object maxValue);
}

public interface IRandomizer<out TValue> : IRandomizer
{
    new TValue GetRandomValue();
}

public interface IRangeRandomizer : IRandomizer
{
    new object GetRandomValue(object minValue, object maxValue);
}

public interface IRangedRandomizer<TValue> : IRandomizer<TValue>, IRangeRandomizer
{
    TValue GetRandomValue(TValue minValue, TValue maxValue);
}
```

The non-generic `IRandomizer` is the one to hold when the value type is only known at runtime — a
dictionary of randomizers keyed by `Type`, for example. `IRangedRandomizer<TValue>` is the one to hold
when the type is known and bounded generation is needed.

`BaseRandomizer<TValue>` implements the plumbing: it provides the `object`-based members by casting to
and from `TValue`, and throws `InvalidOperationException` if a derived randomizer ever returns `null`.
Deriving from it means implementing just two abstract methods.

## Built-in randomizers

| Type | Class | `GetRandomValue()` | `GetRandomValue(min, max)` |
|---|---|---|---|
| `string` | `StringRandomizer` | 8 characters from `A–Z`, `a–z`, `0–9` | 8 characters between the **first character** of `minChar` and of `maxChar` |
| `int` | `IntRandomizer` | `Random.Next()` | `Random.Next(minValue, maxValue)` — max exclusive |
| `bool` | `BooleanRandomizer` | `true` or `false` | equal bounds return that value, otherwise unbounded |
| `DateTime` | `DateTimeRandomizer` | between `DateTime.MinValue` and `DateTime.MaxValue` | a random **whole number of days** after `minValue` |
| `DateTimeOffset` | `DateTimeOffsetRandomizer` | between `DateTimeOffset.MinValue` and `DateTimeOffset.MaxValue` | a random whole number of days after `minValue` |

## `Randomizer` — the factory

```csharp
public static IRangedRandomizer<TValue> GetRandomizer<TValue>()
public static IRandomizer GetRandomizer(Type type)
```

Both throw `NotSupportedException` for any type outside the table above. The generic overload
additionally casts to `IRangedRandomizer<TValue>`, so it is the one to use when you know the type at
compile time:

```csharp
var stringRandomizer = Randomizer.GetRandomizer<string>();
var intRandomizer = Randomizer.GetRandomizer<int>();

var reference = stringRandomizer.GetRandomValue();      // e.g. "aQ3zR8Lk"
var quantity  = intRandomizer.GetRandomValue(1, 100);   // 1..99
```

Each call to `GetRandomizer` constructs a **new** instance, each carrying its own `Random`. Cache the
randomizer if you are generating many values.

## Generating a random value for a runtime type

The non-generic overload is what makes the abstraction worth having. A test-data builder can populate
an object graph without a `switch` over property types at every call site:

```csharp
public static class TestEntityBuilder
{
    public static TEntity CreateWithRandomValues<TEntity>() where TEntity : new()
    {
        var entity = new TEntity();

        foreach (var property in typeof(TEntity).GetProperties().Where(p => p.CanWrite))
        {
            try
            {
                var randomizer = Randomizer.GetRandomizer(property.PropertyType);
                property.SetValue(entity, randomizer.GetRandomValue());
            }
            catch (NotSupportedException)
            {
                // No randomizer for this property type — leave it at its default.
            }
        }

        return entity;
    }
}
```

Used against a domain type:

```csharp
var order = TestEntityBuilder.CreateWithRandomValues<Order>();
// order.Reference, order.Quantity, order.PlacedOn and order.IsPriority are all populated
```

> For general-purpose test-data generation, [AutoFixture](https://github.com/AutoFixture/AutoFixture)
> — wired up in this repository through
> [`Ploch.TestingSupport.XUnit3.AutoMoq`](testing-support.md) — covers far more ground.
> `Randomizers` is for the narrower case where you need an explicit, injectable strategy for one or
> two types.

## Bounded generation

```csharp
var dateRandomizer = Randomizer.GetRandomizer<DateTime>();

var deliveryDate = dateRandomizer.GetRandomValue(DateTime.Today, DateTime.Today.AddDays(30));
```

Two behaviours to be aware of when generating dates:

- The range is computed in **whole days** (`(maxValue - minValue).Days`), so any time-of-day component
  of `minValue` is carried through unchanged and sub-day ranges collapse to `minValue`. For a random
  instant within a single day, generate a random number of seconds with `IntRandomizer` and add it
  yourself.
- `GetRandomValue()` with no bounds spans `DateTime.MinValue` to `DateTime.MaxValue`, which is nearly
  10 000 years — far wider than the day count an `int` can hold precisely for every case. Always pass
  an explicit range for anything that has to look plausible.

`StringRandomizer` has an extra, non-interface overload that controls the length as well as the
character range:

```csharp
var stringRandomizer = new StringRandomizer();

var code = stringRandomizer.GetRandomValue(numberOfCharacters: 12, minChar: 'A', maxChar: 'Z');
```

Note that the character bounds are passed to `Random.Next(minChar, maxChar)`, whose upper bound is
**exclusive** — `maxChar: 'Z'` produces characters up to `'Y'`. Pass the character *after* the last one
you want. When the bounds are given through the `IRangedRandomizer<string>` interface — as
`GetRandomValue(string minChar, string maxChar)` — only the first character of each string is used,
and the result is always eight characters long.

## Writing a custom randomizer

Derive from `BaseRandomizer<TValue>` and implement the two abstract methods:

```csharp
public sealed class DecimalRandomizer : BaseRandomizer<decimal>
{
    private readonly Random _random = new();

    public override decimal GetRandomValue() => GetRandomValue(0m, 1_000m);

    public override decimal GetRandomValue(decimal minValue, decimal maxValue)
    {
        var range = maxValue - minValue;

        return minValue + (range * (decimal)_random.NextDouble());
    }
}
```

The base class supplies `GetRandomValue(object, object)` and the explicit
`IRandomizer.GetRandomValue()` implementation, both of which null-check the result.

Custom randomizers are not discoverable through the static `Randomizer` factory — it has a fixed,
closed set of supported types. Register them in your own container or map instead:

```csharp
services.AddSingleton<IRangedRandomizer<decimal>, DecimalRandomizer>();
```

## Thread safety and cryptographic strength

Each randomizer instance owns a private, **non-thread-safe** `System.Random`. Do not share a single
instance across concurrent threads; create one per thread, or guard access.

`Ploch.Common` also exposes `ThreadSafeRandom.Shared` in the root namespace — a `[ThreadStatic]`
`Random` seeded from a lock-protected global — which is what
[`Shuffle` and `TakeRandom`](collections-samples.md#randomisation--shuffle-and-takerandom) use
internally:

```csharp
var index = ThreadSafeRandom.Shared.Next(items.Count);
```

None of this is cryptographically secure. For tokens, session identifiers, password salts, nonces or
anything else whose predictability an attacker benefits from, use
`System.Security.Cryptography.RandomNumberGenerator`.

## See also

- [Collections and enumerable extensions](collections-samples.md) — `Shuffle` and `TakeRandom`
- [Testing support](testing-support.md)
- [`Ploch.Common.Randomizers` API reference](../api/Ploch.Common.Randomizers.html)
