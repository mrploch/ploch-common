# Ploch.TestingSupport.MockConsoleApp

A minimal console application used as a launch target in process-related integration tests.

The value of this package is the *existence* of an independently launchable process, not anything
it computes. Use it to test code that spawns, supervises or captures output from child processes,
without depending on a real application artefact.

## What it does

1. Writes `Hello, World! I'm a mock console app that can be used in testing.` to standard output.
2. Waits, then exits:
   - when standard input is **redirected** (the normal case for a test harness) it waits for a
     line on stdin, so your test can end it cleanly with `StandardInput.WriteLine()`;
   - when run **interactively** it waits for a key press.

## Installation

```shell
dotnet add package Ploch.TestingSupport.MockConsoleApp \
    --source https://nuget.pkg.github.com/mrploch/index.json
```

The package is published to GitHub Packages, not NuGet.org, so the feed must be configured in
your `NuGet.Config` or passed with `--source` as above.

The package payload lives under `tools/` and is staged into your project's output directory at
`MockConsoleApp/` by an auto-imported MSBuild targets file. Nothing is added to your compile
references.

The application is packed without a native apphost, so a single package behaves identically on
Windows, Linux and macOS. Launch it through the `dotnet` muxer rather than as an `.exe`.

## Usage

```csharp
private static ProcessStartInfo MockConsoleApp() =>
    new("dotnet", $"\"{Path.Combine(AppContext.BaseDirectory, "MockConsoleApp", "Ploch.TestingSupport.MockConsoleApp.dll")}\"")
    {
        RedirectStandardInput = true,
        RedirectStandardOutput = true,
        UseShellExecute = false
    };
```

### Capturing standard output

```csharp
[Fact]
public async Task ProcessLauncher_should_capture_stdout()
{
    using var process = Process.Start(MockConsoleApp())!;

    var firstLine = await process.StandardOutput.ReadLineAsync();

    firstLine.Should().Contain("Hello, World!");

    process.StandardInput.WriteLine();
    await process.WaitForExitAsync();
}
```

### Observing a running process

```csharp
[Fact]
public void ProcessManager_should_detect_running_process()
{
    using var process = Process.Start(MockConsoleApp())!;
    var manager = new ProcessManager(process.Id);

    manager.IsRunning.Should().BeTrue();

    process.StandardInput.WriteLine();   // release the stub
    process.WaitForExit(millisecondsTimeout: 5000);

    manager.IsRunning.Should().BeFalse();
}
```

## Target frameworks

The application is built with `RollForward=Major`, so a packaged asset binds to the lowest
higher major runtime when its own major is not installed. Without it the default `Minor`
policy would refuse to cross a major version and a `net9.0`-only machine could not start the
stub at all.

`net10.0` and `net8.0`. The targets file stages the `net10.0` asset into `net10.0`-or-later
consumers and the `net8.0` asset into everything else, because a `net8.0` asset does not roll
forward onto a machine that only has the .NET 10 runtime.

## Related

- [Ploch.TestingSupport.XUnit3](https://common.github.ploch.dev/) — xUnit v3 helpers for writing
  the tests that consume this application.
