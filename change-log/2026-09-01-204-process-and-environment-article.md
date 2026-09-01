# #204 — Process and environment utilities article for the documentation site

**Type:** documentation
**Breaking changes:** none

## Summary

Added an authored article covering the four process- and environment-facing helpers in `Ploch.Common` —
`EnvironmentVariables`, `EnvironmentUtilities`, `OperatingSystemExtensions` and
`Ploch.Common.Diagnostics.ProcessExtensions` — to the Articles section of the documentation site. The page is
organised around the questions an application asks about the machine it is running on, and is explicit about
where behaviour is OS-conditional.

## Details

New page `DocumentationSite/articles/process-and-environment.md`, wired into
`DocumentationSite/articles/toc.yml`.

It covers:

- **`EnvironmentVariables`** as a configuration surface, and the two places its permissiveness bites:
  `GetBool` accepts only `true`/`false` (so `1`, `yes` and `on` all yield `null`, which a `?? false` silently
  turns into *off*), and `GetEnumValue` accepts numeric strings and comma-separated name lists, so `99`
  returns an **undefined** `(TEnum)99` rather than `null`. Both are shown with a full value/result table and a
  guarded wrapper. Also notes that `ignoreCase` defaults to `true` here, the opposite of the underlying
  `SafeParseToEnum` string extension, and that a `null` variable name surfaces as the BCL's
  `ArgumentNullException` with parameter name `variable`.
- **`EnvironmentUtilities.GetCurrentAppPath`** — why it beats `Environment.CurrentDirectory` for locating
  deployed sidecar files, its entry-assembly/`AppDomain.BaseDirectory` fallback chain, its behaviour under
  single-file publish (`Assembly.Location` is empty, the `NullIfEmpty` fallback covers it), the resulting
  `IL3000` trim warning in consuming builds, and its lack of a trailing directory separator.
- **`EnvironmentUtilities.GetEnvironmentCommandLine`** — flagged prominently, because it splits
  `Environment.CommandLine` on spaces without honouring quoting. Documented with real output showing a quoted
  path shredded into three fragments, and the worse case where a space in the *application's own* path leaks
  path fragments in as phantom arguments. `Environment.GetCommandLineArgs()` is recommended instead.
- **`OperatingSystemExtensions.IsWindows`** — only `PlatformID.Win32NT` counts; the real reason to prefer it
  over the `net5.0`+ static is that it takes an `OperatingSystem` *instance*, which makes platform branching
  unit-testable on any agent; plus the `netstandard2.0` angle, `PlatformID.Unix` covering Linux/macOS/FreeBSD
  alike, and the fact that it is not a `CA1416` platform guard.
- **`ProcessExtensions`** — Windows/Linux only (`PlatformNotSupportedException` elsewhere, `CA1416` on
  `net5.0`+); a pinned-worker scenario; the pointer-width bound on processor numbers and why
  `Environment.ProcessorCount` is deliberately *not* used as an index bound; the platform split when a
  non-existent processor is requested (Windows rejects the whole mask with `Win32Exception` and leaves the
  previous affinity intact, Linux intersects silently, which is why `SetEnabledProcessors` reads the mask back
  and throws `InvalidOperationException`); that `GetEnabledProcessors` returns an eager `List<int>` snapshot
  rather than a live view; a full argument-guard table including `SetEnabledProcessors(null)` throwing
  `ArgumentException` rather than `ArgumentNullException` and an unstarted `Process` failing inside the BCL;
  and the caution against pinning the current process, with a capture-and-restore pattern.

Every code sample was compiled against the real `Ploch.Common` API in a scratch project, and every
behavioural claim — exception types, parameter names, exception message text, snapshot semantics, the
command-line splitting output, and the single-file publish path resolution — was verified by executing it and
quoting the real output.

No changes to shipped library code — packages are unaffected.

## Review follow-up (PR #334)

Corrections made after review, each re-verified by execution:

- **Empty vs absent environment variables** — the page originally claimed that `GetString`, `GetBool` and
  `GetEnumValue` behave identically for a blank value on every platform, and attributed empty-variable
  handling to a Windows/Linux split. Both are wrong. `GetString` forwards `Environment.GetEnvironmentVariable`
  and can return `""`, while the two parsing methods normalise blank input to `null`; and the
  delete-on-empty behaviour of `Environment.SetEnvironmentVariable(name, "")` is a **.NET 9 breaking change**,
  identical on Windows and Linux, not a platform difference. Measured on Windows 11 and Ubuntu 24.04 under
  both `net8.0` and `net9.0`.
- **Linux processor affinity is per-thread** — `Process.ProcessorAffinity` on Linux goes through
  `sched_setaffinity(2)` for the process id, which the kernel treats as the main thread. Threads already
  running keep their old mask, and `GetEnabledProcessors` reports the main thread's mask. A new section
  documents the Windows/Linux split with measured results and the practical consequence for pinning.
- **`GetFlag` sample** — lower-cases the raw value so `YES`/`ON` are recognised, matching the surrounding text.
- **`RequireDefinedEnum` sample** — reads the raw string first, so "set to nonsense" throws instead of
  silently falling back to the default, which is the principle the section argues for.
