# Collections and Enumerable Extensions

The `Ploch.Common.Collections` namespace fills the gaps that LINQ leaves open when working with
`IEnumerable<T>`, `ICollection<T>`, `IDictionary<TKey, TValue>`, arrays and `IQueryable<T>`.

```csharp
using Ploch.Common.Collections;
```

| Class | Extends | Highlights |
|---|---|---|
| `EnumerableExtensions` | `IEnumerable<T>`, `IEnumerable` | `Join`, `JoinWithFinalSeparator`, `None`, `If`, `ForEach`, `Shuffle`, `TakeRandom`, `Second`, `IsNullOrEmpty` |
| `EnumerableQueries` | `IEnumerable<T>` | `GetWithEmptyProperty` |
| `CollectionExtensions` | `ICollection<T>` | `AddMany`, `Add`/`AddIfNotNull` for key–value collections |
| `DictionaryExtensions` | `IDictionary<TKey, TValue>` | `AddMany` with duplicate-key policy |
| `ArrayExtensions` | `T[]` | `Exists` |
| `QueryableExtensions` | `IQueryable<T>` | `If` |

Every method guards its arguments with [`Ploch.Common.ArgumentChecking`](argument-validation.md), so
passing `null` produces an `ArgumentNullException` naming the offending parameter rather than a
`NullReferenceException` from somewhere deep inside a LINQ pipeline.

---

## Conditional query composition — `If`

`If` is the single most useful method in the namespace. It applies a query transformation only when a
condition holds, which removes the branching that optional filters normally force on you.

```csharp
public static IEnumerable<T> If<T>(this IEnumerable<T> enumerable, bool condition, Func<IEnumerable<T>, IEnumerable<T>> action)
public static IQueryable<T> If<T>(this IQueryable<T> queryable, bool condition, Func<IQueryable<T>, IQueryable<T>> action)
```

The classic case is a search endpoint where every filter is optional. Without `If`, the query has to
be built up through a mutable local and a stack of `if` statements:

```csharp
// Before
var query = dbContext.Orders.AsQueryable();

if (customerId.HasValue)
{
    query = query.Where(order => order.CustomerId == customerId.Value);
}

if (!string.IsNullOrEmpty(status))
{
    query = query.Where(order => order.Status == status);
}

if (placedAfter.HasValue)
{
    query = query.Where(order => order.PlacedOn >= placedAfter.Value);
}

return await query.OrderByDescending(order => order.PlacedOn).Take(pageSize).ToListAsync();
```

With `If` the whole thing stays a single expression:

```csharp
// After
return await dbContext.Orders
                      .If(customerId.HasValue, q => q.Where(order => order.CustomerId == customerId!.Value))
                      .If(!string.IsNullOrEmpty(status), q => q.Where(order => order.Status == status))
                      .If(placedAfter.HasValue, q => q.Where(order => order.PlacedOn >= placedAfter!.Value))
                      .OrderByDescending(order => order.PlacedOn)
                      .Take(pageSize)
                      .ToListAsync();
```

Because the `IQueryable<T>` overload returns `IQueryable<T>`, the expression tree is preserved and the
filters are still translated into SQL by Entity Framework Core — nothing is pulled into memory.

The `IEnumerable<T>` overload behaves identically for in-memory sequences:

```csharp
var visibleItems = allItems.If(!includeArchived, items => items.Where(item => !item.IsArchived))
                           .If(onlyFavourites, items => items.Where(item => item.IsFavourite));
```

---

## Building strings from sequences

### `Join`

```csharp
public static string Join<TValue>(this IEnumerable<TValue> source, string separator)
public static string Join<TValue, TResult>(this IEnumerable<TValue> source, string separator, Func<TValue, TResult> valueSelector)
```

`Join` is `string.Join` in the position where it reads naturally — at the end of a LINQ chain rather
than wrapped around it. The second overload projects each element first, so no intermediate `Select`
is needed:

```csharp
var header = request.Headers.Join("; ");

var auditLine = order.Lines.Join(", ", line => $"{line.Quantity}x {line.ProductCode}");
// "2x SKU-1001, 1x SKU-2043"
```

### `JoinWithFinalSeparator`

```csharp
public static string JoinWithFinalSeparator<TValue>(this IEnumerable<TValue> source, string separator, string finalSeparator)
public static string JoinWithFinalSeparator<TValue, TResult>(this IEnumerable<TValue> source, string separator, string finalSeparator, Func<TValue, TResult> valueSelector)
```

For user-facing prose, the last separator normally differs from the rest. This is exactly what
validation and notification messages need:

```csharp
var missing = new[] { "e-mail address", "postcode", "date of birth" };

var message = $"Please supply your {missing.JoinWithFinalSeparator(", ", " and ")}.";
// "Please supply your e-mail address, postcode and date of birth."
```

With a projection:

```csharp
var recipients = subscribers.JoinWithFinalSeparator(", ", " and ", subscriber => subscriber.DisplayName);
```

---

## Membership checks — `ValueIn`

```csharp
public static bool ValueIn<TValue>(this TValue value, params TValue[] values)
public static bool ValueIn<TValue>(this TValue value, IEqualityComparer<TValue>? comparer, params TValue[] values)
public static bool ValueIn<TValue>(this TValue value, IEnumerable<TValue> values, IEqualityComparer<TValue>? comparer = null)
```

`ValueIn` reads left to right — the value under test comes first, the candidates follow — which is
the opposite of `Contains` and much closer to the way the condition is spoken:

```csharp
if (response.StatusCode.ValueIn(HttpStatusCode.BadGateway,
                                HttpStatusCode.ServiceUnavailable,
                                HttpStatusCode.GatewayTimeout))
{
    return await RetryAsync(request, cancellationToken);
}
```

The comparer overload is the one to reach for with strings from external systems, where casing is
never guaranteed:

```csharp
var isPrivileged = user.Role.ValueIn(StringComparer.OrdinalIgnoreCase, "Administrator", "Owner");
```

> `Ploch.Common` also provides `In` and `NotIn` in the root `Ploch.Common` namespace
> (`IsInExtensions`), which offer the same idea with `IComparer<T>` overloads.

---

## Emptiness and positional access

```csharp
public static bool None<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
public static bool IsEmpty<T>(this IEnumerable<T> enumerable)
public static bool IsEmpty(this IEnumerable enumerable)
public static bool IsNullOrEmpty<T>(this IEnumerable<T>? enumerable)
public static T Second<T>(this IEnumerable<T> enumerable)
```

`None` states the negative condition directly instead of forcing the reader to notice a leading `!`
several tokens away from the predicate:

```csharp
if (basket.Lines.None(line => line.Quantity > 0))
{
    return Result.Invalid("The basket contains no orderable lines.");
}
```

`IsNullOrEmpty` is the `IEnumerable<T>` counterpart of `string.IsNullOrEmpty` and is safe to call on a
`null` reference, which makes it ideal at an API boundary where a collection property may simply be
absent from the payload:

```csharp
if (request.Tags.IsNullOrEmpty())
{
    request.Tags = DefaultTags;
}
```

There is also a non-generic `IsEmpty(this IEnumerable)` overload for legacy collection types such as
`ArrayList` or an untyped `IDictionary`. It advances the enumerator exactly once and disposes it if it
is disposable.

`Second` returns the element at index 1, throwing `InvalidOperationException` when the sequence has
fewer than two elements — the same failure mode as `First()`:

```csharp
var runnerUp = leaderboard.OrderByDescending(entry => entry.Score).Second();
```

### `AreIntegersSequentialInOrder`

```csharp
public static bool AreIntegersSequentialInOrder(this IEnumerable<int> enumerable)
public static bool AreIntegersSequentialInOrder(this IEnumerable<long> enumerable)
```

Verifies that each element is exactly one greater than its predecessor. Useful when validating that a
chunked upload, an event stream or a page of sequence numbers has no gaps:

```csharp
var sequenceNumbers = receivedChunks.Select(chunk => chunk.SequenceNumber).OrderBy(number => number);

if (!sequenceNumbers.AreIntegersSequentialInOrder())
{
    throw new InvalidOperationException("The upload is missing one or more chunks.");
}
```

Note that an empty sequence and a single-element sequence both return `true`.

---

## `ForEach` — side effects that stay in the chain

```csharp
public static IEnumerable<T> ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
```

Unlike `List<T>.ForEach`, this overload **returns the source sequence**, so it composes:

```csharp
importedRows.ForEach(row => _logger.LogDebug("Importing {RowId}", row.Id))
            .Select(MapToEntity)
            .ForEach(entity => _validator.ValidateAndThrow(entity));
```

`ForEach` enumerates eagerly, so it is safe to use as the terminal step of a pipeline without an
additional `ToList()`.

---

## Randomisation — `Shuffle` and `TakeRandom`

```csharp
public static IEnumerable<TValue> Shuffle<TValue>(this IEnumerable<TValue> source)
public static IEnumerable<TValue> TakeRandom<TValue>(this IEnumerable<TValue> source, int count)
```

Both use a Fisher–Yates shuffle backed by `ThreadSafeRandom.Shared`, so they are safe to call
concurrently from multiple request threads. `TakeRandom` performs only a *partial* shuffle — O(n)
setup and O(`count`) selection — with uniform probability and no duplicate picks.

```csharp
// Rotate the "featured products" panel on every page render.
var featured = catalogue.AvailableProducts.TakeRandom(4);

// Randomise question order in an assessment.
var questions = quiz.Questions.Shuffle().ToList();
```

`TakeRandom` clamps `count` to the size of the source, and returns an empty sequence for a
zero or negative `count` without enumerating the source — matching `Enumerable.Take` semantics.

> These are deliberately **not** cryptographically secure. Use `System.Security.Cryptography.RandomNumberGenerator`
> for tokens, nonces or anything else an attacker benefits from predicting.

---

## `ExceptItems`

```csharp
public static IEnumerable<TItem> ExceptItems<TItem>(this IEnumerable<TItem> source, params TItem[] itemsToRemove)
```

`Enumerable.Except` requires a second *sequence*; `ExceptItems` accepts loose arguments, which removes
the array literal at the call site:

```csharp
var exportableColumns = allColumns.ExceptItems("PasswordHash", "SecurityStamp", "ConcurrencyToken");
```

---

## `GetWithEmptyProperty`

```csharp
public static IEnumerable<T> GetWithEmptyProperty<T>(this IEnumerable<T> items, Func<T, string?> propertySelector)
```

Selects the items whose chosen string property is `null`, empty **or white space**. This is the shape
of a data-quality report:

```csharp
var incompleteContacts = contacts.GetWithEmptyProperty(contact => contact.EmailAddress).ToList();

if (incompleteContacts.Count > 0)
{
    _logger.LogWarning("{Count} contacts have no e-mail address: {Ids}",
                       incompleteContacts.Count,
                       incompleteContacts.Join(", ", contact => contact.Id));
}
```

---

## Adding many items — `AddMany`

`ICollection<T>` has `Add`, but no bulk equivalent, and `IDictionary<TKey, TValue>` throws on a
duplicate key with no way to say "overwrite" or "skip". `AddMany` addresses both, with an explicit
`DuplicateHandling` policy.

```csharp
public enum DuplicateHandling
{
    Ignore,     // keep the existing entry, skip the incoming one
    Overwrite,  // replace the existing entry
    Throw       // throw ArgumentException (the default)
}
```

### Collections

```csharp
public static TCollection AddMany<TCollection, TItem>(this TCollection collection, IEnumerable<TItem> items, DuplicateHandling duplicateHandling = DuplicateHandling.Throw)
    where TCollection : ICollection<TItem>

public static ICollection<TItem> AddMany<TCollection, TItem>(this TCollection collection, DuplicateHandling duplicateHandling = DuplicateHandling.Throw, params TItem[] items)
    where TCollection : ICollection<TItem>
```

The sequence overload returns `TCollection`, preserving the concrete type for further chaining:

```csharp
var permissions = new HashSet<string>()
    .AddMany(rolePermissions, DuplicateHandling.Ignore)
    .AddMany(userPermissions, DuplicateHandling.Ignore);
```

Note the parameter order in the `params` overload: the policy comes **before** the items, because
`params` must be last.

```csharp
var tags = new List<string>();
tags.AddMany(DuplicateHandling.Ignore, "urgent", "customer-facing", "urgent");
// tags: [ "urgent", "customer-facing" ]
```

### Dictionaries

```csharp
public static IDictionary<TKey, TValue> AddMany<TDictionary, TKey, TValue>(this TDictionary dictionary, IEnumerable<KeyValuePair<TKey, TValue>> items, DuplicateHandling duplicateHandling = DuplicateHandling.Throw)
```

Layered configuration is the natural example — later sources win:

```csharp
var settings = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    .AddMany(defaultSettings)
    .AddMany(environmentSettings, DuplicateHandling.Overwrite)
    .AddMany(commandLineSettings, DuplicateHandling.Overwrite);
```

With the default `DuplicateHandling.Throw`, a duplicate key raises `ArgumentException` naming the key,
which is what you want when two configuration sources are not supposed to overlap.

### Key–value collection helpers

```csharp
public static ICollection<KeyValuePair<TKey, TValue?>> Add<TKey, TValue>(this ICollection<KeyValuePair<TKey, TValue?>> collection, TKey key, TValue? value)
public static ICollection<KeyValuePair<TKey, TValue?>> AddIfNotNull<TKey, TValue>(this ICollection<KeyValuePair<TKey, TValue?>> collection, TKey key, TValue? value)
    where TValue : class?
```

These target collections of key–value pairs that are *not* dictionaries — HTTP header collections,
query-string builders and similar. `AddIfNotNull` skips the entry entirely when the value is `null`,
which is precisely the behaviour wanted when building an outbound request from optional inputs:

```csharp
var queryParameters = new List<KeyValuePair<string, string?>>();

queryParameters.Add("page", pageNumber.ToString(CultureInfo.InvariantCulture))
               .AddIfNotNull("search", searchTerm)
               .AddIfNotNull("category", categoryFilter);
```

`AddIfNotNull` is constrained to reference types (`where TValue : class?`). For an optional value type,
project it to its string form first, or use `Add` with a pre-checked value.

---

## `ArrayExtensions.Exists`

```csharp
public static bool Exists<TItem>(this TItem[] array, Predicate<TItem> predicate)
```

A guarded, extension-method form of the static `Array.Exists`, which avoids the LINQ allocation of
`Any()` on hot paths where the receiver is known to be an array:

```csharp
var hasErrorRow = importedRows.Exists(row => row.Severity == Severity.Error);
```

---

## See also

- [Argument validation](argument-validation.md) — the guards used throughout these methods
- [Strings, parsing and text building](strings.md)
- [`Ploch.Common.Collections` API reference](../api/Ploch.Common.Collections.html)
