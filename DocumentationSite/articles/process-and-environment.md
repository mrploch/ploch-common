# Processes and the Environment

Every non-trivial application eventually has to ask questions about the machine it woke up on.
*Where was I installed?* *Is tracing switched on for this deployment?* *Am I on Windows?* *Which
processors may this worker use?* The BCL answers all four, but each answer is either one layer too
low to use directly or one edge case sharper than it first appears.

`Ploch.Common` supplies four small, focused helpers for exactly those questions:

| Type | Answers |
|------|---------|
| <xref:Ploch.Common.EnvironmentVariables> | *What is configured?* — environment variables read as `string`, `bool?` or an enum, without `TryParse` boilerplate at every call site. |
| <xref:Ploch.Common.EnvironmentUtilities> | *Where am I, and how was I started?* — the directory the application was deployed to, and the raw command line. |
| <xref:Ploch.Common.OperatingSystemExtensions> | *Which platform is this?* — an `IsWindows()` that hangs off an `OperatingSystem` instance, so it can be substituted in tests. |
| <xref:Ploch.Common.Diagnostics.ProcessExtensions> | *Which processors may this process use?* — processor affinity as a set of processor numbers instead of a pointer-sized bitmask. |

The first three live in the `Ploch.Common` namespace; the fourth is in `Ploch.Common.Diagnostics`.
For a wider tour of the package, see the [Ploch.Common library guide](../../docs/libraries/common.md).

These helpers are deliberately thin, and none of them hides a platform difference that the caller
ought to know about. Where behaviour differs between Windows and Linux this article says so, and
every sample that only makes sense on one platform is marked.

## Reading configuration from environment variables

Environment variables are the lowest common denominator of configuration: they are how a container
image is parameterised, how a CI agent passes secrets, and how an operator switches on diagnostics
without a redeploy. The BCL gives you exactly one primitive, `Environment.GetEnvironmentVariable`,
which returns `string?`. Everything else — the null check, the parse, the fallback — is yours.

<xref:Ploch.Common.EnvironmentVariables> collapses the common cases into a single expression each:

```csharp
public static string? GetString(string variableName);

public static bool? GetBool(string variableName);

public static TEnum? GetEnumValue<TEnum>(string variableName, bool ignoreCase = true)
    where TEnum : struct, Enum;
```

All three return `null` for an absent variable, so the whole "is it set, and is it valid?" question
collapses into a null-coalescing operator:

```csharp
public sealed class DiagnosticsOptions
{
    public static DiagnosticsOptions FromEnvironment() =>
        new()
        {
            // Absent, blank and unparseable all fall back to the default.
            VerboseLogging = EnvironmentVariables.GetBool("APP_VERBOSE_LOGGING") ?? false,
            MinimumLevel = EnvironmentVariables.GetEnumValue<LogSeverity>("APP_LOG_LEVEL") ?? LogSeverity.Information,
            TraceEndpoint = EnvironmentVariables.GetString("APP_TRACE_ENDPOINT"),
        };

    public bool VerboseLogging { get; init; }

    public LogSeverity MinimumLevel { get; init; }

    public string? TraceEndpoint { get; init; }
}
```

That reads well, and for a well-behaved deployment it is correct. The rest of this section is about
the deployments that are not well behaved — because *silently falling back to the default* is
exactly the failure mode that costs an afternoon.

### `GetBool` understands `true` and `false`, and nothing else

`GetBool` delegates to `bool.TryParse`, which accepts only those two literals, case-insensitively
and with surrounding whitespace tolerated. Everything else yields `null`. One row per value the
variable was set to:

| Variable value | `GetBool` result |
|----------------|------------------|
| `true`, `True`, `TRUE`, `true` surrounded by spaces | `true` |
| `false` | `false` |
| `1` | `null` |
| `0` | `null` |
| `yes` | `null` |
| `on` | `null` |
| *(empty or whitespace)* | `null` |
| *(variable not set)* | `null` |

`1` is the interesting row. It is how a great many tools spell "on" — `DOCKER_BUILDKIT=1`, `CI=1`,
`DOTNET_NOLOGO=1` — and `GetBool` maps it to `null`, which the `?? false` above turns into *off*. An
operator who sets `APP_VERBOSE_LOGGING=1` gets no verbose logging, and no error either.

Where a variable is set by people rather than by your own deployment templates, accept the wider
vocabulary explicitly:

```csharp
public static bool GetFlag(string variableName, bool defaultValue = false)
{
    // Lower-cased, because GetBool is case-insensitive and the wider vocabulary must be too —
    // otherwise "ON" would fall through to GetBool, come back null, and silently become the default.
    var raw = EnvironmentVariables.GetString(variableName)?.Trim().ToLowerInvariant();

    return raw switch
    {
        null or "" => defaultValue,
        "1" or "yes" or "on" => true,
        "0" or "no" or "off" => false,
        _ => EnvironmentVariables.GetBool(variableName) ?? defaultValue,
    };
}
```

Note which method does which job: `GetString` supplies the raw text for the comparisons, and
`GetBool` handles only the final `true`/`false` case. `GetBool` cannot distinguish "set to nonsense"
from "not set", so it is the wrong tool for deciding whether a value should be *rejected*.

### `GetEnumValue` accepts more than the enum's names

`GetEnumValue<TEnum>` is a thin wrapper over `Enum.TryParse`, and it inherits two behaviours from it
that regularly surprise people. Given:

```csharp
public enum LogSeverity
{
    Verbose = 0,
    Debug = 1,
    Information = 2,
    Warning = 3,
    Error = 4,
}
```

| Variable value | `GetEnumValue<LogSeverity>` result |
|----------------|-----------------------------------|
| `Warning` | `LogSeverity.Warning` |
| `warning` | `LogSeverity.Warning` — `ignoreCase` defaults to `true` |
| `3` | `LogSeverity.Warning` — numeric strings are accepted |
| `99` | `(LogSeverity)99` — **an undefined value, not `null`** |
| `-1` | `(LogSeverity)(-1)` — likewise |
| `Debug, Error` | `(LogSeverity)5` — comma-separated names are combined, even though this is not a `[Flags]` enum |
| *(empty or whitespace)* | `null` |
| *(variable not set)* | `null` |

`Enum.IsDefined(typeof(LogSeverity), (LogSeverity)99)` is `false`, so such a value matches no
`switch` arm you have written and formats as `"99"` rather than as a name. If an out-of-range number
in configuration should be a startup failure rather than a mystery at first use, validate it:

```csharp
public static TEnum RequireDefinedEnum<TEnum>(string variableName, TEnum defaultValue)
    where TEnum : struct, Enum
{
    // GetEnumValue returns null both for "not configured" and for "configured with rubbish", so the
    // raw string decides which of the two it is. Only the first may fall back to the default.
    var raw = EnvironmentVariables.GetString(variableName).NullIfWhiteSpace();

    if (raw is null)
    {
        return defaultValue;
    }

    var value = EnvironmentVariables.GetEnumValue<TEnum>(variableName);

    if (value is null || !Enum.IsDefined(typeof(TEnum), value.Value))
    {
        throw new InvalidOperationException(
            $"Environment variable '{variableName}' is set to '{raw}', which is not a defined " +
            $"value of {typeof(TEnum).Name}. Valid values: {string.Join(", ", Enum.GetNames(typeof(TEnum)))}.");
    }

    return value.Value;
}
```

The two-step shape is what makes the difference: reading the raw string first is the only way to
tell "the operator did not set this" from "the operator set it to `Wrning`", because `GetEnumValue`
collapses both to `null`.

Two further details are worth knowing.

**`ignoreCase` defaults to `true` here**, which is the opposite of the underlying
`SafeParseToEnum<TEnum>` string extension in `Ploch.Common`, whose default is `false`. The choice
suits environment variables — operators type `debug`, not `Debug` — but do not carry the assumption
across to the string extension. Pass `ignoreCase: false` when a variable must match the declared
casing exactly; `GetEnumValue<LogSeverity>("APP_LOG_LEVEL", ignoreCase: false)` returns `null` for
`warning`.

**The variable name itself is not validated by this class.** Passing `null` reaches the BCL, which
throws `ArgumentNullException` with parameter name `variable` — the framework's parameter name, not
`variableName`. In practice the name is a literal or a constant, so this rarely bites.

### There is no `GetInt32`, and that is deliberate

Numeric variables are read by composing `GetString` with the parsing extensions in `Ploch.Common`,
which follow the same "`null` means absent or unparseable" convention:

```csharp
var maxConcurrency = EnvironmentVariables.GetString("APP_MAX_CONCURRENCY").ParseToInt32()
                     ?? Environment.ProcessorCount;
```

### Empty and absent are not the same thing, and .NET 9 changed which is which

`GetString` forwards `Environment.GetEnvironmentVariable` unchanged, so it returns whatever the
process environment holds — including an empty string. `GetBool` and `GetEnumValue` do not: both
treat empty and whitespace as `null` before parsing. **`GetString` is the one that can hand back
`""`**, so a branch on `GetString(name) is null` meaning "not configured" is not equivalent to the
same branch on the parsing methods.

How a variable *becomes* empty matters too, because
[.NET 9 changed it](https://learn.microsoft.com/dotnet/core/compatibility/core-libraries/9.0/empty-env-variable):
before .NET 9, `Environment.SetEnvironmentVariable(name, "")` deleted the variable; from .NET 9 it
sets it to an empty value. That is a **runtime-version** difference, not a platform one: the change
was made precisely because an empty value is valid on every supported platform. Both columns below
were measured on both platforms — Windows 11 (8.0.30 and 9.0.19) and Ubuntu 24.04 (a self-contained
`net8.0` build, and 9.0.14) — with identical results:

| | .NET 8 | .NET 9 and later |
|---|---|---|
| `Environment.SetEnvironmentVariable(name, "")`, then `GetString(name)` | `null` — the assignment deletes the variable | `""` — the variable is set, and empty |
| A variable inherited from the parent process as empty, then `GetString(name)` | `""` | `""` |
| A variable that was never set | `null` | `null` |
| `GetBool(name)` and `GetEnumValue<TEnum>(name)`, for every row above | `null` | `null` |

The row that applies is the one for the runtime the *application* runs on, not the target
`Ploch.Common` was compiled for. An inherited empty variable is possible on both platforms and under
both runtimes, so `GetString` returning `""` is never a case that can be dismissed as unreachable.

Where "present but blank" must be treated as "not configured" everywhere, normalise explicitly —
`Ploch.Common` has an extension for exactly this:

```csharp
var endpoint = EnvironmentVariables.GetString("APP_TRACE_ENDPOINT").NullIfWhiteSpace();
```

## Locating the running application

Configuration files, licence files, bundled tools and templates tend to live *next to the
application*, and the obvious API for finding that directory is the wrong one:
`Environment.CurrentDirectory` is where the process was launched **from**, which for a Windows
service, a scheduled task or a double-clicked shortcut has nothing to do with where the application
was installed.

<xref:Ploch.Common.EnvironmentUtilities.GetCurrentAppPath> answers the question that was actually
being asked:

```csharp
var templatesPath = Path.Combine(EnvironmentUtilities.GetCurrentAppPath(), "Templates");

foreach (var template in Directory.EnumerateFiles(templatesPath, "*.hbs"))
{
    registry.Register(Path.GetFileNameWithoutExtension(template), File.ReadAllText(template));
}
```

It resolves the directory of `Assembly.GetEntryAssembly()?.Location`, falling back to
`AppDomain.CurrentDomain.BaseDirectory`, and throws `InvalidOperationException` if neither yields a
directory. Two consequences follow from that fallback chain.

**Under a test runner**, `Assembly.GetEntryAssembly()` may be `null` — the runner, not your test
assembly, owns the entry point. The fallback keeps the method working; the repository's own tests
are written to accommodate either outcome.

**Under single-file publish**, `Assembly.Location` returns an empty string for a bundled assembly.
The implementation maps that empty string to `null` before the fallback, so the method still returns
the correct directory. Verified on `net8.0`, published with `-p:PublishSingleFile=true`:

```text
Assembly.GetEntryAssembly()?.Location    = ""
AppDomain.CurrentDomain.BaseDirectory    = "…\sf\"
EnvironmentUtilities.GetCurrentAppPath() = "…\sf"
```

Do note that the `Assembly.Location` access still trips the `IL3000` trim analyser in projects
published single-file or trimmed, and the warning is reported against `EnvironmentUtilities.cs` in
the consuming build. Where a warning-free trimmed build matters more than the test-runner fallback,
call `AppContext.BaseDirectory` directly instead.

The returned path has **no trailing directory separator**, unlike `AppContext.BaseDirectory`, which
always ends in one. `Path.Combine` copes with either; string comparisons between the two do not.

### `GetEnvironmentCommandLine` splits on spaces — read this before using it

<xref:Ploch.Common.EnvironmentUtilities.GetEnvironmentCommandLine(System.Boolean)> returns the
process command line, optionally including the application itself:

```csharp
public static IEnumerable<string> GetEnvironmentCommandLine(bool includeApplication = false);
```

It is implemented as `Environment.CommandLine.Split(' ')`, with the first element skipped unless
`includeApplication` is `true`. That is a *character* split, not a command-line parse: it does not
honour quoting. Actual output, running with `--config "C:\Program Files\My App\app.json" --verbose`:

```text
Environment.CommandLine
  …\scratch.dll --config "C:\Program Files\My App\app.json" --verbose

GetEnvironmentCommandLine()
  ["--config", "\"C:\Program", "Files\My", "App\app.json\"", "--verbose"]

Environment.GetCommandLineArgs()
  ["…\scratch.dll", "--config", "C:\Program Files\My App\app.json", "--verbose"]
```

The quoted path became three fragments, and the quote characters were kept. It gets worse when the
*application's own path* contains a space, because `Skip(1)` then drops only the first fragment of
that path and the remainder leaks in as phantom arguments. The same program, run from
`C:\…\claude 204 space dir`:

```text
GetEnvironmentCommandLine()
  ["204", "space", "dir\scratch.dll\"", "--flag"]
```

`204` and `space` are not arguments; they are pieces of the executable's own path.

So: **use `GetEnvironmentCommandLine` only where the arguments and the application path are known
not to contain spaces**, and prefer `Environment.GetCommandLineArgs()` — parsed by the runtime, and
therefore quote-aware — whenever they might. The only thing `GetEnvironmentCommandLine` offers that
`GetCommandLineArgs` does not is a lazily-projected `IEnumerable<string>` that has already dropped
the application, which is a small return for the risk.

A reasonable use is diagnostics, where a one-liner matters more than fidelity:

```csharp
logger.LogInformation("Started with arguments: {Arguments}",
                      string.Join(" ", EnvironmentUtilities.GetEnvironmentCommandLine()));
```

Even there, be aware that a connection string or token passed on the command line will be logged.

## Branching on the operating system

<xref:Ploch.Common.OperatingSystemExtensions> contains a single method:

```csharp
public static bool IsWindows(this OperatingSystem operatingSystem);
```

It returns `true` when `operatingSystem.Platform` is `PlatformID.Win32NT`, and `false` for every
other `PlatformID` — including the legacy Windows members `Win32Windows`, `Win32S` and `WinCE`,
which modern .NET never reports. A `null` argument throws `ArgumentNullException` with parameter
name `operatingSystem`.

Applied to the ambient platform it is a drop-in for the `net5.0`+ static `OperatingSystem.IsWindows()`,
and it works on `netstandard2.0`, where that static does not exist:

```csharp
if (Environment.OSVersion.IsWindows())
{
    ApplyWindowsAclHardening(path);
}
```

But calling it on `Environment.OSVersion` throws away the reason to prefer it. Because it is an
extension on an *instance*, the platform becomes an input you can substitute — which turns "what
happens on Linux?" from a question you answer by deploying into one you answer with a unit test:

```csharp
public interface IPlatform
{
    OperatingSystem OperatingSystem { get; }
}

public sealed class AmbientPlatform : IPlatform
{
    public OperatingSystem OperatingSystem => Environment.OSVersion;
}

public sealed class ConfigDirectoryResolver(IPlatform platform)
{
    public string Resolve(string applicationName) =>
        platform.OperatingSystem.IsWindows()
            ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), applicationName)
            : Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".config", applicationName);
}
```

```csharp
[Fact]
public void Resolve_should_use_a_dot_directory_on_non_windows_platforms()
{
    var platform = Mock.Of<IPlatform>(p => p.OperatingSystem == new OperatingSystem(PlatformID.Unix, new Version()));

    var resolved = new ConfigDirectoryResolver(platform).Resolve("Contoso");

    resolved.Should().Contain(Path.Combine(".config", "Contoso"));
}
```

That test runs, and passes, on a Windows build agent.

Two caveats. `PlatformID` is a coarse instrument: modern .NET reports `PlatformID.Unix` for Linux,
macOS and FreeBSD alike, so `IsWindows() == false` tells you only "not Windows". When those need
telling apart, use `OperatingSystem.IsLinux()` / `IsMacOS()` on `net5.0`+, or
`RuntimeInformation.IsOSPlatform(…)` on `netstandard2.0`. And `IsWindows()` is a plain `bool`, not a
platform *guard* the compiler recognises — it will not silence `CA1416` the way the built-in
`OperatingSystem.IsWindows()` does.

## Processor affinity

The last helper is the most specialised, and the only one with a genuine platform restriction.

`Process.ProcessorAffinity` is an `IntPtr` bitmask: bit *n* set means "this process may run on
processor *n*". Working with it directly means writing shifts and masks at the call site, which is
tedious and easy to get wrong at the boundaries.
<xref:Ploch.Common.Diagnostics.ProcessExtensions> exposes the same capability as processor
*numbers*:

```csharp
public static void SetSingleProcessorAffinity(this Process process, int processorNumber);

public static void SetEnabledProcessors(this Process process, params int[] enabledProcessorsNumbers);

public static IEnumerable<int> GetEnabledProcessors(this Process process);
```

> [!IMPORTANT]
> **Windows and Linux only.** `Process.ProcessorAffinity` throws `PlatformNotSupportedException` on
> other platforms, macOS included, and all three methods go through it. On `net5.0` and later the
> methods carry `[SupportedOSPlatform("windows")]` and `[SupportedOSPlatform("linux")]`, so calling
> them from code that also targets macOS produces a `CA1416` warning — the analyser doing its job,
> not a false positive. Guard the call site, or annotate the caller.

The realistic reason to reach for this is a CPU-bound worker whose scheduling you want to control: a
benchmark harness that must not migrate between cores mid-measurement, an encoder kept away from the
cores serving interactive requests, or a licence-constrained third-party executable that must be
confined to a fixed core count. All three share a shape — start a child process, constrain it, leave
the parent alone:

```csharp
// Windows/Linux only.
public static Process StartPinnedWorker(string executable, string arguments, params int[] processors)
{
    var process = Process.Start(new ProcessStartInfo(executable, arguments)
    {
        UseShellExecute = false,
        CreateNoWindow = true,
    })!;

    try
    {
        process.SetEnabledProcessors(processors);
    }
    catch
    {
        process.Kill();
        throw;
    }

    return process;
}
```

Here is that API exercised for real against a child process on a 32-processor Windows machine
(`IntPtr.Size * 8 == 64`, `Environment.ProcessorCount == 32`):

```text
child.GetEnabledProcessors()                  => [0, 1, 2, …, 31]
child.SetSingleProcessorAffinity(1); then Get => [1]
child.SetEnabledProcessors(0, 2, 3); then Get => [0, 2, 3]
```

### `GetEnabledProcessors` returns a snapshot, not a live view

The name and the `IEnumerable<int>` return type together suggest a lazy iterator over the process's
current mask. It is not one: the mask is read, and the processor numbers materialised into a
`List<int>`, *when the method is called*. Later changes to the affinity are invisible to an
already-returned sequence:

```csharp
child.SetEnabledProcessors(0, 1);

var snapshot = child.GetEnabledProcessors();   // the mask is read here
child.SetSingleProcessorAffinity(5);           // the affinity changes after the call

string.Join(", ", snapshot);                     // "0, 1" — the old mask
string.Join(", ", child.GetEnabledProcessors()); // "5"    — the current mask
```

That output is from an actual run. The eager materialisation is deliberate: it means the argument
guard throws at the call site rather than at first enumeration, and it makes the result safe to
enumerate more than once. Call the method again whenever a fresh reading is wanted.

### Processor numbers are bounded by pointer width, not by processor count

Both setters validate that every processor number falls in `[0, IntPtr.Size * 8)` — 0 to 63 in a
64-bit process, 0 to 31 in a 32-bit one — because that is what the native mask can represent.
Out-of-range values throw `ArgumentOutOfRangeException` with an explicit message:

```text
Processor number must be between 0 and 63 — the exclusive upper bound is the width of the native
processor-affinity mask (64 bits). (Parameter 'processorNumber')
Actual value was 64.
```

What the guard deliberately does **not** do is compare the processor number against
`Environment.ProcessorCount`. That property is a *count* of the processors available to the process
— since .NET 6 it already reflects affinity and container CPU limits — not an upper bound on valid
processor *indices*. A process confined by a cpuset to processors 8–15 reports `ProcessorCount == 8`
while the only legal processor numbers are 8 to 15; validating against the count would reject every
one of them.

The consequence is that a processor number which is in range but does not exist on this machine is
caught *after* the guard has passed rather than by it — and what catches it differs by platform:
Windows refuses the call outright, whereas on Linux the operating system accepts it and the library's
own read-back check is what fails.

**Windows refuses the whole mask.** Requesting processors 0 and 62 on a 32-processor machine throws
`System.ComponentModel.Win32Exception: The parameter is incorrect.`, and — verified by re-reading the
mask afterwards — the process keeps the affinity it had before the call.

**Linux does not refuse it.** `sched_setaffinity(2)` applies the *intersection* of the requested mask
and the processors actually available, so the call would otherwise succeed while silently enabling
fewer processors than were asked for. To close that gap, `SetEnabledProcessors` reads the mask back
after writing it and throws `InvalidOperationException` naming the processors that were dropped:

```text
The operating system did not enable the requested processor(s) 63 — they do not exist on this
machine or are not available to the process.
```

That read-back check is on `SetEnabledProcessors` only. `SetSingleProcessorAffinity` sets a single
bit, where a silent partial application would leave an empty mask that the operating system rejects
outright, so it needs no equivalent.

### Argument guards

Every entry point validates, and the failures are worth knowing because several of them are not the
exception type the signature suggests:

| Call | Result |
|------|--------|
| `((Process)null!).SetSingleProcessorAffinity(0)` | `ArgumentNullException` (`process`) |
| `((Process)null!).SetEnabledProcessors(0)` | `ArgumentNullException` (`process`) |
| `((Process)null!).GetEnabledProcessors()` | `ArgumentNullException` (`process`) — thrown at the call, not at enumeration |
| `process.SetSingleProcessorAffinity(-1)` | `ArgumentOutOfRangeException` (`processorNumber`) |
| `process.SetSingleProcessorAffinity(64)` *(64-bit process)* | `ArgumentOutOfRangeException` (`processorNumber`) |
| `process.SetEnabledProcessors()` | `ArgumentException` (`enabledProcessorsNumbers`): "At least one processor number must be specified." |
| `process.SetEnabledProcessors(null!)` | `ArgumentException` (`enabledProcessorsNumbers`) — **not** `ArgumentNullException` |
| `process.SetEnabledProcessors(0, 200)` | `ArgumentOutOfRangeException` (`enabledProcessorsNumbers`) |
| `new Process().GetEnabledProcessors()` *(never started)* | `InvalidOperationException`: "No process is associated with this object." — from `Process`, after the null guard has passed |

The last row is the one that catches people: a default-constructed `Process` is not `null`, so the
library's guard is satisfied and the failure comes from the BCL when the affinity property is read.

### On Linux this is a *thread* setting, not a process setting

`Process.ProcessorAffinity` is named after the process, and on Windows it behaves that way. On Linux
it does not, because the syscall underneath — `sched_setaffinity(2)` — is per-thread, and .NET passes
the process id, which the kernel interprets as the id of the *main thread*.

The difference is observable. A .NET application with four busy threads already running, whose
affinity is then narrowed to a single processor:

| | Windows | Linux |
|---|---|---|
| Threads running *before* the change | Migrated onto the new mask. Four threads that had been observed across all 32 processors reported **only processor 1** from that moment on. | Unaffected. The main thread moved to processor 1; every other thread already in the process — the four workers, and the runtime's own threads alongside them — kept the full `0-31` mask. |
| Threads created *after* the change | Constrained by the process mask. | Constrained *if created by an already-constrained thread* — a Linux thread inherits its creator's mask. A thread created by the pinned main thread inherited processor 1. |
| What `GetEnabledProcessors` reports | The process mask. | The **main thread's** mask, which may not be the mask of the thread doing the work. |

That is measured behaviour on both platforms — Windows 11 and Ubuntu 24.04, 32 processors each,
on `net9.0` — not an inference from the API shape.

Two practical consequences on Linux. First, pinning is only reliable when it happens **before** the
threads that matter exist — which is why `StartPinnedWorker` above sets the affinity immediately
after `Process.Start`. Treat that as best effort rather than a guarantee: `Process.Start` returns
once the operating system has created the process, not once the child has reached a known point in
its own startup, so any thread the child has already created keeps its original mask and only
threads created after the call inherit the new one. Where every thread must be pinned, the child has
to set its own affinity at entry, or the parent needs a startup handshake — a named
`EventWaitHandle`, or a pipe the child writes to — so it can pin before the child creates anything.
Applied to a process that has been running for a while, it constrains the main thread and leaves the
existing thread pool exactly where it was. Second, if individual threads must be pinned separately,
`Process.ProcessorAffinity` is the wrong API altogether: use `sched_setaffinity` on each thread, or
start the threads from an already-pinned one.

### Pinning the current process

Everything above works on `Process.GetCurrentProcess()` too, and on Windows that is a far bigger
hammer than it looks: the affinity applies to the whole process — every thread, the thread pool, the
garbage collector — for the remainder of its life, and it moves threads that are *already running*.
Confining a server process to one processor to "measure something" also confines everything else it
is doing. On Linux the blast radius is smaller for the reasons above, and so is the benefit: the
measurement thread is only pinned if it is the main thread or was created after the change.

If you must, capture and restore:

```csharp
// Windows/Linux only.
var process = Process.GetCurrentProcess();
var original = process.GetEnabledProcessors().ToArray();   // a snapshot, taken now

try
{
    process.SetSingleProcessorAffinity(original[0]);
    RunSingleThreadedMeasurement();
}
finally
{
    process.SetEnabledProcessors(original);
}
```

Taking the first *enabled* processor rather than hard-coding `0` matters: on a container or a
cpuset-constrained host, processor 0 may not be in the allowed set at all, and requesting it would
fail.

## Quick reference

| I want to… | Use |
|------------|-----|
| Read a configuration string | `EnvironmentVariables.GetString(name)` |
| Read a flag, defaulting when absent or invalid | `EnvironmentVariables.GetBool(name) ?? false` — remembering that `1` is *not* `true` |
| Read an enum, case-insensitively | `EnvironmentVariables.GetEnumValue<TEnum>(name)` — then `Enum.IsDefined` if numeric input must be rejected |
| Read a number | `EnvironmentVariables.GetString(name).ParseToInt32()` |
| Find files deployed alongside the application | `EnvironmentUtilities.GetCurrentAppPath()` |
| Get parsed command-line arguments | `Environment.GetCommandLineArgs()` — **not** `GetEnvironmentCommandLine()`, which splits on spaces |
| Branch on Windows in a testable way | `platform.OperatingSystem.IsWindows()` with an injected `OperatingSystem` |
| Read a process's allowed processors | `process.GetEnabledProcessors()` *(Windows/Linux)* |
| Pin a process to one processor | `process.SetSingleProcessorAffinity(n)` *(Windows/Linux)* |
| Confine a process to a set of processors | `process.SetEnabledProcessors(a, b, c)` *(Windows/Linux)* |
