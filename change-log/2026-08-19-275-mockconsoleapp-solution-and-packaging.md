## MockConsoleApp: solution membership, redirected-stdin crash, and a consumable package

### Fixed

- **`Ploch.TestingSupport.MockConsoleApp` is now a member of `Ploch.Common.slnx`.** It never had been, on any revision of that file, despite `Ploch.Common.Tests` carrying a `ProjectReference` to it. `dotnet build` was unaffected because MSBuild traverses `ProjectReference` edges transitively, so CI stayed green; Rider and Visual Studio build only solution members and reported `CS0006: Metadata file '…\obj\Debug\{net10.0,net8.0}\ref\Ploch.TestingSupport.MockConsoleApp.dll' could not be found`. Six other solutions across the workspace already listed the project — only its own home solution omitted it.

- **The stub no longer crashes when standard input is redirected.** `Program.cs` ended in `Console.ReadKey()`, which throws `InvalidOperationException` under redirected stdin — the normal condition when a test harness launches a child process, and precisely what the documented usage example did. The app now branches on `Console.IsInputRedirected`, waiting on `Console.ReadLine()` when redirected and `Console.ReadKey()` when attached to a real console. Previously such a launch died with exit code `-532462766` and an unhandled exception on stderr.

### Changed

- **The NuGet package is now actually consumable.** Build output is packed to `tools/<tfm>/` instead of `lib/`, so a `PackageReference` yields a launchable binary rather than a compile-time reference to an `Exe`. The package carries `build/Ploch.TestingSupport.MockConsoleApp.targets`, imported automatically, which stages the stub into the consumer's output directory at `MockConsoleApp/`. Consumers on `net10.0` or later receive the `net10.0` asset and everything else the `net8.0` asset, because a `net8.0` asset does not roll forward onto a machine that only has the .NET 10 runtime.

- **`UseAppHost=false`.** A single package now behaves identically on Windows, Linux and macOS; the stub is launched through the `dotnet` muxer rather than as a platform-specific `.exe`. Previously the packed apphost would have been baked for whichever OS ran the build.

- **`Ploch.Common.Tests` stages the stub to the same `MockConsoleApp/` location** via a `StageMockConsoleApp` target, so in-repo `ProjectReference` consumers and external `PackageReference` consumers resolve it identically. Its reference is now `ReferenceOutputAssembly="false"` — the stub is launched, never compiled against.

- **`ProcessExtensionsTests` no longer hardcodes a build path.** It previously reached into `../../../../../src/TestingSupport.MockConsoleApp/bin/Debug/net10.0/Ploch.TestingSupport.MockConsoleApp.exe`, which pinned the `Debug` configuration, the `net10.0` leg and the `.exe` extension. It now resolves the staged stub from `AppContext.BaseDirectory` and holds the process open through a live stdin pipe rather than relying on `ReadKey` blocking.

- **A package README is included**, resolving the *"missing a readme"* pack warning.

### Documentation

- Rewrote `docs/libraries/testing-support-mock-console-app.md`. The previous version documented a `tools` directory that the package did not produce, an installation route that could not work (the package is not on NuGet.org), and a `net10.0`-only target that had been `net10.0;net8.0` since #209. Its second usage example redirected standard input and would have crashed the stub.

### Notes for maintainers

Two packaging approaches were rejected. `PackAsTool` produces a `DotnetTool`-typed package installed with `dotnet tool install` and invoked by command name, which contradicts the documented `dotnet add package` workflow, and on .NET 10 it fans out into RID-specific packages with strict publish ordering. The `IsTool` property flattens every target framework into a single `tools/` folder, so on this multi-targeted project the `net8.0` output collided with `net10.0` and was silently dropped (`NU5118`). The per-TFM layout is produced by a `TargetsForTfmSpecificContentInPackage` hook instead.

### Refs

- #275 (MockConsoleApp: missing from solution, crashes on redirected stdin, ships an unusable package)
- Related to #266 (secondary solution files referencing removed projects — the inverse problem)
- Builds on #207 / #209 (the multi-targeting that this packaging has to accommodate)
