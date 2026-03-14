# Ploch.TestingSupport.MockConsoleApp

> A minimal console application executable used as a test target for console application integration tests.

## Overview

`Ploch.TestingSupport.MockConsoleApp` is a purpose-built stub executable. It provides a concrete console application binary that integration tests or test harnesses can launch as a child process without depending on any real application artefact.

The application does nothing more than write two lines to standard output and then wait for a key press before exiting. That behaviour is intentionally trivial: the value of this package is the _existence_ of an independently launchable process, not anything it computes.

Typical uses include:

- Testing code that spawns and monitors child processes (e.g. process-management utilities).
- Verifying that a test scaffold correctly captures standard output from a sub-process.
- End-to-end testing of console application launchers or wrappers that need a real executable to target.

The application targets `net10.0` and produces a self-contained executable when published.

## Installation

```shell
dotnet add package Ploch.TestingSupport.MockConsoleApp
```

After installation, the `Ploch.TestingSupport.MockConsoleApp.exe` (or the platform-equivalent binary) is available in the package's tools directory and can be referenced in test code by path.

## Usage Examples

### Launching the mock app from a test

```csharp
[Fact]
public async Task ProcessLauncher_should_capture_stdout()
{
    // The executable path would typically be resolved from the NuGet package
    // or from a known relative path in the test project output directory.
    var executablePath = Path.Combine(
        AppContext.BaseDirectory,
        "Ploch.TestingSupport.MockConsoleApp.exe");

    var launcher = new ProcessLauncher(executablePath);
    var output = await launcher.CaptureOutputAsync();

    output.Should().Contain("Hello, World!");
}
```

### Verifying process exit behaviour

```csharp
[Fact]
public void ProcessManager_should_detect_running_process()
{
    var startInfo = new ProcessStartInfo(executablePath)
    {
        RedirectStandardInput = true,
        UseShellExecute = false
    };
    using var process = Process.Start(startInfo);
    var manager = new ProcessManager(process!.Id);

    manager.IsRunning.Should().BeTrue();

    // Simulate key press to allow the process to exit
    process.StandardInput.Write('\n');
    process.WaitForExit(timeoutMilliseconds: 2000);

    manager.IsRunning.Should().BeFalse();
}
```

## Related Libraries

- [Ploch.TestingSupport.XUnit3](testing-support-xunit3.md) — xUnit v3 helpers for writing the tests that consume this application
