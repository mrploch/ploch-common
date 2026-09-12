# Environment, Processes and the Host Operating System

Four small, focused types cover the questions an application asks about the machine it is running on:

| Type | Namespace | Purpose |
|---|---|---|
| `EnvironmentVariables` | `Ploch.Common` | Read environment variables and convert them to `bool` or an enum. |
| `EnvironmentUtilities` | `Ploch.Common` | The application's own directory and its command-line arguments. |
| `OperatingSystemExtensions` | `Ploch.Common` | Platform test on an `OperatingSystem` instance. |
| `ProcessExtensions` | `Ploch.Common.Diagnostics` | Read and set a process's processor affinity. |

---

## `EnvironmentVariables` — typed environment configuration

```csharp
using Ploch.Common;

public static string? GetString(string variableName)
public static bool? GetBool(string variableName)
public static TEnum? GetEnumValue<TEnum>(string variableName, bool ignoreCase = true) where TEnum : struct, Enum
```

All three return `null` when the variable is absent, and the conversion overloads return `null` when
the value is present but cannot be parsed. That single, uniform "no usable value" signal makes the
null-coalescing operator the natural way to express a default:

```csharp
var verboseLogging = EnvironmentVariables.GetBool("MYAPP_VERBOSE_LOGGING") ?? false;
var logLevel       = EnvironmentVariables.GetEnumValue<LogLevel>("MYAPP_LOG_LEVEL") ?? LogLevel.Information;
var apiBaseAddress = EnvironmentVariables.GetString("MYAPP_API_BASE") ?? "https://api.example.com";
```

`GetEnumValue` ignores case by default, so container and CI environments that write
`MYAPP_LOG_LEVEL=warning` behave the same as those that write `Warning`. Pass `ignoreCase: false` when
the casing is part of the contract.

Because parse failures return `null` rather than throwing, a malformed variable silently falls back to
the default. When a bad value should be loud rather than ignored, check for it explicitly:

```csharp
var rawLevel = EnvironmentVariables.GetString("MYAPP_LOG_LEVEL");

if (rawLevel is not null && EnvironmentVariables.GetEnumValue<LogLevel>("MYAPP_LOG_LEVEL") is null)
{
    throw new InvalidOperationException($"MYAPP_LOG_LEVEL has an unrecognised value: '{rawLevel}'.");
}
```

A common production shape is a feature-flag reader that is checked at start-up and cached:

```csharp
public sealed class FeatureFlags
{
    public bool UseNewPricingEngine { get; } = EnvironmentVariables.GetBool("FEATURE_NEW_PRICING") ?? false;
    public bool EnableDetailedTracing { get; } = EnvironmentVariables.GetBool("FEATURE_TRACING") ?? false;
    public DeploymentRing Ring { get; } = EnvironmentVariables.GetEnumValue<DeploymentRing>("DEPLOYMENT_RING") ?? DeploymentRing.Production;
}
```

`GetBool` is implemented over `StringParsingExtensions.ParseToBool`, so it accepts the values
`bool.TryParse` accepts (`true`/`false`, any casing, surrounding white space) — not `1`/`0` or
`yes`/`no`. Map those yourself if your deployment pipeline emits them.

> For applications built on `Microsoft.Extensions.Configuration`, the configuration system's
> environment-variable provider remains the right choice for hierarchical settings.
> `EnvironmentVariables` is for the cases outside it: start-up code that runs before the host is
> built, console tools, MSBuild-invoked utilities and libraries that must not take a configuration
> dependency.

---

## `EnvironmentUtilities` — the application's own location and arguments

```csharp
public static string GetCurrentAppPath()
public static IEnumerable<string> GetEnvironmentCommandLine(bool includeApplication = false)
```

`GetCurrentAppPath` returns the directory containing the entry assembly, falling back to
`AppDomain.CurrentDomain.BaseDirectory` when the entry assembly reports no location — the situation in
single-file publishes and some test hosts. It throws `InvalidOperationException` if neither can be
resolved.

```csharp
var templatesDirectory = Path.Combine(EnvironmentUtilities.GetCurrentAppPath(), "Templates");
var template = File.ReadAllText(Path.Combine(templatesDirectory, "invoice.html"));
```

> Under a test runner, `Assembly.GetEntryAssembly()` is the runner rather than the assembly under
> test, so the path points at the runner's directory. Resolve test-fixture paths from
> `typeof(SomeTestType).Assembly.Location` — or from
> [`AssemblyExtensions.GetAssemblyDirectory`](reflection.md) — instead.

`GetEnvironmentCommandLine` splits `Environment.CommandLine` on spaces and, by default, drops the
first token (the executable path):

```csharp
var arguments = EnvironmentUtilities.GetEnvironmentCommandLine().ToList();
var withExecutable = EnvironmentUtilities.GetEnvironmentCommandLine(includeApplication: true).ToList();
```

This is the raw command line as the operating system reports it, which is exactly what you want when
`Main(string[] args)` is out of reach — a library, a static initialiser, or a hosted service. It is a
plain space split, so **quoted arguments containing spaces are not preserved as single tokens**. For
real argument parsing use the `args` array passed to `Main`, or a dedicated parser.

---

## `OperatingSystemExtensions`

```csharp
public static bool IsWindows(this OperatingSystem operatingSystem)
```

An extension on `OperatingSystem` that tests `Platform == PlatformID.Win32NT`:

```csharp
using Ploch.Common;

if (Environment.OSVersion.IsWindows())
{
    RegisterWindowsEventLogSink();
}
```

Its value is at a seam: a type that receives an `OperatingSystem` (rather than reading
`Environment.OSVersion` internally) can be unit-tested against a constructed instance:

```csharp
public sealed class InstallPathResolver(OperatingSystem operatingSystem)
{
    public string GetDefaultInstallPath() =>
        operatingSystem.IsWindows()
            ? @"C:\Program Files\MyApp"
            : "/usr/local/myapp";
}

// In a test:
var resolver = new InstallPathResolver(new OperatingSystem(PlatformID.Unix, new Version(5, 15)));
resolver.GetDefaultInstallPath().Should().Be("/usr/local/myapp");
```

For a runtime check with no seam involved, `System.OperatingSystem.IsWindows()` (.NET 5+) and
`RuntimeInformation.IsOSPlatform(OSPlatform.Windows)` remain the idiomatic choices — and only the
former participates in platform-compatibility analysis (CA1416).

---

## `ProcessExtensions` — processor affinity

```csharp
using Ploch.Common.Diagnostics;

public static void SetSingleProcessorAffinity(this Process process, int processorNumber)
public static void SetEnabledProcessors(this Process process, params int[] enabledProcessorsNumbers)
public static IEnumerable<int> GetEnabledProcessors(this Process process)
```

`Process.ProcessorAffinity` is an `IntPtr` bitmask. These extensions let you work in processor
*numbers* instead, and add the validation and verification that the raw property does not perform.

```csharp
using var process = Process.GetCurrentProcess();

// Pin a latency-sensitive worker to a single core.
process.SetSingleProcessorAffinity(2);

// Or reserve a specific subset for it.
process.SetEnabledProcessors(0, 1, 2, 3);

// Read back what the operating system actually granted.
var cores = process.GetEnabledProcessors().ToList();
_logger.LogInformation("Process is bound to processors {Processors}", string.Join(", ", cores));
```

A realistic use is confining a CPU-bound batch job so that it cannot starve the interactive workload
on the same host:

```csharp
public void RestrictToBackgroundCores(Process batchProcess, int totalCores)
{
    // Leave cores 0 and 1 for the foreground service.
    var backgroundCores = Enumerable.Range(2, totalCores - 2).ToArray();

    batchProcess.SetEnabledProcessors(backgroundCores);
    batchProcess.PriorityClass = ProcessPriorityClass.BelowNormal;
}
```

### Behaviour and limits worth knowing

- **Windows and Linux only.** `Process.ProcessorAffinity` throws `PlatformNotSupportedException`
  elsewhere (macOS included). On .NET 5 and later the methods carry `[SupportedOSPlatform]`
  attributes, so the platform-compatibility analyser will flag an unguarded call.
- **The mask is pointer-sized.** Processor numbers must be below `IntPtr.Size * 8` — 32 in a 32-bit
  process, 64 in a 64-bit one. Anything outside that (or negative) throws
  `ArgumentOutOfRangeException`.
- **Processor numbers are not validated against `Environment.ProcessorCount`.** That property is a
  *count*, not an index bound, and since .NET 6 it already reflects the process's own affinity and CPU
  limits — a process constrained to processors 8–15 may legitimately target processor 12. Requesting a
  processor that does not exist is rejected by the operating system when the affinity is applied.
- **`SetEnabledProcessors` verifies what was applied.** Linux's `sched_setaffinity(2)` applies the
  *intersection* of the requested mask with the processors available to the process, so a request
  mixing available and unavailable processors would otherwise succeed only partially and silently.
  The method reads the mask back and throws `InvalidOperationException` naming the processors that
  were not granted. Windows rejects such masks outright.
- **An empty processor list throws `ArgumentException`** — an empty affinity mask is never meaningful.
- **`GetEnabledProcessors` reports every set bit**, in ascending order, and is deliberately not capped
  by `Environment.ProcessorCount`, so a non-contiguous processor set is reported exactly as it is.

Setting affinity requires sufficient privilege for the target process; expect a
`System.ComponentModel.Win32Exception` (Windows) or `UnauthorizedAccessException` when operating on a
process you do not own.

## See also

- [Argument validation](argument-validation.md)
- [Utility toolbox — paths, hashing, dates and sizes](utility-toolbox.md)
- [`Ploch.Common.Diagnostics` API reference](../api/Ploch.Common.Diagnostics.html)
