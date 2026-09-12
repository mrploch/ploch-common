# Argument Validation with `Ploch.Common.ArgumentChecking`

The `Ploch.Common.ArgumentChecking` namespace provides fluent guard-clause extension methods that
replace the repetitive `if (x is null) throw new ArgumentNullException(nameof(x));` prologue found at
the top of most production methods.

Two static classes make up the namespace:

| Class | Purpose |
|---|---|
| `Guard` | Null, empty, range and boolean-condition checks for arguments and required state. |
| `PathGuard` | File-system path validation — well-formed paths and file existence. |

Both are `static partial` classes whose surface differs slightly by target framework — see
[Target framework differences](#target-framework-differences).

```csharp
using Ploch.Common.ArgumentChecking;
```

## Why guards rather than hand-written `if` blocks

Every guard method **returns the validated value**, so validation can be inlined into the assignment
it protects. That removes the "check block, then assign block" duplication entirely:

```csharp
public sealed class OrderProcessor
{
    private readonly IOrderRepository _repository;
    private readonly ILogger<OrderProcessor> _logger;
    private readonly int _maximumBatchSize;

    public OrderProcessor(IOrderRepository repository, ILogger<OrderProcessor> logger, int maximumBatchSize)
    {
        _repository = repository.NotNull();
        _logger = logger.NotNull();
        _maximumBatchSize = maximumBatchSize.Positive();
    }
}
```

The methods are additionally annotated with JetBrains `[AssertionMethod]` and
`[AssertionCondition]` attributes, so ReSharper and Rider understand that control flow cannot
continue past a failed guard, and stop reporting false-positive "possible null reference" warnings on
the lines that follow.

## `NotNull` — reject `null` arguments

`NotNull` throws `ArgumentNullException` and returns the non-null value. There are two overloads: one
constrained to `where T : struct` (for `Nullable<T>` arguments, returning the unwrapped value) and one
for reference types.

```csharp
public Invoice Render(Customer customer, DateTime? issuedOn)
{
    customer.NotNull();                     // ArgumentNullException(nameof(customer)) when null
    var issueDate = issuedOn.NotNull();     // DateTime, not DateTime? — the value is unwrapped

    return new Invoice(customer, issueDate);
}
```

The method is also annotated `[NotNull]` on the argument, so the compiler's nullable-flow analysis
treats the argument as non-null for the remainder of the method — no `!` null-forgiving operator
needed downstream.

## `RequiredNotNull` — reject invalid *state*

`NotNull` is for arguments; `RequiredNotNull` is for state that *should* already be populated. It
throws `InvalidOperationException` rather than `ArgumentNullException`, because the caller did not
pass the offending value — the object is simply not in a usable condition.

```csharp
public sealed class ReportBuilder
{
    private ReportTemplate? _template;

    public ReportBuilder UseTemplate(ReportTemplate template)
    {
        _template = template.NotNull();

        return this;
    }

    public Report Build()
    {
        // InvalidOperationException: "Variable _template cannot be null."
        var template = _template.RequiredNotNull();

        return template.Render();
    }
}
```

Both overloads accept an optional message format string whose `{0}` placeholder is substituted with
the captured variable name:

```csharp
var template = _template.RequiredNotNull("A template must be supplied before calling Build() ({0}).");
```

Getting the distinction right matters in a web API: `ArgumentNullException` surfacing from a request
handler usually means a `400`-shaped bug in binding or validation, whereas `InvalidOperationException`
means the server got itself into a state it should not have — a `500`. Using the correct guard keeps
that signal intact all the way to the error-handling middleware.

## `NotNullOrEmpty` — strings and collections

There are two `NotNullOrEmpty` overloads: one for `string?` and a generic one constrained to
`where TEnumerable : class, IEnumerable`. Both throw `ArgumentNullException` for `null` and
`ArgumentException` for empty:

```csharp
public Task<SearchResults> SearchAsync(string query, IReadOnlyCollection<string> facets)
{
    query.NotNullOrEmpty();
    facets.NotNullOrEmpty();

    // ...
}
```

The enumerable overload enumerates only far enough to establish whether the sequence yields at least
one element, so it is safe to pass a lazily evaluated sequence — but note that a `IEnumerable<T>`
built from an iterator will be *partially consumed* by the check. Guard materialised collections
(`IReadOnlyCollection<T>`, arrays, `List<T>`) rather than raw iterators.

`RequiredNotNullOrEmpty` is the state-oriented counterpart for strings, throwing
`InvalidOperationException`:

```csharp
public Uri BuildEndpoint()
{
    var baseAddress = _options.BaseAddress.RequiredNotNullOrEmpty();

    return new Uri(baseAddress);
}
```

## `NotOutOfRange` — validate enum arguments

Public methods that accept an enum cannot rely on the type system alone: `(OrderStatus)999` is a
perfectly legal value of the type. `NotOutOfRange` closes that hole, throwing
`ArgumentOutOfRangeException` for undefined values.

```csharp
public IQueryable<Order> FilterByStatus(IQueryable<Order> orders, OrderStatus status)
{
    status.NotOutOfRange();

    return orders.Where(order => order.Status == status);
}
```

The check is `[Flags]`-aware. For an enum decorated with `[FlagsAttribute]`, a combination of
declared flags is accepted even though `Enum.IsDefined` would reject it, while a value containing
bits that no declared member covers is still rejected:

```csharp
[Flags]
public enum FilePermissions
{
    None = 0,
    Read = 1,
    Write = 2,
    Execute = 4
}

(FilePermissions.Read | FilePermissions.Write).NotOutOfRange();  // passes — every bit is declared
((FilePermissions)16).NotOutOfRange();                           // ArgumentOutOfRangeException
```

## `Positive` — numeric arguments greater than the type default

`Positive` is generic over `where TValue : struct, IComparable<TValue>`, so it works for `int`,
`long`, `decimal`, `TimeSpan` and any other comparable value type. It throws
`ArgumentOutOfRangeException` when the value compares less than or equal to `default(TValue)` — note
that **zero is rejected**.

```csharp
public IEnumerable<T> Paginate<T>(IEnumerable<T> source, int pageSize, TimeSpan timeout)
{
    pageSize.Positive();
    timeout.Positive();   // TimeSpan.Zero and negative timeouts both rejected

    // ...
}
```

## `RequiredTrue` and `RequiredFalse` — arbitrary conditions

For invariants that no dedicated guard covers, `RequiredTrue` and `RequiredFalse` assert a boolean
condition and throw `InvalidOperationException`:

```csharp
public void Commit()
{
    _isDisposed.RequiredFalse("The unit of work has already been disposed.");
    _hasPendingChanges.RequiredTrue("Commit was called with no pending changes.");

    // ...
}
```

On .NET 7 and later `RequiredTrue` needs no message at all — see the next section.

## Target framework differences

`Ploch.Common` multi-targets `netstandard2.0` and `net8.0`, and the guard API adapts to the
capabilities of each.

**On `net8.0` (and any `NET7_0_OR_GREATER` consumer)** every guard takes its parameter-name argument
via `[CallerArgumentExpression]`, so it is optional and filled in by the compiler:

```csharp
customer.NotNull();                   // parameter name captured automatically as "customer"
customer.NotNull("customerRecord");   // still overridable when you want a different label
```

`RequiredTrue` additionally captures `[CallerMemberName]`, `[CallerFilePath]` and
`[CallerLineNumber]`, producing a message of the form
*"Condition {expression} is required to be true in {member}, {file} at {line}"* when no custom format
is supplied:

```csharp
(order.Total > 0).RequiredTrue();   // message includes the expression, member, file and line
```

The `net8.0` build also adds `NotNullOrDefault<TValue>`, which rejects `default(TValue)` as well as
`null` — useful for arguments such as `Guid` or `DateTime` where the default is never meaningful:

```csharp
public Task<Tenant> LoadAsync(Guid tenantId)
{
    tenantId.NotNullOrDefault();   // ArgumentNullException when Guid.Empty

    // ...
}
```

**On `netstandard2.0`** — the target used by .NET Framework consumers — `[CallerArgumentExpression]`
is unavailable, so the parameter name is a required positional argument and `RequiredTrue` requires an
explicit message:

```csharp
customer.NotNull(nameof(customer));
pageSize.Positive(nameof(pageSize));
status.NotOutOfRange(nameof(status));
isDisposed.RequiredTrue("The connection must be open.");
```

Code inside `Ploch.Common` itself uses the explicit `nameof(...)` form throughout, because it must
compile against both targets. Application code that only ever runs on modern .NET can omit it.

## `PathGuard` — validating file-system paths

`PathGuard` covers the two path checks that appear most often at a public API boundary.

```csharp
using Ploch.Common.ArgumentChecking;
```

`IsValidPath` verifies that the string is a non-empty, well-formed path (it does **not** touch the
disk), and `EnsureFileExists` additionally verifies that a file is present at that path:

```csharp
public Configuration LoadConfiguration(string configurationFilePath)
{
    configurationFilePath.EnsureFileExists();

    return Deserialize(File.ReadAllText(configurationFilePath));
}

public void SetOutputDirectory(string outputDirectory)
{
    _outputDirectory = outputDirectory.IsValidPath();
}
```

Both throw `ArgumentException` on failure — `EnsureFileExists` reports *"The path does not exist"*
rather than a `FileNotFoundException`, because the failure is a bad argument, not a failed I/O
operation.

On `net8.0` two state-oriented variants are also available — `RequiredIsValidPath` and
`RequiredFileExists` — which throw `InvalidOperationException` instead, following the same
argument-versus-state distinction as `Guard`:

```csharp
public Stream OpenTemplate()
{
    var path = _options.TemplatePath.RequiredFileExists();

    return File.OpenRead(path);
}
```

The `netstandard2.0` build instead exposes `RequireValidPath(path, parameterName)`, which is stricter
than `IsValidPath`: as well as rejecting invalid characters it requires the path to be **rooted**,
throwing `InvalidOperationException` for a relative path. `EnsureFileExists` on that target delegates
to it, so relative paths are rejected there too. As with `Guard`, the `netstandard2.0` build requires
the parameter name explicitly (`path.IsValidPath(nameof(path))`), while the `net8.0` build captures it
automatically.

## Choosing the right guard

| Situation | Guard | Exception thrown |
|---|---|---|
| A caller passed `null` | `NotNull` | `ArgumentNullException` |
| Internal state is not populated yet | `RequiredNotNull` | `InvalidOperationException` |
| A caller passed an empty string or collection | `NotNullOrEmpty` | `ArgumentException` |
| Required state is an empty string | `RequiredNotNullOrEmpty` | `InvalidOperationException` |
| An enum argument may be an undeclared value | `NotOutOfRange` | `ArgumentOutOfRangeException` |
| A count, size or duration must be greater than zero | `Positive` | `ArgumentOutOfRangeException` |
| A `Guid`/`DateTime` argument must not be its default (net8.0) | `NotNullOrDefault` | `ArgumentNullException` |
| An arbitrary invariant must hold | `RequiredTrue` / `RequiredFalse` | `InvalidOperationException` |
| A path argument must be well formed | `IsValidPath` | `ArgumentException` |
| A file must exist at the given path | `EnsureFileExists` | `ArgumentException` |

> **Note:** the older `Ploch.Common.DawnGuard` package, built on the Dawn.Guard library, is
> deprecated. New code should use `Ploch.Common.ArgumentChecking`.

## See also

- [Collections and enumerable extensions](collections-samples.md)
- [Strings, parsing and text building](strings.md)
- [`Guard` API reference](../api/Ploch.Common.ArgumentChecking.Guard.html)
- [`PathGuard` API reference](../api/Ploch.Common.ArgumentChecking.PathGuard.html)
