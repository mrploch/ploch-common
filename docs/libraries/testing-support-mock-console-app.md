# Ploch.TestingSupport.MockConsoleApp

> A minimal console application executable used as a test target for console application integration tests.

## Overview

`Ploch.TestingSupport.MockConsoleApp` is a purpose-built stub executable. It provides a concrete console application binary that integration tests or test harnesses can launch as a child process without depending on any real application artefact.

The application does nothing more than write to standard output and then wait before exiting. That behaviour is intentionally trivial: the value of this package is the _existence_ of an independently launchable process, not anything it computes.

Typical uses include:

- Testing code that spawns and monitors child processes (e.g. process-management utilities).
- Verifying that a test scaffold correctly captures standard output from a sub-process.
- End-to-end testing of console application launchers or wrappers that need a real process to target.

## Behaviour

1. Writes `Hello, World! I'm a mock console app that can be used in testing.` to standard output.
2. Then waits, choosing how based on whether standard input is redirected:

   | Standard input | Second line written | Exits when |
   |---|---|---|
   | Redirected (the normal case for a test harness) | `Send a line on standard input to exit.` | A line arrives on standard input |
   | A real console (interactive) | `Press any key to exit.` | A key is pressed |

   The branch matters: `Console.ReadKey` throws `InvalidOperationException` when standard input is redirected, so a stub that always called it would crash under exactly the conditions a test creates. See issue #275.

3. Exits with code `0`.

## Target frameworks

`net10.0` and `net8.0`.

The application is packed **without a native apphost**, so a single package behaves identically on Windows, Linux and macOS. There is no `.exe` — launch it through the `dotnet` muxer.

It is also built with `RollForward=Major`. The default roll-forward policy is `Minor`, which never crosses a major version, so without this the `net8.0` asset would refuse to start on a machine that has only the .NET 9 runtime. With it, a packaged asset binds to the lowest higher major runtime available, which keeps `net9.0` consumers (and future `net11.0` ones) working from the same two assets. Because `dotnet <app>.dll` runs the application inside the `dotnet` process itself, the `Process` handle you get back is the stub's own process, so process inspection and manipulation (affinity, priority, exit codes) behave as expected.

## Installation

```shell
dotnet add package Ploch.TestingSupport.MockConsoleApp
```

The package is published to **GitHub Packages** (`https://nuget.pkg.github.com/mrploch`), not to NuGet.org. Add that feed to your `NuGet.Config` (the repositories in this workspace already do, authenticated with `MRPLOCH_GITHUB_PACKAGES_TOKEN`) or pass it explicitly:

```shell
dotnet add package Ploch.TestingSupport.MockConsoleApp --source https://nuget.pkg.github.com/mrploch/index.json
```

The package payload lives under `tools/<tfm>/` and carries an MSBuild targets file that is imported automatically. On build, the stub is staged into your project's output directory at `MockConsoleApp/`:

```text
bin/Debug/net10.0/MockConsoleApp/Ploch.TestingSupport.MockConsoleApp.dll
bin/Debug/net10.0/MockConsoleApp/Ploch.TestingSupport.MockConsoleApp.deps.json
bin/Debug/net10.0/MockConsoleApp/Ploch.TestingSupport.MockConsoleApp.runtimeconfig.json
```

Nothing is added to your compile references — this package ships a binary to launch, not an API to call. Consumers targeting `net10.0` or later receive the `net10.0` asset; everything else receives the `net8.0` asset, because a `net8.0` asset does not roll forward onto a machine that only has the .NET 10 runtime.

Projects inside this repository consume the stub via `ProjectReference` instead, but `tests/Common.Tests/Ploch.Common.Tests.csproj` stages it to the same `MockConsoleApp/` location, so the resolution code below is identical either way.

## Usage Examples

### Resolving and launching the stub

```csharp
private static ProcessStartInfo MockConsoleApp()
{
    var dll = Path.Combine(
        AppContext.BaseDirectory,
        "MockConsoleApp",
        "Ploch.TestingSupport.MockConsoleApp.dll");

    return new ProcessStartInfo("dotnet", $"\"{dll}\"")
    {
        RedirectStandardInput = true,
        RedirectStandardOutput = true,
        UseShellExecute = false,
        CreateNoWindow = true
    };
}
```

### Capturing standard output

```csharp
[Fact]
public async Task ProcessLauncher_should_capture_stdout()
{
    using var process = Process.Start(MockConsoleApp())!;

    var firstLine = await process.StandardOutput.ReadLineAsync();

    firstLine.Should().Contain("Hello, World!");

    process.StandardInput.WriteLine();      // release the stub
    await process.WaitForExitAsync();
    process.ExitCode.Should().Be(0);
}
```

### Verifying process exit behaviour

```csharp
[Fact]
public void ProcessManager_should_detect_running_process()
{
    using var process = Process.Start(MockConsoleApp())!;
    var manager = new ProcessManager(process.Id);

    manager.IsRunning.Should().BeTrue();

    process.StandardInput.WriteLine();
    process.WaitForExit(millisecondsTimeout: 5000);

    manager.IsRunning.Should().BeFalse();
}
```

Leave the standard input pipe **open** for as long as the process needs to stay alive. Closing it, or never opening it under a console-less host, makes the stub's `Console.ReadLine` return `null` at end-of-stream and the process exits immediately.

## Related Libraries

- [Ploch.TestingSupport.XUnit3](testing-support-xunit3.md) — xUnit v3 helpers for writing the tests that consume this application
