# Collections and enumerable extensions

LINQ is very good at transforming a sequence and says almost nothing about everything that surrounds
one. The result is a familiar set of small frictions: a negated `Any()` that reads backwards, a cascade
of `if` statements to bolt optional filters onto a query, a `foreach` that breaks a fluent chain purely
to cause a side effect, a hand-rolled loop to join a list with "and" before the last item, and a choice
between `Add` throwing and `[key] =` silently overwriting when merging two dictionaries.

<xref:Ploch.Common.Collections> covers exactly those shapes, for `IEnumerable<T>`, `IQueryable<T>`,
`ICollection<T>`, `IDictionary<TKey, TValue>` and arrays. Nothing here replaces LINQ — every method is a
thin, well-behaved extension that composes with it.

Reach for this namespace when you are writing the code *around* a query: validating an inbound batch at a
service boundary, assembling a repository search from a request object full of nullable parameters,
formatting a collection into a message a person will read, or seeding a collection from a configuration
source. Reach for plain LINQ when you are transforming the data itself.

> [!NOTE]
> This article is a scenario-led companion to [Collections Samples](collections-samples.md), which is a
> per-method reference for `EnumerableExtensions` alone.

## Installation

```powershell
dotnet add package Ploch.Common
```

Everything below lives in a single namespace:

```csharp
using Ploch.Common.Collections;
```

The library targets `netstandard2.0` and `net8.0`, so it is usable from .NET Framework as well as modern
.NET. See the [Ploch.Common library guide](../../docs/libraries/common.md) for the wider package.

## At a glance

| Type | Members | Use it for |
| --- | --- | --- |
| <xref:Ploch.Common.Collections.EnumerableExtensions> | `If`, `None`, `ForEach`, `Join`, `JoinWithFinalSeparator`, `ValueIn`, `IsEmpty`, `IsNullOrEmpty`, `Second`, `ExceptItems`, `Shuffle`, `TakeRandom`, `AreIntegersSequentialInOrder` | The bulk of the namespace: `IEnumerable<T>` ergonomics |
| <xref:Ploch.Common.Collections.QueryableExtensions> | `If` | The `IQueryable<T>` counterpart, so conditional filters stay translatable to SQL |
| <xref:Ploch.Common.Collections.CollectionExtensions> | `AddMany`, `Add`, `AddIfNotNull` | Bulk and conditional population of `ICollection<T>` |
| <xref:Ploch.Common.Collections.DictionaryExtensions> | `AddMany` | Merging key/value pairs into a dictionary with an explicit duplicate policy |
| <xref:Ploch.Common.Collections.DuplicateHandling> | `Ignore`, `Overwrite`, `Throw` | The duplicate policy shared by both `AddMany` families |
| <xref:Ploch.Common.Collections.ArrayExtensions> | `Exists` | `Array.Exists` as an extension method |
| <xref:Ploch.Common.Collections.EnumerableQueries> | `GetWithEmptyProperty` | Data-quality sweeps for missing string values |

## Building a query from optional filters

This is the highest-value member of the namespace and the reason most teams adopt it.

A search endpoint typically receives a request object where nearly every property is optional, and each
one must become a `Where` clause *only when supplied*. Written imperatively that becomes a run of
reassignments which pushes the shape of the query out of view:

```csharp
public IQueryable<Invoice> SearchWithoutIf(InvoiceSearch search)
{
    var query = store.Invoices;
    if (search.CustomerId is not null)
    {
        query = query.Where(i => i.CustomerId == search.CustomerId);
    }

    if (search.IssuedAfter.HasValue)
    {
        query = query.Where(i => i.IssuedOn >= search.IssuedAfter.Value);
    }

    return query;
}
```

`If` keeps the whole query as one expression, with the condition sitting immediately beside the clause it
guards:

```csharp
public IQueryable<Invoice> Search(InvoiceSearch search) =>
    store.Invoices
         .If(search.CustomerId is not null, q => q.Where(i => i.CustomerId == search.CustomerId))
         .If(search.IssuedAfter.HasValue, q => q.Where(i => i.IssuedOn >= search.IssuedAfter!.Value))
         .If(search.IssuedBefore.HasValue, q => q.Where(i => i.IssuedOn <= search.IssuedBefore!.Value))
         .If(!search.Statuses.IsNullOrEmpty(), q => q.Where(i => search.Statuses!.Contains(i.Status)))
         .OrderByDescending(i => i.IssuedOn)
         .If(search.Take.HasValue, q => q.Take(search.Take!.Value));
```

Three things make this safe rather than merely tidy:

- **The `IQueryable<T>` overload returns `IQueryable<T>`.** The callback receives and returns the
  queryable, so the expression tree is built exactly as if you had written the `Where` inline, and Entity
  Framework Core translates it to SQL unchanged. Nothing is buffered into memory.
- **`If` is not deferred.** The condition is evaluated the moment the method is called, and the callback
  either runs or does not. Only the *query* it produces is deferred, in the usual LINQ way.
- **The callback is not restricted to `Where`.** `Take`, `Skip`, `OrderBy`, `Include` — anything with the
  right shape composes, which is why the `Take` clause above sits after `OrderByDescending`.

The same method exists for `IEnumerable<T>`, for filtering an in-memory collection:

```csharp
public static IEnumerable<Invoice> Filter(IEnumerable<Invoice> invoices, decimal? minimumTotal, bool includePaid) =>
    invoices.If(minimumTotal.HasValue, source => source.Where(i => i.Total >= minimumTotal!.Value))
            .If(!includePaid, source => source.Where(i => i.Status != "Paid"));
```

Both overloads throw `ArgumentNullException` if the source or the callback is `null`.

> [!IMPORTANT]
> Pick the overload that matches the static type of your source. If an `IQueryable<T>` is held in a
> variable typed as `IEnumerable<T>`, the `IEnumerable<T>` overload binds, the callback takes an
> `IEnumerable<T>`, and the filter runs client-side after the whole table has been fetched. This is the
> ordinary LINQ overload-resolution trap, not something `If` introduces — but `If` makes it easy to miss.

## Guarding a service boundary

`None`, `IsEmpty`, `IsNullOrEmpty` and `ValueIn` together let a validator read as a list of statements
about the input rather than a pile of negations:

```csharp
public sealed class InvoiceBatchValidator
{
    private static readonly HashSet<string> SettleableStatuses =
        new(StringComparer.OrdinalIgnoreCase) { "Approved", "PartiallyPaid" };

    public IReadOnlyList<string> Validate(IReadOnlyCollection<Invoice>? batch)
    {
        if (batch.IsNullOrEmpty())
        {
            return ["The batch contained no invoices."];
        }

        var problems = new List<string>();

        if (batch!.None(i => i.Status.ValueIn(SettleableStatuses)))
        {
            problems.Add("No invoice in the batch is in a settleable state.");
        }

        var unreferenced = batch!.GetWithEmptyProperty(i => i.CustomerReference).ToList();
        if (!unreferenced.IsEmpty())
        {
            problems.Add($"Missing customer reference on: {unreferenced.Join(", ", i => i.Reference)}.");
        }

        return problems;
    }
}
```

Points worth knowing:

- **`None(predicate)` is `All(x => !predicate(x))`,** so an empty sequence returns `true` — the same
  vacuous-truth convention as `All`. It short-circuits on the first match.
- **`IsNullOrEmpty` accepts a nullable source; `IsEmpty` does not.** `IsEmpty` guards its argument and
  throws `ArgumentNullException` on `null`, so use it once you already know the reference is non-null,
  and `IsNullOrEmpty` at the boundary where you do not.
- **`IsNullOrEmpty` does not carry a `[NotNullWhen(false)]` annotation,** so the compiler will not narrow
  nullability after the check. That is why `batch!` appears above. If you prefer to avoid the
  null-forgiving operator, assign to a local inside the guard instead.
- **There is a non-generic `IsEmpty(this IEnumerable)`** for legacy APIs that hand back `ArrayList`,
  `IDictionary` or an untyped `IEnumerable`. It disposes the enumerator if it implements `IDisposable`.

```csharp
public static bool NonGenericIsEmpty(IEnumerable legacyList) => legacyList.IsEmpty();
```

## Membership tests with `ValueIn`

`ValueIn` inverts the reading order of `Contains`: the interesting value comes first and the candidate set
follows, which suits guard clauses and switch-like conditions.

```csharp
public static bool IsRetryableStatusCode(int statusCode) => statusCode.ValueIn(408, 429, 502, 503, 504);

public static bool IsWriteMethod(string method) =>
    method.ValueIn(StringComparer.OrdinalIgnoreCase, "POST", "PUT", "PATCH", "DELETE");

public static bool IsKnownStatus(string status, IEnumerable<string> known) =>
    status.ValueIn(known, StringComparer.OrdinalIgnoreCase);
```

There are three overloads, and the comparer sits in a different position in each:

| Signature | Comparer position |
| --- | --- |
| `ValueIn<TValue>(this TValue value, params TValue[] values)` | none — default comparer |
| `ValueIn<TValue>(this TValue value, IEqualityComparer<TValue>? comparer, params TValue[] values)` | **before** the `params` array, because a `params` array must come last |
| `ValueIn<TValue>(this TValue value, IEnumerable<TValue> values, IEqualityComparer<TValue>? comparer = null)` | **after** the sequence, as an optional argument |

The behaviour that surprises people:

> [!WARNING]
> When no comparer is supplied **and** the candidate set is an `ICollection<T>`, `ValueIn` delegates to
> `ICollection<T>.Contains`, so the *collection's own* comparer decides. Pass a comparer explicitly to
> override it.

```csharp
var caseInsensitive = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "Approved" };

// No comparer supplied and the source is an ICollection<T>, so the set's own
// comparer decides: this is true.
var matched = "APPROVED".ValueIn(caseInsensitive);

// Supplying a comparer explicitly bypasses ICollection<T>.Contains and applies
// the comparer to every element: this is false.
var notMatched = "APPROVED".ValueIn(caseInsensitive, StringComparer.Ordinal);
```

`TValue` is unconstrained, so `ValueIn` is `null`-tolerant on both sides: a `null` value is found in a set
that contains `null`, and is simply not found in one that does not. Only the *candidate set* itself may
not be `null`.

## Formatting a collection for a human

Log lines, validation messages and audit trails all want a collection rendered as prose. `Join` covers the
plain case, `JoinWithFinalSeparator` covers the case where the last separator differs.

```csharp
public static string DescribeRejection(IReadOnlyCollection<Invoice> rejected) =>
    $"Rejected {rejected.Count} invoices: {rejected.JoinWithFinalSeparator(", ", " and ", i => i.Reference)}.";

public static string AuditLine(Invoice invoice, IEnumerable<string> changedFields) =>
    $"{invoice.Reference}: changed {changedFields.Join(", ")}";
```

Both methods have a projecting overload taking a `Func<TValue, TResult>`, which saves a `Select` and, more
usefully, avoids relying on a type's `ToString()` for output a user will see. Without a selector, `Join`
calls `ToString()` on each element and renders `null` elements as an empty string.

The boundary cases are defined and tested, so you do not need to special-case a short list:

```csharp
string[] none = [];
string[] one = ["INV-001"];
string[] two = ["INV-001", "INV-002"];

none.JoinWithFinalSeparator(", ", " and ");  // ""
one.JoinWithFinalSeparator(", ", " and ");   // "INV-001"
two.JoinWithFinalSeparator(", ", " and ");   // "INV-001 and INV-002"
```

`JoinWithFinalSeparator` needs the element count, so it materialises the sequence unless it is already an
`IReadOnlyList<T>`. Passing a `List<T>` or an array avoids the copy.

## Side effects without leaving the chain

`ForEach` runs an action over every element and returns **the same enumerable**, so it can sit inside a
fluent chain.

```csharp
public static IReadOnlyList<Invoice> Stamp(IReadOnlyList<Invoice> invoices, DateTimeOffset now)
{
    invoices.ForEach(i => i.IssuedOn = now);

    return invoices;
}
```

Two properties follow from "the same enumerable", and both matter:

- **`ForEach` is eager.** It iterates immediately, unlike `Select`, which does not run until enumerated.
  Nothing is lost if you ignore the return value.
- **`ForEach` returns the source, not a buffered copy.** If the source is a lazy query, the returned value
  is that same lazy query, and enumerating it again re-executes it. Against a database or a generator that
  means the work happens twice — and if the action mutated objects, the second pass sees fresh instances
  that were never touched.

```csharp
public static List<Invoice> MaterialiseFirst(IEnumerable<Invoice> query)
{
    // Materialise before ForEach: the source is walked once, and the side effect
    // is applied to the objects the caller keeps.
    var materialised = query.ToList();
    materialised.ForEach(i => i.Status = "Queued");

    return materialised;
}
```

The rule of thumb: call `ToList()` before `ForEach` whenever the source is anything other than an
already-materialised collection.

## Populating collections and dictionaries

`AddMany` adds a range to any `ICollection<T>` — `List<T>`, `HashSet<T>`, `ObservableCollection<T>` — with
an explicit policy for what happens on a duplicate, chosen from
<xref:Ploch.Common.Collections.DuplicateHandling>:

| Value | Behaviour |
| --- | --- |
| `Throw` | throws `ArgumentException` on the first duplicate. **This is the default.** |
| `Ignore` | skips the duplicate, leaving the existing entry untouched |
| `Overwrite` | replaces the existing entry with the incoming one |

```csharp
public static ISet<string> CollectTags(IEnumerable<string> discovered)
{
    var tags = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "invoice" };
    tags.AddMany(discovered, DuplicateHandling.Ignore);

    return tags;
}
```

> [!NOTE]
> The default being `Throw` is deliberate: a silent duplicate is usually a bug in the data. Pass `Ignore`
> explicitly when duplicates are expected, so the intent is visible at the call site.

```csharp
var seen = new List<string> { "INV-001" };

// DuplicateHandling.Throw is the default and is shown here for clarity.
seen.AddMany(DuplicateHandling.Throw, "INV-002", "INV-001");
// ArgumentException: Item INV-001 already exists in the collection.
```

There is a `params` overload for literal item lists; note that because a `params` array must come last,
the duplicate-handling argument moves *in front of* the items — the mirror image of the sequence overload,
where it comes last and defaults.

The dictionary version merges a sequence of key/value pairs, keying duplicates by `ContainsKey`:

```csharp
public static IDictionary<string, decimal> MergeOverrides(Dictionary<string, decimal> defaults,
                                                          IEnumerable<KeyValuePair<string, decimal>> overrides) =>
    defaults.AddMany(overrides, DuplicateHandling.Overwrite);
```

It is generic over the dictionary type, so it works with anything implementing
`IDictionary<TKey, TValue>` — including `ConcurrentDictionary<TKey, TValue>` and `SortedDictionary<TKey, TValue>`:

```csharp
public static IDictionary<string, int> Seed(ConcurrentDictionary<string, int> counters) =>
    counters.AddMany(new Dictionary<string, int> { ["approved"] = 0, ["rejected"] = 0 }, DuplicateHandling.Ignore);
```

> [!WARNING]
> `AddMany` is **not** an atomic merge. Per pair it tests `ContainsKey` and then calls `Add` (or the
> indexer) as two separate operations, so on a `ConcurrentDictionary<TKey, TValue>` two threads can both
> observe the same key as absent and one will then throw from `Add` — despite `DuplicateHandling.Ignore` —
> and an `Overwrite` can clobber a value another thread wrote in between. Use it on a `ConcurrentDictionary`
> only for single-threaded seeding, as above; for a genuinely concurrent merge use that type's own atomic
> `TryAdd`, `GetOrAdd` or `AddOrUpdate`.

`Add` and `AddIfNotNull` operate on `ICollection<KeyValuePair<TKey, TValue?>>` — which every
`IDictionary<TKey, TValue?>` satisfies — and return the collection, so a set of optional entries becomes
one chain instead of a run of `if` blocks:

```csharp
public static IDictionary<string, string> BuildRequestHeaders(string? correlationId, string? tenantId)
{
    var headers = new Dictionary<string, string?>();
    headers.AddIfNotNull("X-Correlation-Id", correlationId)
           .AddIfNotNull("X-Tenant-Id", tenantId)
           .Add("Accept", "application/json");

    // AddIfNotNull has already dropped the null entries, so no filtering is needed here.
    return headers.ToDictionary(kvp => kvp.Key, kvp => kvp.Value!);
}
```

`AddIfNotNull` is constrained to reference types (`where TValue : class?`). For an optional value type,
check `HasValue` and call `Add` yourself.

## Sampling and shuffling

`Shuffle` returns the source in random order; `TakeRandom` draws a sample *without replacement* — it never
picks the same **position** twice. Both use a Fisher–Yates shuffle over a private copy of the source, so the
original is never mutated.

```csharp
public static IReadOnlyList<Invoice> PickForManualAudit(IReadOnlyCollection<Invoice> settled, int sampleSize) =>
    settled.TakeRandom(sampleSize).ToList();

public static IEnumerable<Uri> ShuffledEndpoints(IEnumerable<Uri> replicas) => replicas.Shuffle();
```

`TakeRandom` is total — it never throws for an awkward count:

| `count` | Result |
| --- | --- |
| zero or negative | empty sequence, and the source is not enumerated at all |
| greater than the source size | every item, in random order |
| within range | exactly `count` items, drawn from `count` distinct positions, uniformly selected |

> [!IMPORTANT]
> "Without replacement" is about *positions*, not *values*. `TakeRandom` performs no de-duplication, so a
> source holding equal values can return them more than once: `new[] { 1, 1, 2 }.TakeRandom(2)` yields
> `[1, 1]` about a third of the time, because positions `0` and `1` are two distinct draws that happen to
> hold the same value.
>
> Because every *position* is equally likely, a value's chance of being drawn is proportional to how often
> it occurs: in a ten-item source where one value appears five times and five others appear once each, the
> repeated value is five times likelier to be drawn than any one of the singletons. That weighting is often
> what you want — it is how a sample stays representative of the source.
>
> When you need distinct *values* instead, de-duplicate first — `source.Distinct().TakeRandom(count)`. Note
> that this **removes** the occurrence weighting: after `Distinct()` the repeats are gone, so every
> surviving value is equally likely to be drawn, regardless of how often it appeared in the original source.

A `null` source throws `ArgumentNullException`. Both methods buffer the source into a list, so they are
`O(n)` in memory and unusable on an infinite sequence.

> [!CAUTION]
> Both methods use `System.Random`, not a cryptographic generator. They are appropriate for load
> balancing, audit sampling, test-data ordering and shuffling a playlist. They are **not** appropriate for
> generating tokens, nonces, shuffling anything an adversary benefits from predicting, or any other
> security-sensitive purpose — use `System.Security.Cryptography.RandomNumberGenerator` there.

## Smaller helpers

**`GetWithEmptyProperty`** filters to items whose selected string property is `null`, empty **or
white-space** — a `string.IsNullOrWhiteSpace` test, not `IsNullOrEmpty`. It is built for data-quality
sweeps over imported records:

```csharp
var unreferenced = batch.GetWithEmptyProperty(i => i.CustomerReference);
```

**`AreIntegersSequentialInOrder`** checks that each element is exactly one greater than the one before,
which is the usual test for "did we receive every page / sequence number, in order". Overloads exist for
`IEnumerable<int>` and `IEnumerable<long>` only, and an empty or single-element sequence returns `true`:

```csharp
public static bool PagesAreContiguous(IEnumerable<int> pageNumbers) => pageNumbers.AreIntegersSequentialInOrder();
```

> [!WARNING]
> The successor test is evaluated in **unchecked** arithmetic, so "exactly one greater" is one greater
> *modulo the integer range* rather than mathematically. At the numeric boundary the comparison therefore
> wraps: both `new[] { int.MaxValue, int.MinValue }` and `new[] { long.MaxValue, long.MinValue }` are
> reported as sequential. This only matters for sequence numbers that are allowed to roll over — page
> numbers and record counters never reach the boundary. If a wrapped sequence must be rejected in your
> domain, range-check the endpoints yourself before calling.

**`Second`** returns the second element, and throws `InvalidOperationException` if the sequence has fewer
than two — the same contract as `First`:

```csharp
public static string RunnerUp(IEnumerable<string> ranked) => ranked.Second();
```

**`ExceptItems`** is `Except` with a `params` array, which removes the `new[] { ... }` ceremony from
excluding a couple of known values:

```csharp
public static IEnumerable<string> WithoutSystemAccounts(IEnumerable<string> accounts) =>
    accounts.ExceptItems("system", "anonymous");
```

> [!WARNING]
> Because it delegates to `Except`, `ExceptItems` inherits set semantics and **removes duplicates from the
> result**. If duplicates in the source are significant, use `Where(x => !x.ValueIn(...))` instead.

```csharp
string[] source = ["a", "a", "b", "c"];
var result = source.ExceptItems("c").ToList(); // ["a", "b"] - not ["a", "a", "b"]
```

**`ArrayExtensions.Exists`** exposes the static `Array.Exists` as an extension method, so an array check
composes like every other predicate in a chain:

```csharp
public static bool HasOverdue(Invoice[] invoices, DateTimeOffset cutoff) =>
    invoices.Exists(i => i.IssuedOn < cutoff && i.Status != "Paid");
```

## Enumeration behaviour, summarised

Deciding whether a method walks your sequence — and how many times — matters most when the source is a
database query or a generator.

| Method | Enumerates the source |
| --- | --- |
| `If` | never itself; it returns whatever the callback returns |
| `None` | once, short-circuiting on the first match |
| `IsEmpty`, `IsNullOrEmpty` | one element at most |
| `ForEach` | once, **eagerly**, then returns the same (possibly lazy) source |
| `Join` | once |
| `JoinWithFinalSeparator` | once; buffers unless the source is already an `IReadOnlyList<T>` |
| `ValueIn` | once, short-circuiting; delegates to `ICollection<T>.Contains` when it can |
| `Shuffle`, `TakeRandom` | once, buffering the whole source into a list |
| `AreIntegersSequentialInOrder` | once, buffering into an array |
| `Second` | at most two elements |
| `ExceptItems` | deferred — inherits `Except` semantics |
| `GetWithEmptyProperty` | deferred — inherits `Where` semantics |

## Argument guarding is not uniform

Most of these methods validate their arguments with the same
[argument-checking helpers](../../docs/libraries/common.md) as the rest of `Ploch.Common`, throwing
`ArgumentNullException` that names their own parameter: `None`, `ValueIn`, `If`, `ForEach`, `Second`,
`IsEmpty`, `TakeRandom`, `AreIntegersSequentialInOrder`, `JoinWithFinalSeparator`, `AddMany`, `Add`,
`AddIfNotNull` and `ArrayExtensions.Exists`. (`IsNullOrEmpty` is the deliberate exception: accepting `null`
is the whole point of it, and it returns `true`.)

Five overloads across four methods add no guard of their own and inherit whatever the BCL call underneath
does. That still throws in every case but one — but under *LINQ's* parameter name rather than theirs:

| Method | Null argument | What actually happens |
| --- | --- | --- |
| `Shuffle` | `source` | `ArgumentNullException`, `ParamName` `"source"` — thrown by `ToList`, and right only because the names coincide |
| `Join` (both overloads) | `source` | `ArgumentNullException`, `ParamName` `"source"` — thrown by `Select`; likewise a coincidence |
| `Join` (selector overload) | `valueSelector` | `ArgumentNullException`, `ParamName` **`"selector"`** — `Select`'s parameter, not `Join`'s |
| `ExceptItems` | `source` | `ArgumentNullException`, `ParamName` **`"first"`** — `Except`'s parameter |
| `ExceptItems` | `itemsToRemove` | `ArgumentNullException`, `ParamName` **`"second"`** |
| `GetWithEmptyProperty` | `items` | `ArgumentNullException`, `ParamName` **`"source"`** — `Where`'s parameter |
| `GetWithEmptyProperty` | `propertySelector` | **`NullReferenceException`, deferred** — the null selector is captured by the `Where` predicate, so nothing fails until the query is enumerated |

The last row is the one worth remembering: it is the only case that yields neither an
`ArgumentNullException` nor a failure at the call site. None of this affects correct calling code — it
matters only if you catch `ArgumentNullException` around these calls or branch on `ParamName`.

## See also

- [Collections Samples](collections-samples.md) — per-method reference for `EnumerableExtensions`
- <xref:Ploch.Common.Collections> — full API reference for the namespace
- [Ploch.Common library guide](../../docs/libraries/common.md) — installation and the wider package
- [Samples](samples.md) — a short tour of the library as a whole
