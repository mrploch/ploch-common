# Strings, Parsing and Text Building

Four types in the root `Ploch.Common` namespace cover everyday string work:

| Type | Purpose |
|---|---|
| `StringExtensions` | Null/empty tests, Base64, case-insensitive comparison, prefix replacement, `ContainsAny`, integer conversion. |
| `StringParsingExtensions` | Nullable-returning parsers for `bool`, `int` and `long`. |
| `StringBuilderExtensions` | Conditional appends. |
| `Strings` and `Chars` | Constants and repeated-character factories. |

```csharp
using Ploch.Common;
```

---

## Null and empty tests

```csharp
public static bool IsNullOrEmpty(this string? str)
public static bool IsNotNullOrEmpty(this string? str)
public static bool IsNullOrWhiteSpace(this string? str)
public static string? NullIfEmpty(this string? str)
public static string? NullIfWhiteSpace(this string? str)
```

The first three are extension-method forms of the `string` statics, which lets the subject of the test
lead the expression rather than trail it:

```csharp
if (request.CustomerReference.IsNullOrWhiteSpace())
{
    return Result.Invalid("A customer reference is required.");
}
```

`IsNotNullOrEmpty` matters more than it first appears: `!string.IsNullOrEmpty(x)` puts the negation
several tokens away from the thing being negated, and it is easy to misread in a compound condition.

`NullIfEmpty` and `NullIfWhiteSpace` are the ones that earn their keep in production code. An empty
string arriving from an HTML form, a CSV import or a query string almost always means "absent", but
it is not `null`, so the null-coalescing operator does nothing:

```csharp
// "" is not null, so the default is never applied.
var displayName = request.DisplayName ?? user.UserName;

// Correct: normalise empty to null first.
var displayName = request.DisplayName.NullIfWhiteSpace() ?? user.UserName;
```

The same normalisation belongs at every persistence boundary, so that a column holds either a real
value or `NULL` and never a mix of `NULL` and `''`:

```csharp
entity.MiddleName = request.MiddleName.NullIfWhiteSpace();
entity.Notes = request.Notes.NullIfWhiteSpace();
```

---

## Case-insensitive comparison — `EqualsIgnoreCase`

```csharp
public static bool EqualsIgnoreCase(this string? str, string? other)
```

Uses `StringComparison.OrdinalIgnoreCase`, and handles `null` on either side — two `null`s are equal,
one `null` is not. Because the receiver may be `null`, it never throws:

```csharp
if (header.Name.EqualsIgnoreCase("X-Correlation-Id"))
{
    correlationId = header.Value;
}
```

Ordinal comparison is the correct choice for identifiers, header names, scheme names, file extensions
and enum-ish strings. For comparing *user-visible text* in a culture-aware way, use
`string.Equals(a, b, StringComparison.CurrentCultureIgnoreCase)` instead.

---

## `ContainsAny`

```csharp
public static bool ContainsAny(this string str, params string[] strings)
public static bool ContainsAny(this string str, StringComparison comparison, params string[] strings)
public static bool ContainsAny(this string str, IEnumerable<string> strings)
public static bool ContainsAny(this string str, StringComparison comparison, IEnumerable<string> strings)
```

Tests whether the string contains **any** of the candidates. The overloads without a `StringComparison`
use `StringComparison.InvariantCulture`; pass `StringComparison.OrdinalIgnoreCase` explicitly when
matching identifiers or classifying free text.

```csharp
var isTransientFailure = exception.Message.ContainsAny(StringComparison.OrdinalIgnoreCase,
                                                       "timeout",
                                                       "deadlock",
                                                       "connection reset");

if (isTransientFailure)
{
    return await RetryAsync(operation, cancellationToken);
}
```

The `IEnumerable<string>` overload takes the candidate list from configuration:

```csharp
if (propertyName.ContainsAny(StringComparison.OrdinalIgnoreCase, _options.SensitiveFieldMarkers))
{
    value = "***";
}
```

---

## `ReplaceStart` — replace only a prefix

```csharp
public static string ReplaceStart(this string str, string oldValue, string newValue, StringComparison stringComparison = StringComparison.InvariantCulture)
```

`string.Replace` substitutes *every* occurrence, which is wrong whenever the token being replaced is a
prefix that may legitimately recur later in the string. `ReplaceStart` acts only on the start, and
returns the string unchanged when it does not begin with `oldValue`.

Rebasing paths and URLs is the canonical use:

```csharp
var relativePath = fullPath.ReplaceStart(rootDirectory, string.Empty, StringComparison.OrdinalIgnoreCase);
// @"C:\data\reports\2026\q1.csv" with root @"C:\data\" becomes @"reports\2026\q1.csv"

var rewritten = requestUrl.ReplaceStart("https://legacy.example.com", "https://api.example.com");
```

Note how `string.Replace` would corrupt a path such as `C:\data\archive\data\old.csv` by replacing the
second `data` segment too.

---

## Base64 conversion

```csharp
public static string ToBase64String(this string str)
public static string ToBase64String(this string str, Encoding encoding)
public static string FromBase64String(this string str)
public static string FromBase64String(this string str, Encoding encoding)
```

The single-argument overloads use UTF-8. Round-tripping is symmetrical, which makes these convenient
for opaque values that must survive a text-only channel — continuation tokens, state parameters, small
payloads embedded in a URL or header:

```csharp
var continuationToken = $"{lastId}|{lastTimestamp:O}".ToBase64String();

// Later, on the next request:
var parts = continuationToken.FromBase64String().Split('|');
```

`FromBase64String` throws `FormatException` on malformed input, so validate tokens that arrive from
outside your system before decoding.

> Base64 is an encoding, not encryption. Anything in the string is readable by whoever holds it. Sign
> or encrypt values that must not be tampered with or read.

---

## Integer conversion

`StringExtensions` and `StringParsingExtensions` offer three different failure behaviours, so pick the
one that matches the call site.

```csharp
// StringExtensions — throws on invalid input
public static int ToInt32(this string str)
public static long ToInt64(this string str)

// StringExtensions — TryParse pattern, invariant culture by default
public static bool TryConvertToInt32(this string str, out int result)
public static bool TryConvertToInt32(this string str, IFormatProvider provider, out int result)
public static bool TryConvertToInt64(this string str, out long result)
public static bool TryConvertToInt64(this string str, IFormatProvider provider, out long result)

// StringParsingExtensions — nullable result, null-safe receiver
public static bool? ParseToBool(this string? str)
public static int? ParseToInt32(this string? str)
public static long? ParseToInt64(this string? str)
```

| Behaviour | Use for |
|---|---|
| `ToInt32` / `ToInt64` — throws `FormatException` | Values you control, where a malformed value is a bug. |
| `TryConvertToInt32` / `TryConvertToInt64` | Untrusted input where you need to branch on success. |
| `ParseToInt32` / `ParseToInt64` / `ParseToBool` | Optional values where "absent" and "unparseable" are handled the same way. |

The `ParseTo*` family accepts a `null` receiver and returns `null` for `null`, white space, or an
unparseable value — which composes directly with `??`:

```csharp
var pageSize = queryString["pageSize"].ParseToInt32() ?? DefaultPageSize;
var includeArchived = queryString["includeArchived"].ParseToBool() ?? false;
```

`ParseToBool` is `bool.TryParse` underneath, so it accepts `"true"`/`"false"` in any casing but **not**
`"1"`, `"0"`, `"yes"` or `"no"`. Map those yourself if your inputs use them.

`ToInt32` and the parameterless `TryConvert*` overloads use `CultureInfo.InvariantCulture`, which is
the right default for machine-readable input — protocol values, configuration, identifiers. Pass an
explicit `IFormatProvider` for values typed by a user in a specific locale.

---

## `StringBuilderExtensions` — conditional appends

```csharp
public static StringBuilder AppendIfNotNull<TValue>(this StringBuilder builder, TValue? value, Func<TValue?, string>? formatFunc = null)
public static StringBuilder AppendIfNotNullOrEmpty<TValue>(this StringBuilder builder, TValue? value, Func<TValue?, string>? formatFunc = null)
```

Both are generic over the value type, so they work for anything — not just strings — and both return
the builder so the chain is unbroken. The optional `formatFunc` is applied **only when the value is
appended**, which means the formatting expression never has to be null-safe.

The result is that composing text from a mix of required and optional parts stays a single expression
instead of a run of `if` blocks:

```csharp
var address = new StringBuilder()
              .AppendLine(customer.AddressLine1)
              .AppendIfNotNullOrEmpty(customer.AddressLine2, line => line + Environment.NewLine)
              .AppendIfNotNullOrEmpty(customer.County, county => county + Environment.NewLine)
              .AppendLine(customer.PostCode)
              .ToString();
```

`AppendIfNotNull` appends when the value is non-`null`, whatever its `ToString()` yields;
`AppendIfNotNullOrEmpty` additionally skips values whose `ToString()` is `null` or empty — which is
what you want for strings, and for wrapper types whose "empty" state renders as an empty string.

Building a diagnostic line from optional context:

```csharp
var context = new StringBuilder("Request failed")
              .AppendIfNotNull(correlationId, id => $" [correlation: {id}]")
              .AppendIfNotNull(retryAttempt, attempt => $" (attempt {attempt})")
              .AppendIfNotNullOrEmpty(userReference, reference => $" for {reference}")
              .ToString();
```

---

## `Strings` and `Chars` — constants and repeats

```csharp
public static class Chars
{
    public static readonly char Space;      // ' '
    public static readonly char Underscore; // '_'
    public static readonly char Dash;       // '-'
    public static readonly char Dot;        // '.'
}

public static class Strings
{
    public static readonly string Space;      // " "
    public static readonly string Underscore; // "_"
    public static readonly string Dash;       // "-"
    public static readonly string Dot;        // "."

    public static string Spaces(int count);
    public static string Underscores(int count);
    public static string Dashes(int count);
    public static string Dots(int count);
}
```

The factory methods build a string of repeated characters. The obvious use is indentation and rules in
console or text output:

```csharp
public static string RenderTree(TreeNode node, int depth = 0)
{
    var builder = new StringBuilder()
                  .Append(Strings.Spaces(depth * 2))
                  .AppendLine(node.Name);

    foreach (var child in node.Children)
    {
        builder.Append(RenderTree(child, depth + 1));
    }

    return builder.ToString();
}

Console.WriteLine(Strings.Dashes(60));   // a horizontal rule
```

Each factory validates its argument with
[`Positive`](argument-validation.md#positive--numeric-arguments-greater-than-the-type-default), so
**`count` must be greater than zero** — `Strings.Spaces(0)` throws `ArgumentOutOfRangeException`
rather than returning an empty string. Guard the zero case at the call site:

```csharp
var indent = depth > 0 ? Strings.Spaces(depth * 2) : string.Empty;
```

---

## See also

- [Collections and enumerable extensions](collections-samples.md) — `Join` and `JoinWithFinalSeparator`
- [Conversions and enums](conversions-and-enums.md) — `ParseToEnum` and `SafeParseToEnum`
- [Argument validation](argument-validation.md)
- [`Ploch.Common` API reference](../api/Ploch.Common.html)
