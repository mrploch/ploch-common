# Utility Toolbox — Paths, Streams, Hashing, Dates and Matching

The types in this article do not belong to a single theme; what they have in common is that each one
replaces a small piece of code that everybody writes, and that a surprising number of people write
subtly wrongly.

| Area | Type | Namespace |
|---|---|---|
| File-system paths | `PathUtils` | `Ploch.Common.IO` |
| Streams | `StreamExtensions` | `Ploch.Common.IO` |
| Command lines | `CommandLineParser`, `CommandLineInfo` | `Ploch.Common.IO` |
| Hashing | `Hashing` | `Ploch.Common.Cryptography` |
| Dates and formats | `DateTimeExtensions`, `DateTimeFormats` | `Ploch.Common` |
| Sizes | `ContentSizes` | `Ploch.Common` |
| Timing | `StopwatchUtil` | `Ploch.Common` |
| Set membership and defaults | `IsInExtensions`, `IfNullHelpers`, `ComparisonUtils` | `Ploch.Common` |
| Property copying | `ObjectCloningHelpers` | `Ploch.Common` |
| Assembly metadata | `AssemblyInformation`, `AssemblyInformationProvider` | `Ploch.Common` |
| Pattern matching | `IMatcher<T>`, `GlobEvaluator`, `RegexListEvaluator`, `PropertyMatcher<T>` | `Ploch.Common.Matchers` |

---

## `PathUtils` — path manipulation that survives real input

```csharp
using Ploch.Common.IO;

public static string GetDirectoryName(this string directoryPath)
public static string ToSafeFileName(string input)
public static string NormalizePathWithTrailingSeparator(string path)
public static string NormalizePathWithoutTrailingSeparator(string path)
public static string GetRelativePath(string fromPath, string toPath)
public static string WithExtension(this string path, string extension, bool replaceExistingExtension = true, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
public static string GetFullPathWithoutExtension(string path)
```

### `ToSafeFileName` — turn arbitrary text into a file name

Every export feature eventually needs to name a file after something a user typed. `ToSafeFileName`
replaces each character in `Path.GetInvalidFileNameChars()` with an underscore, so the result is
always usable:

```csharp
var fileName = PathUtils.ToSafeFileName($"{customer.Name} — {DateTime.UtcNow:yyyy-MM-dd} report")
                        .WithExtension(".csv");

await File.WriteAllTextAsync(Path.Combine(exportDirectory, fileName), csv, cancellationToken);
```

Note that it sanitises a **file name**, not a path — directory separators are invalid file-name
characters and are therefore replaced. Do not pass a full path to it.

### `WithExtension` — append or replace, idempotently

```csharp
"report".WithExtension(".csv");                                    // "report.csv"
"report.txt".WithExtension(".csv");                                // "report.csv"  (replaced)
"report.txt".WithExtension(".csv", replaceExistingExtension: false); // "report.txt.csv"
"report.csv".WithExtension(".csv");                                // "report.csv"  (already correct)
"report".WithExtension("csv");                                     // "report.csv"  (leading dot added)
```

The leading dot is optional, matching is case-insensitive by default, and a path that already carries
the requested extension is returned unchanged — so the method is safe to apply repeatedly, which
`Path.ChangeExtension` string-concatenation code usually is not:

```csharp
var outputPath = _options.OutputPath.WithExtension(".json");
```

### Normalisation and the trailing-separator problem

`NormalizePathWithTrailingSeparator` and `NormalizePathWithoutTrailingSeparator` both run the path
through `Path.GetFullPath` — resolving `.`, `..` and mixed separators — and then guarantee the
presence or absence of a trailing separator.

That guarantee matters more than it looks. String-comparing two directory paths where one ends in a
separator and the other does not is a classic source of "the same directory is not the same
directory" bugs, and a directory path handed to `new Uri(...)` without a trailing separator is treated
as a *file* by `Uri.MakeRelativeUri`:

```csharp
var configured = PathUtils.NormalizePathWithoutTrailingSeparator(options.WorkingDirectory);
var actual = PathUtils.NormalizePathWithoutTrailingSeparator(Directory.GetCurrentDirectory());

if (!configured.Equals(actual, StringComparison.OrdinalIgnoreCase))
{
    _logger.LogWarning("Running from {Actual}, expected {Configured}", actual, configured);
}
```

Because both call `Path.GetFullPath`, a relative path is resolved against the *current working
directory* — which is rarely the directory you meant in a hosted service. Combine with a known root
first.

`GetDirectoryName` is the short-name accessor: it returns the last segment of a directory path
(`"reports"` from `@"C:\data\reports"`), which is `DirectoryInfo.Name`, **not**
`Path.GetDirectoryName`'s parent path. The names are similar and the semantics are opposite, so read
the call site carefully.

### `GetRelativePath`

```csharp
var relative = PathUtils.GetRelativePath(@"C:\data\", @"C:\data\reports\2026\q1.csv");
// "reports\2026\q1.csv"
```

`fromPath` is treated as a directory (a trailing separator is added for you) and the result uses the
platform's directory separator. When the two paths are on different URI schemes — different drives on
Windows, for instance — `toPath` is returned unchanged rather than an exception being thrown, so check
for rooted output if that case matters to you.

On `net8.0` the framework's own `Path.GetRelativePath` is available and is the better choice; this
helper exists for the `netstandard2.0` target.

---

## `StreamExtensions` and `Hashing`

```csharp
using Ploch.Common.IO;
using Ploch.Common.Cryptography;

public static IEnumerable<byte> ToBytes(this Stream stream)

public static string ToHashString(this Stream stream, HashAlgorithm algorithm)
public static string ToMD5HashString(this Stream stream)
```

`ToHashString` computes the hash and returns it as an uppercase hexadecimal string — the form that
goes into an `ETag`, a manifest file or a de-duplication key:

```csharp
using var fileStream = File.OpenRead(uploadPath);
using var sha256 = SHA256.Create();

var contentHash = fileStream.ToHashString(sha256);

if (await _blobIndex.ExistsAsync(contentHash, cancellationToken))
{
    return Result.Success(new UploadResult(contentHash, Deduplicated: true));
}
```

`ToMD5HashString` is the convenience overload for the case where the hash is a **content fingerprint,
not a security control** — cache keys, change detection, comparing a local file against a remote
`Content-MD5` header. MD5 is cryptographically broken; never use it to verify that content has not
been tampered with. For that, use SHA-256 through `ToHashString`.

Both hashing methods read the stream from its **current position** to the end and leave it at the end.
Rewind before reading the content again:

```csharp
var hash = uploadStream.ToHashString(sha256);
uploadStream.Position = 0;                       // rewind before persisting
await _blobStore.WriteAsync(hash, uploadStream, cancellationToken);
```

`ToBytes` buffers the whole stream and returns its content as an `IEnumerable<byte>` (a `byte[]`
behind the interface). Unlike the hashing methods it **seeks to position 0 first**, so it always
returns the complete content — which also means it requires a seekable stream and will throw on a
network or pipe stream. It reads everything into memory, so keep it for small payloads in tests and
tooling; stream large content with `Stream.CopyToAsync` instead.

---

## `CommandLineParser` — splitting a command line properly

```csharp
using Ploch.Common.IO;

public static string? GetApplicationPath(string commandLineString)
public static CommandLineInfo? GetAsArguments(string commandLineString)
public static CommandLineInfo? GetCommandLine(string commandLineString)
```

`Environment.CommandLine` — and the `CommandLine` property of a WMI `Win32_Process` record — is a
single string in which the executable path and each argument may be quoted. Splitting it on spaces
breaks the moment a path contains one, which on Windows is most of them.

`CommandLineParser` performs a quote-aware split and returns a `CommandLineInfo`, a readonly struct
that separates the two halves:

```csharp
public readonly struct CommandLineInfo : IEquatable<CommandLineInfo>
{
    public string? ApplicationPath { get; }
    public IEnumerable<string> Arguments { get; }
}
```

```csharp
var commandLine = @"""C:\Program Files\MyApp\MyApp.exe"" --config ""C:\Program Data\config.json"" --verbose";

var info = CommandLineParser.GetCommandLine(commandLine);

info!.Value.ApplicationPath;   // C:\Program Files\MyApp\MyApp.exe
info.Value.Arguments;          // ["--config", "C:\Program Data\config.json", "--verbose"]
```

`GetApplicationPath` returns just the executable path, and `GetAsArguments` treats the **whole** string
as arguments — no leading application path is extracted. The typical use is inspecting other
processes:

```csharp
foreach (var process in Process.GetProcessesByName("MyApp"))
{
    var info = CommandLineParser.GetCommandLine(GetCommandLineOf(process));

    if (info?.Arguments.Contains("--worker") == true)
    {
        _logger.LogInformation("Worker instance found: {ProcessId}", process.Id);
    }
}
```

`CommandLineInfo` implements `IEquatable<CommandLineInfo>` with element-wise argument comparison, so
two invocations can be compared directly — useful for detecting that a service was restarted with
different arguments.

This is a *splitter*, not an argument parser: it produces tokens, it does not understand
`--name=value`, short-flag bundling or verbs. Use
[`System.CommandLine`](https://learn.microsoft.com/dotnet/standard/commandline/) or
`Ploch.CommandLine.Spectre` for that.

---

## Dates, formats and sizes

```csharp
public static long  ToEpochSeconds(this DateTime dateTime)
public static long? ToEpochSeconds(this DateTime? dateTime)
public static DateTime  ToDateTime(this long epochSecondsOrMilliseconds)
public static DateTime  ToDateTime<T>(this T epochSeconds) where T : struct, IComparable, IConvertible, ...
public static DateTime? ToDateTime(this long? epochSeconds)
```

Unix timestamps arrive from JavaScript clients, JWT claims, Kafka headers and log pipelines — some in
seconds, some in milliseconds, rarely labelled. `ToDateTime` **detects the unit**: a value outside the
range `DateTime` can represent in seconds is divided by 1000 before conversion, and the result is
always UTC.

```csharp
var issuedAt = jwtClaims["iat"].ParseToInt64()!.Value.ToDateTime();    // seconds
var eventTime = kafkaHeader.Timestamp.ToDateTime();                   // milliseconds — same call
```

That heuristic is a convenience, not a contract: when you *know* the unit, prefer
`DateTimeOffset.FromUnixTimeSeconds` / `FromUnixTimeMilliseconds`, which cannot guess wrong. Use
`ToDateTime` where the unit genuinely varies between sources.

`ToEpochSeconds` casts to `DateTimeOffset` and calls `ToUnixTimeSeconds`, performing **no time-zone
manipulation** — a `DateTime` with `Kind == Unspecified` is interpreted as local time by that cast.
Normalise to UTC before converting:

```csharp
var epoch = timestamp.ToUniversalTime().ToEpochSeconds();
```

`DateTimeFormats` holds the format strings that otherwise get retyped (and mistyped) in every project:

```csharp
public const string YearMonthDayHourMinuteSecondNumbersOnly            = "yyyyMMddHHmmss";
public const string YearMonthDayHourMinuteSecondNumbersWithDashes      = "yyyy-MM-dd-HH-mm-ss";
public const string YearMonthDayHourMinuteSecondNumbersWithDashesAndColons = "yyyy-MM-dd HH:mm:ss";

// nested Date class
public const string YearMonthDayNumbersOnly       = "yyyyMMdd";
public const string YearMonthDayNumbersWithDashes = "yyyy-MM-dd";
```

The sortable, separator-free forms are what you want in generated file names, because lexical order
then matches chronological order:

```csharp
var backupName = $"backup-{DateTime.UtcNow.ToString(DateTimeFormats.YearMonthDayHourMinuteSecondNumbersOnly, CultureInfo.InvariantCulture)}"
                 .WithExtension(".zip");
// "backup-20260828091500.zip"
```

`ContentSizes` names the binary size constants and converts:

```csharp
public const int KiloByte = 1024;
public const int MegaByte = KiloByte * KiloByte;

public static long KilobytesToBytes(long kilobytes);
public static long MegabytesToBytes(long megabytes);
```

```csharp
options.MaxRequestBodySize = ContentSizes.MegabytesToBytes(_settings.MaxUploadMegabytes);

if (fileInfo.Length > ContentSizes.MegabytesToBytes(50))
{
    return Result.Invalid("Uploads are limited to 50 MB.");
}
```

These are the *binary* units (1 KiB = 1024 bytes), which is what buffer sizes and platform limits use.
Storage vendors and some APIs mean 1000 — do not mix the two in one calculation.

---

## `StopwatchUtil` — measure without the boilerplate

```csharp
public static TimeSpan Time(Action action)
public static Task<TimeSpan> TimeAsync(Func<Task> asyncAction)
public static Task<TimeSpan> TimeAsync(Task task)
```

Three lines of `Stopwatch.StartNew()` / work / `sw.Stop()` collapse into one expression, and the
`Stopwatch` cannot be left running or read before it is stopped:

```csharp
var elapsed = StopwatchUtil.Time(() => _index.Rebuild(documents));

_logger.LogInformation("Rebuilt the index over {Count} documents in {Elapsed}", documents.Count, elapsed);
```

The two async overloads differ in *when the clock starts*, which matters:

- `TimeAsync(Func<Task>)` starts the stopwatch **before** invoking the delegate, so synchronous set-up
  inside the method is included. This is normally what you want.
- `TimeAsync(Task)` receives a task that has usually already started, so it measures only the time
  from the call until completion.

```csharp
var elapsed = await StopwatchUtil.TimeAsync(() => _httpClient.GetAsync(uri, cancellationToken));
```

The returned `TimeSpan` is wall-clock time, so it includes any time the thread spent descheduled. For
benchmarking rather than diagnostics, use
[BenchmarkDotNet](https://benchmarkdotnet.org/); for production telemetry, prefer an `Activity` or a
metrics histogram, which carry context this helper cannot.

---

## Small predicates: `In`, `NotIn`, `OrIfNull`, `IsDefault`

```csharp
public static bool In<TValue>(this TValue value, params TValue[] values)
public static bool In<TValue>(this TValue? value, IEnumerable<TValue?> values)
public static bool In<TValue>(this TValue? value, IComparer<TValue?> comparer, params TValue[] values)
public static bool NotIn<TValue>(this TValue value, params TValue[] values)
// ... and the matching comparer overloads

public static TValue OrIfNull<TValue>(this TValue? value, TValue defaultValue)
public static TEnumerable OrIfNullOrEmpty<TEnumerable>(this TEnumerable enumerable, TEnumerable defaultValue)

public static bool IsDefault<TValue>(this TValue? value)
public static bool IsNotDefault<TValue>(this TValue? value)
```

`In` puts the subject first, which reads better than an inverted `Contains` and removes the array
literal from the middle of a condition:

```csharp
// Instead of:  if (new[] { OrderStatus.Cancelled, OrderStatus.Refunded }.Contains(order.Status))
if (order.Status.In(OrderStatus.Cancelled, OrderStatus.Refunded))
{
    return Result.Invalid("This order can no longer be amended.");
}

if (request.CountryCode.NotIn(_options.SupportedCountries))
{
    return Result.Invalid($"{request.CountryCode} is not a supported market.");
}
```

The `IComparer<TValue?>` overloads let membership be decided by something other than default equality
— case-insensitive strings, or a domain-specific equivalence:

```csharp
if (header.Value.In(StringComparer.OrdinalIgnoreCase, "gzip", "deflate", "br"))
{
    // ...
}
```

`IsDefault` answers "is this value `null`, `0`, `false`, `default(Guid)` or `default(DateTime)`?" in a
single generic call — the check that generic code cannot otherwise express without a `switch` over
type kinds:

```csharp
public static void ApplyIfSet<TValue>(TValue? incoming, Action<TValue> apply)
{
    if (incoming.IsNotDefault())
    {
        apply(incoming!);
    }
}
```

`OrIfNull` is `??` with a method name, which is useful in a fluent chain where a null-coalescing
operator would need parentheses; `OrIfNullOrEmpty` substitutes the default for an empty collection or
string as well.

`ObjectCloningHelpers` rounds this group out with reflection-based property copying, for updating an
existing tracked entity in place rather than replacing it:

```csharp
public static void CopyProperties<T>(this T source, T target)
public static void CopyPropertiesIncludeOnly<T>(this T source, T target, params string[]? includedProperties)
public static void CopyPropertiesExcluding<T>(this T source, T target, params string[]? excludedProperties)
```

```csharp
// Update the tracked entity without touching its key or audit columns.
incoming.CopyPropertiesExcluding(tracked, nameof(Order.Id), nameof(Order.CreatedOn), nameof(Order.CreatedBy));

await unitOfWork.CommitAsync(cancellationToken);
```

This is a shallow copy of public, writable properties. Reference-typed properties are shared with the
source afterwards, and the property names are strings — use `nameof` so a rename cannot silently turn
an exclusion into a no-op.

---

## `AssemblyInformation` — product, description and version

```csharp
public class AssemblyInformation(string product, string description, string version)
{
    public AssemblyInformation(Assembly assembly);
}

public static AssemblyInformation GetAssemblyInformation(this object obj)
public static AssemblyInformation GetAssemblyInformation(this Type type)
```

Reads `AssemblyProductAttribute`, `AssemblyDescriptionAttribute` and `AssemblyFileVersionAttribute`
from an assembly, reached either from a type or from any instance:

```csharp
var information = typeof(Startup).GetAssemblyInformation();

_logger.LogInformation("Starting {Product} {Version}", information.Product, information.Version);
```

`Version` is the assembly **file** version (the four-part `AssemblyFileVersion`), not the
informational version — so it carries the numeric version without the prerelease tag or commit
identifier that Nerdbank.GitVersioning puts into `AssemblyInformationalVersion`. When a diagnostics
endpoint needs to identify the exact build, read the informational version directly:

```csharp
var informationalVersion = typeof(Startup).Assembly
                                          .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
                                          .InformationalVersion;
```

Note also that the three attributes are read non-defensively, so constructing
`AssemblyInformation` from an assembly built without any one of them throws
`NullReferenceException`. Every project in this repository sets all three through
`Directory.Build.props`; a third-party assembly may not.

---

## `Ploch.Common.Matchers` — composable string matching

```csharp
using Ploch.Common.Matchers;

public interface IMatcher<in T>
{
    bool IsMatch(T? value);
}

public interface IStringMatcher : IMatcher<string?>;
```

A single-method abstraction over "does this value match?", with three implementations. The point is
that a *rule set loaded from configuration* becomes an injectable dependency rather than a hard-coded
condition.

### `GlobEvaluator` — include and exclude patterns

```csharp
public GlobEvaluator(IEnumerable<string> includes,
                     IEnumerable<string> excludes,
                     bool nullMatchResult = false,
                     bool emptyMatchResult = false,
                     StringComparison comparisonType = StringComparison.OrdinalIgnoreCase)
```

Wraps `Microsoft.Extensions.FileSystemGlobbing.Matcher`. A value matches when it matches at least one
include pattern and no exclude pattern; `null` and empty inputs return the configured results rather
than throwing, so a matcher can be applied to a sequence without pre-filtering:

```csharp
var matcher = new GlobEvaluator(includes: _options.IncludedFilePatterns,   // e.g. ["**/*.cs", "**/*.props"]
                                excludes: _options.ExcludedFilePatterns);  // e.g. ["**/obj/**", "**/bin/**"]

var filesToProcess = allFiles.Where(file => matcher.IsMatch(file.RelativePath)).ToList();
```

### `RegexListEvaluator` — a list of regular expressions

```csharp
public RegexListEvaluator(IEnumerable<string> regexList,
                          bool nullValueMatchResult = false,
                          bool compiled = true,
                          bool ignoreCase = true)
```

Matches when **any** pattern in the list matches. Patterns are compiled by default, which pays for
itself when the matcher is a long-lived singleton and costs start-up time when it is not:

```csharp
// Registered as a singleton — compilation happens once.
services.AddSingleton<IStringMatcher>(_ => new RegexListEvaluator(configuration
                                                                  .GetSection("Redaction:Patterns")
                                                                  .Get<string[]>() ?? []));
```

### `PropertyMatcher<TSourceType>` — lift a string matcher onto an object

```csharp
public PropertyMatcher(Func<TSourceType?, string?> propertySelector, IMatcher<string?> stringMatcher)
```

Projects an object to one of its string properties and applies a string matcher to it, turning any
`IStringMatcher` into an `IMatcher<TSourceType>`:

```csharp
var byFileName = new PropertyMatcher<IFileInfo>(file => file?.Name,
                                                new GlobEvaluator(["*.log", "*.txt"], ["*.tmp.*"]));

var logFiles = directory.EnumerateFiles().Where(byFileName.IsMatch);
```

Because both sides are interfaces, a glob-based rule can be swapped for a regex-based one — or for a
test double — without the consuming code changing.

`GlobMatcherExtensions` (`IncludePatterns` / `ExcludePatterns`) provides the fluent, collection-taking
helpers on `Matcher` that `GlobEvaluator` uses internally, and is public so you can use `Matcher`
directly the same way.

## See also

- [Argument validation](argument-validation.md)
- [Collections and enumerable extensions](collections-samples.md)
- [Environment, processes and the host operating system](environment-and-processes.md)
- [Type loading and assembly scanning](type-loading.md)
- [`Ploch.Common` API reference](../api/Ploch.Common.html)
