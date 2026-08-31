# Argument Validation

Every public method has a contract, and most of the cost of a bug is the distance between the point where a
contract is broken and the point where the program finally falls over. A `null` that slips past a constructor
surfaces three layers later as a `NullReferenceException` with a stack trace pointing at code that did nothing
wrong. An out-of-range enum quietly falls through a `switch` and takes the default branch.

The `Ploch.Common.ArgumentChecking` namespace exists to close that distance. It provides two static classes of
guard clauses, written as extension methods so that a validation reads as one line at the top of a method:

| Class | Covers |
|-------|--------|
| <xref:Ploch.Common.ArgumentChecking.Guard> | Nulls, empty strings and sequences, positive numbers, defined enum values, arbitrary boolean preconditions. |
| <xref:Ploch.Common.ArgumentChecking.PathGuard> | File-system paths: valid characters, existence on disk. |

Reach for these when you are writing the *entry point* of something — a public API method, a constructor that
takes injected dependencies, a use case invoked from a controller, a library method someone else will call.
Do not scatter them through private helpers that only your own already-validated code calls; guard clauses
earn their keep at boundaries, and become noise everywhere else.

Both classes ship in the `Ploch.Common` package:

```bash
dotnet add package Ploch.Common
```

For the wider picture of what else lives in this package, see the
[Ploch.Common library guide](../../docs/libraries/common.md).

## The most important distinction: `NotNull` versus `RequiredNotNull`

Nearly every guard in the namespace comes in two flavours, and choosing between them is the single decision
that most affects how useful your exceptions are:

| Family | Throws | Means |
|--------|--------|-------|
| `NotNull`, `NotNullOrEmpty`, `NotNullOrDefault`, `Positive`, `NotOutOfRange`, `IsValidPath`, `EnsureFileExists` | `ArgumentNullException`, `ArgumentException`, `ArgumentOutOfRangeException` | **The caller passed something invalid.** The bug is on the other side of the call. |
| `RequiredNotNull`, `RequiredNotNullOrEmpty`, `RequiredTrue`, `RequiredFalse`, `RequiredIsValidPath`, `RequiredFileExists` | `InvalidOperationException` | **This object is in a state it should not be in.** Nobody passed anything; something was never initialised, or was initialised wrongly. |

That mapping is not decorative. `ArgumentNullException` carries a `ParamName`, and every diagnostic tool,
log aggregator and human reader treats it as "look at the call site". `InvalidOperationException` carries no
parameter name and says "look at this object's lifecycle". Pick the wrong one and you send whoever is on call
to the wrong file.

The rule of thumb: **if the value arrived as a parameter of the method you are writing, use the `Not*` family.
If the value came from a field, a property, a configuration lookup or a lazily-initialised cache, use the
`Required*` family.**

```csharp
using Ploch.Common.ArgumentChecking;

public sealed class InvoiceExporter
{
    private readonly IInvoiceRepository _repository;
    private string? _outputDirectory;

    public InvoiceExporter(IInvoiceRepository repository)
    {
        // The caller handed us this. If it is null, the caller is wrong.
        _repository = repository.NotNull();
    }

    public void Configure(string outputDirectory)
    {
        _outputDirectory = outputDirectory.NotNullOrEmpty();
    }

    public void Export(int invoiceId)
    {
        // Nobody passed _outputDirectory to Export. If it is still null, this object
        // was used before it was configured - a lifecycle bug, not a caller bug.
        var directory = _outputDirectory.RequiredNotNullOrEmpty();

        invoiceId.Positive();

        // ... write the invoice into `directory`
    }
}
```

Calling `Export` before `Configure` produces
`InvalidOperationException: Variable _outputDirectory cannot be null.` (or `cannot be empty.` if it was
configured with `""`) — which points at the object.
Calling `Configure(null!)` produces an `ArgumentNullException` whose `ParamName` is `outputDirectory` — which
points at the call site. Swap the two guards and both messages become misleading.

## Guards return their argument

Every guard returns the validated value, so validation and assignment are one statement rather than two. This
matters most in constructors, where the alternative is a block of `if (x is null) throw` followed by a second
block of assignments:

```csharp
public sealed class DocumentArchiveService
{
    private readonly IDocumentStore _store;
    private readonly IClock _clock;
    private readonly string _archiveRoot;

    public DocumentArchiveService(IDocumentStore store, IClock clock, ArchiveOptions options)
    {
        _store = store.NotNull();
        _clock = clock.NotNull();
        _archiveRoot = options.NotNull().RootPath.NotNullOrEmpty();
    }
}
```

The nullable-annotation attributes on the guards (`[NotNull]`, `[NotNullIfNotNull]`) mean the compiler's
flow analysis follows along: after `x.NotNull()`, `x` is treated as non-null for the rest of the method, so
you get no spurious CS8602 warnings and no need for the `!` operator.

Guards are also usable as expressions inside a larger call, which is handy at the edges of a system:

```csharp
public Task<ArchivedDocument> ArchiveAsync(Document document, CancellationToken cancellationToken)
    => _store.WriteAsync(document.NotNull(), _archiveRoot, cancellationToken);
```

## Parameter names are captured for you (on .NET 7 and later)

On `net7.0` and later targets — which includes the `net8.0` build of `Ploch.Common` — the name parameter of
every guard is optional and decorated with `[CallerArgumentExpression]`. You never write the name as a string
literal, so it cannot go stale when the parameter is renamed:

```csharp
public void Send(string recipientAddress)
{
    recipientAddress.NotNullOrEmpty();
    // ArgumentNullException.ParamName == "recipientAddress"
}
```

Because the attribute captures the *expression*, not just an identifier, richer call sites produce richer
names. `order.Customer.Email.NotNullOrEmpty()` reports `order.Customer.Email` as the parameter name — usually
what you want in a log, occasionally surprising if you were expecting a bare identifier.

> [!IMPORTANT]
> The `netstandard2.0` build has no `CallerArgumentExpression`, so on that target the name parameter is
> **required and positional**. Code written against the `net8.0` build that omits the name will not compile
> when retargeted to `netstandard2.0`. If your code multi-targets, pass the name explicitly:
> `recipientAddress.NotNullOrEmpty(nameof(recipientAddress))`.

The two targets also differ in which members exist at all — see
[Target-framework differences](#target-framework-differences) below.

## Guard reference

### `NotNull`

Two overloads: one for reference types, one for `Nullable<T>` value types. The value-type overload *unwraps*
the nullable, which is why it is worth preferring over a bare null check:

```csharp
public void Schedule(DateTimeOffset? runAt, IJobQueue queue)
{
    IJobQueue validQueue = queue.NotNull();
    DateTimeOffset when = runAt.NotNull();   // DateTimeOffset, not DateTimeOffset?

    validQueue.Enqueue(when);
}
```

Both throw `ArgumentNullException`. The reference-type overload delegates to
`ArgumentNullException.ThrowIfNull` on `net7.0+`, so the message text matches the BCL exactly.

### `NotNullOrEmpty`

Two overloads again: one for `string`, one for anything implementing `IEnumerable`.

```csharp
public void Notify(string subject, IReadOnlyCollection<string> recipients)
{
    subject.NotNullOrEmpty();      // ArgumentNullException, or ArgumentException if ""
    recipients.NotNullOrEmpty();   // ArgumentNullException, or ArgumentException if empty
}
```

The string overload throws `ArgumentException` with the BCL's own wording,
`"The value cannot be an empty string."`, on both targets. A whitespace-only string **passes** — this is an
empty check, not a whitespace check. If blank input is also invalid, guard it yourself:

```csharp
subject.NotNullOrEmpty();
string.IsNullOrWhiteSpace(subject).RequiredFalse("Subject must contain non-whitespace characters.");
```

> [!WARNING]
> The enumerable overload determines emptiness by **taking an enumerator and calling `MoveNext`**. On a
> materialised collection that is free. On a lazily-evaluated LINQ query or a `yield`-returning iterator it
> executes the sequence, and the caller's subsequent enumeration executes it a *second* time — twice the
> database round trips, or an outright failure for a single-pass sequence such as a stream reader. Materialise
> first, then guard:
>
> ```csharp
> // Wrong: the query runs twice.
> var results = repository.Query(filter);
> results.NotNullOrEmpty();
> foreach (var result in results) { /* ... */ }
>
> // Right: one round trip, then guard the materialised list.
> var results = repository.Query(filter).ToList();
> results.NotNullOrEmpty();
> ```
>
> On the `netstandard2.0` build there is a fast path for anything implementing the non-generic `ICollection`,
> which covers `List<T>` and arrays; the `net7.0+` build always enumerates. Do not rely on the fast path.

### `NotNullOrDefault`

Rejects `null` *and* the type's default value, throwing `ArgumentNullException` in both cases. Useful for
identity-shaped parameters where zero or `Guid.Empty` means "never populated":

```csharp
public Task<Order> LoadAsync(Guid orderId)
{
    orderId.NotNullOrDefault();   // ArgumentNullException if Guid.Empty
    // ...
}
```

Be deliberate here: it rejects `0`, `false`, `Guid.Empty` and `default(DateTime)` alike. For a `bool`
parameter, or an `int` where zero is legitimate, this guard is wrong. It is available only on the
`net7.0+` build.

### `Positive`

Constrained to `struct, IComparable<TValue>`, and compares against `default(TValue)` — so it works for every
numeric type, and also for `TimeSpan`, where "positive" naturally means "greater than zero". Zero itself
fails; the guard is strictly greater-than.

```csharp
public IReadOnlyList<AuditEntry> GetPage(int pageSize, TimeSpan queryTimeout)
{
    pageSize.Positive();        // ArgumentOutOfRangeException when 0 or negative
    queryTimeout.Positive();    // ArgumentOutOfRangeException when TimeSpan.Zero or negative
    // ...
}
```

The thrown `ArgumentOutOfRangeException` carries the offending value in `ActualValue`, which is worth more in
a log than the message alone.

### `NotOutOfRange`

Validates that an enum value is actually one of the enum's members. This is the guard people most often skip
and most often need: C# lets any caller cast any integer to any enum type, so an enum parameter is *not*
self-validating.

```csharp
public enum ExportFormat { Csv, Xlsx, Pdf }

public Stream Export(ExportFormat format)
{
    format.NotOutOfRange();   // (ExportFormat)42 throws ArgumentOutOfRangeException

    return format switch
    {
        ExportFormat.Csv  => ExportCsv(),
        ExportFormat.Xlsx => ExportXlsx(),
        ExportFormat.Pdf  => ExportPdf(),
        _ => throw new UnreachableException()
    };
}
```

`[Flags]` enums get bespoke treatment. A combination of defined flags is accepted even though it is not
itself a named member, because that is the whole point of a flags enum; a value containing any *undefined*
bit is rejected. This works across every enum underlying type, signed and unsigned:

```csharp
[Flags]
public enum SyncScope { None = 0, Contacts = 1, Calendar = 2, Files = 4 }

(SyncScope.Contacts | SyncScope.Files).NotOutOfRange();   // passes: 5, all bits defined
((SyncScope)16).NotOutOfRange();                          // throws: bit 4 is not defined
```

Two consequences worth internalising. First, a `[Flags]` enum with no `None = 0` member still accepts the
zero value, because "no flags set" is a legitimate state. Second, on a non-flags enum a bitwise combination is
rejected — `(ExportFormat)(Csv | Xlsx)` is just the undefined value `1`... unless that value happens to be
defined, which is the classic reason to give non-flags enums explicit, non-overlapping values.

### `RequiredTrue` and `RequiredFalse`

The escape hatch for preconditions that no other guard expresses. Both throw `InvalidOperationException`, so
by the taxonomy above they are for *state* checks:

```csharp
public void Commit()
{
    _transactionStarted.RequiredTrue("Commit called before BeginTransaction.");
    _disposed.RequiredFalse("Cannot commit a disposed unit of work.");
    // ...
}
```

On `net7.0+`, `RequiredTrue`'s message argument is optional, and when omitted the default message is built
from four automatically-captured caller values — the *source expression*, the member name, the file path and
the line number:

```csharp
var order = LoadOrder(id);
(order.Total > 0m).RequiredTrue();
// InvalidOperationException: Condition order.Total > 0m is required to be true in
// ProcessPayment, C:\src\Payments\PaymentService.cs at 42
```

That is one of the most useful diagnostics in the namespace, and you get it for free by passing nothing.

> [!NOTE]
> `RequiredFalse` takes a **mandatory** message and has no expression capture; only `RequiredTrue` gets the
> richer default. If you want the diagnostic, invert the condition and use `RequiredTrue`.

Custom message formats are composite format strings. For `RequiredTrue`, `{0}` is the captured expression,
`{1}` the member name, `{2}` the file path and `{3}` the line number:

```csharp
(retryCount < maxRetries).RequiredTrue("Retry budget exhausted; the check {0} failed in {1}.");
```

### `RequiredNotNull` and `RequiredNotNullOrEmpty`

The `InvalidOperationException` counterparts of `NotNull` and `NotNullOrEmpty`, with the same reference-type
and `Nullable<T>` overload pair. Their optional `messageFormat` uses `{0}` for the member name:

```csharp
public sealed class AuditTrail
{
    private IReadOnlyList<AuditEntry>? _entries;

    public void Load(IAuditStore store) => _entries = store.NotNull().ReadAll();

    public AuditEntry Latest()
    {
        var entries = _entries.RequiredNotNull("The audit trail {0} has not been loaded yet.");
        // InvalidOperationException: The audit trail _entries has not been loaded yet.

        return entries[^1];
    }
}
```

Omit the format and you get `Variable _entries cannot be null.` — adequate, but a sentence explaining *what
should have happened first* is what turns a support ticket into a two-minute fix.

## Validating paths with `PathGuard`

`PathGuard` layers two concerns on top of `Guard`: is this string a syntactically plausible path, and does a
file actually exist there? The same caller-error/state-error split applies.

```csharp
using Ploch.Common.ArgumentChecking;

public sealed class ConfigurationImporter
{
    private string? _lastImportedFile;

    // The caller supplied this path: ArgumentException family.
    public ImportResult Import(string configurationFilePath)
    {
        configurationFilePath.EnsureFileExists();   // ArgumentException if missing or malformed
        _lastImportedFile = configurationFilePath;

        return ReadAndApply(configurationFilePath);
    }

    // Our own remembered state: InvalidOperationException family.
    public ImportResult Reimport()
    {
        var path = _lastImportedFile.RequiredFileExists();
        // InvalidOperationException if Import was never called, or the file has since been deleted.

        return ReadAndApply(path);
    }
}
```

| Member | Throws when | Availability |
|--------|-------------|--------------|
| `IsValidPath` | Path is null, empty, or contains characters rejected by `Path.GetInvalidPathChars()` | all targets |
| `EnsureFileExists` | Path is invalid, or no file exists at it | all targets |
| `RequiredIsValidPath` | Same checks, `InvalidOperationException` instead | `net7.0+` only |
| `RequiredFileExists` | Same checks, `InvalidOperationException` instead | `net7.0+` only |
| `RequireValidPath` | Path is invalid **or not rooted** | `netstandard2.0` only |

Two caveats. `EnsureFileExists` accepts relative paths and resolves them against the process's current working
directory, so a path that exists during a unit test may not exist under a service host with a different
working directory — pass absolute paths across process boundaries. And `Path.GetInvalidPathChars()` is
platform-dependent and, on modern .NET, deliberately permissive; `IsValidPath` is a cheap sanity check, not a
guarantee that the file system will accept the name.

## Target-framework differences

`Ploch.Common` multi-targets `netstandard2.0` and `net8.0`, and the guard surface is not identical between
them. The table below is the complete list of asymmetries:

| Member | `netstandard2.0` | `net7.0+` (`net8.0` build) |
|--------|------------------|----------------------------|
| Name parameter on every guard | required, positional | optional, auto-captured |
| `NotNullOrDefault` | not available | available |
| `RequiredIsValidPath`, `RequiredFileExists` | not available | available |
| `RequireValidPath` (rooted-path check) | available | not available |
| `RequiredTrue` default message | not available (message is mandatory) | expression, member, file and line |
| `NotNullOrEmpty` on `IEnumerable` | `ICollection` fast path, then enumerate | always enumerates |

If you are writing a library that itself multi-targets, write against the `netstandard2.0` shape — pass names
explicitly, avoid the `net7.0`-only members — and both builds will compile.

## Migrating from `Ploch.Common.DawnGuard`

`Ploch.Common.DawnGuard` wrapped the third-party [Dawn.Guard](https://github.com/safakgur/guard) library. It
is **deprecated**: its `TypeGuards` class carries `[Obsolete]` and will be removed in a future major version.
`Ploch.Common.ArgumentChecking` replaces it, with no third-party dependency and no `ArgumentInfo<T>` wrapper
struct to thread through call sites.

```csharp
// Before - Ploch.Common.DawnGuard (deprecated)
Guard.Argument(value, nameof(value)).NotNull();

// After - Ploch.Common.ArgumentChecking
value.NotNull();
```

Note that the two libraries both expose a type called `Guard`, in `Dawn` and in
`Ploch.Common.ArgumentChecking` respectively. During a migration, files that reference both need an alias:

```csharp
using PlochGuard = Ploch.Common.ArgumentChecking.Guard;
```

The cleanest migration is to remove the `Ploch.Common.DawnGuard` and `Dawn.Guard` package references entirely
once the last call site is converted; the new guards are extension methods, so the only `using` you need is
`Ploch.Common.ArgumentChecking`.

## Guidance in brief

- Guard at boundaries — public methods, constructors, use case entry points. Not in private helpers.
- Use the `Not*` family for parameters and the `Required*` family for object state; the exception type is a
  message to whoever reads the log.
- Assign the return value; a guard and an assignment are one line.
- Materialise sequences before calling `NotNullOrEmpty` on them.
- Always guard enum parameters with `NotOutOfRange` — an enum parameter is not self-validating.
- On `net7.0+`, omit the parameter name and let the compiler capture the expression.

## See also

- <xref:Ploch.Common.ArgumentChecking.Guard> — full API reference
- <xref:Ploch.Common.ArgumentChecking.PathGuard> — full API reference
- [Ploch.Common library guide](../../docs/libraries/common.md)
- [Collections samples](collections-samples.md)
