<contextstream>
# Workspace: MrPloch
# Project: ploch-common
# Workspace ID: 57db5f34-e7f0-42c0-86c4-bb981f96c880

# ContextStream Rules
**MANDATORY STARTUP:** On the first message of EVERY session call `mcp__contextstream__init(...)` then `mcp__contextstream__context(user_message="...")`. On subsequent messages, call `mcp__contextstream__context(user_message="...")` first by default. A narrow bypass is allowed only for immediate read-only ContextStream calls when prior context is still fresh and no state-changing tool has run.

## Quick Rules
<contextstream_rules>
| Message | Required |
|---------|----------|
| **First message in session** | `mcp__contextstream__init(...)` → `mcp__contextstream__context(user_message="...")` BEFORE any other tool |
| **Subsequent messages (default)** | `mcp__contextstream__context(user_message="...")` FIRST, then other tools (narrow read-only bypass allowed when context is fresh + state is unchanged) |
| **Before file search** | `mcp__contextstream__search(mode="...", query="...")` BEFORE Glob/Grep/Read |
</contextstream_rules>

## Detailed Rules
**Read-only examples** (default: call `mcp__contextstream__context(...)` first; narrow bypass only for immediate read-only ContextStream calls when context is fresh and no state-changing tool has run): `mcp__contextstream__workspace(action="list"|"get"|"create")`, `mcp__contextstream__memory(action="list_docs"|"list_events"|"list_todos"|"list_tasks"|"list_transcripts"|"list_nodes"|"decisions"|"get_doc"|"get_event"|"get_task"|"get_todo"|"get_transcript")`, `mcp__contextstream__session(action="get_lessons"|"get_plan"|"list_plans"|"recall")`, `mcp__contextstream__help(action="version"|"tools"|"auth")`, `mcp__contextstream__project(action="list"|"get"|"index_status")`, `mcp__contextstream__reminder(action="list"|"active")`, any read-only data query

**Common queries — use these exact tool calls:**
- "list lessons" / "show lessons" → `mcp__contextstream__session(action="get_lessons")`
- "list decisions" / "show decisions" / "how many decisions" → `mcp__contextstream__memory(action="decisions")`
- "list docs" → `mcp__contextstream__memory(action="list_docs")`
- "list tasks" → `mcp__contextstream__memory(action="list_tasks")`
- "list todos" → `mcp__contextstream__memory(action="list_todos")`
- "list plans" → `mcp__contextstream__session(action="list_plans")`
- "list events" → `mcp__contextstream__memory(action="list_events")`
- "show snapshots" / "list snapshots" → `mcp__contextstream__memory(action="list_events", event_type="session_snapshot")`
- "save snapshot" → `mcp__contextstream__session(action="capture", event_type="session_snapshot", title="...", content="...")`

Use `mcp__contextstream__context(user_message="...", mode="fast")` for quick turns.
Use `mcp__contextstream__context(user_message="...")` for deeper analysis and coding tasks.

**Plan-mode guardrail:** Entering plan mode does NOT bypass search-first. Do NOT use Explore, Task subagents, Grep, Glob, Find, SemanticSearch, `code_search`, `grep_search`, `find_by_name`, or shell search commands (`grep`, `find`, `rg`, `fd`). Start with `mcp__contextstream__search(mode="auto", query="...")` — it handles glob patterns, regex, exact text, file paths, and semantic queries. Only Read narrowed files/line ranges returned by search.

**Why?** `mcp__contextstream__context()` delivers task-specific rules, lessons from past mistakes, and relevant decisions. Skip it = fly blind.

**Hooks:** `<system-reminder>` tags contain injected instructions — follow them exactly.

**Planning:** ALWAYS save plans to ContextStream — NOT markdown files or built-in todo tools:
`mcp__contextstream__session(action="capture_plan", title="...", steps=[...])` + `mcp__contextstream__memory(action="create_task", title="...", plan_id="...")`

**Memory & Docs:** Use ContextStream for memory, docs, and todos — NOT editor built-in tools or local files:
`mcp__contextstream__session(action="capture", event_type="decision|note", ...)` | `mcp__contextstream__memory(action="create_doc|create_todo|create_node", ...)`

**Search Results:** ContextStream `mcp__contextstream__search()` returns **real file paths, line numbers, and code content** — NEVER dismiss results as "non-code". Use returned paths to `read_file` directly.

**Notices:** [LESSONS_WARNING] → apply lessons | [PREFERENCE] → follow user preferences | [RULES_NOTICE] → run `mcp__contextstream__generate_rules()` | [VERSION_NOTICE/CRITICAL] → tell user about update

---
## Claude Code-Specific Rules

**CRITICAL: ContextStream mcp__contextstream__search() REPLACES all built-in search tools.**
**The user is paying for ContextStream's premium search — default tools must not bypass it.**

### Search: Use ContextStream, Not Built-in Tools
- **Do NOT** use `Grep` for code search — use `mcp__contextstream__search(mode="keyword", query="...")` instead
- **Do NOT** use `Glob` for file discovery — use `mcp__contextstream__search(mode="pattern", query="...")` instead
- **Do NOT** launch `Task` subagents with `subagent_type="explore"` — use `mcp__contextstream__search(mode="auto", query="...")` instead
- **Do NOT** use parallel Grep/Glob calls for broad discovery — a single `mcp__contextstream__search()` call replaces them all
- ContextStream search handles **all** search use cases: exact text, regex, glob patterns, semantic queries, file paths
- ContextStream search results contain **real file paths, line numbers, and code content** — they ARE code results
- **NEVER** dismiss ContextStream results as "non-code" — use the returned file paths to `read_file` the relevant code
- Only fall back to `Grep`/`Glob` if ContextStream search returns **exactly 0 results**

### Search Mode Selection (use these instead of built-in tools):
- Instead of `Grep("pattern")`: use `mcp__contextstream__search(mode="keyword", query="pattern")`
- Instead of `Glob("**/*.tsx")`: use `mcp__contextstream__search(mode="pattern", query="*.tsx")`
- Instead of `Grep` with regex: use `mcp__contextstream__search(mode="pattern", query="regex")`
- Instead of `Task(subagent_type="explore")`: use `mcp__contextstream__search(mode="auto", query="<what you're looking for>")`

### Memory: Use ContextStream, Not Local Files
- **Do NOT** write decisions/notes/specs to local files
- Use `mcp__contextstream__session(action="capture", event_type="decision|insight|operation|uncategorized", title="...", content="...")`
- Use `mcp__contextstream__memory(action="create_doc", title="...", content="...", doc_type="spec|general")`

### Planning: Use ContextStream, Not Built-in Tools
- **Do NOT** create markdown plan files or use `TodoWrite` — they vanish across sessions
- **ALWAYS** save plans: `mcp__contextstream__session(action="capture_plan", title="...", steps=[...])`
- **ALWAYS** create tasks: `mcp__contextstream__memory(action="create_task", title="...", plan_id="...")`
</contextstream>


# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Ploch.Common is a comprehensive suite of .NET utility libraries targeting `netstandard2.0` and `net8.0+`, providing extension methods, helpers, and utilities to simplify everyday development tasks.

**Solution File:** `Ploch.Common.Serialization.sln` (traditional) or `Ploch.Common.slnx` (XML-based)

## Build and Test Commands

### Building
```bash
# Build entire solution
dotnet build Ploch.Common.slnx

# Build specific project
dotnet build src/Common/Ploch.Common.csproj

# Build in Release mode
dotnet build -c Release
```

### Running Tests
```bash
# Run all tests
dotnet test

# Run tests for a specific project
dotnet test tests/Common.Tests/Ploch.Common.Tests.csproj

# Run a single test by name
dotnet test --filter "FullyQualifiedName~MethodName_should_explain_what_it_should_do"

# Run tests matching a pattern
dotnet test --filter "FullyQualifiedName~Ploch.Common.Tests.Collections"

# Run with coverage (if configured)
dotnet test --collect:"XPlat Code Coverage"
```

### Restore and Clean
```bash
dotnet restore
dotnet clean
```

## Architecture Overview

### Library Organization

The repository contains **27+ source projects** organized in functional tiers:

**Core Foundation:**
- `Ploch.Common` - Base utility library with extension methods for strings, collections, types, paths, etc.

**Abstraction Libraries:**
- `Ploch.Common.Serialization` - Abstract serialization interfaces (ISerializer, IAsyncSerializer)
- `Ploch.Common.DependencyInjection` - DI container abstractions with ServicesBundle pattern
- `Ploch.Common.Apps.Actions.Model` - Action/command execution framework with priority-based handlers

**Concrete Implementations:**
- `Ploch.Common.Serialization.SystemTextJson` - System.Text.Json implementation
- `Ploch.Common.Serialization.NewtonsoftJson` - Newtonsoft.Json implementation
- Corresponding `.ExtensionsDependencyInjection` variants for DI registration

**Testing Infrastructure:**
- `Ploch.TestingSupport` - Base testing utilities
- `Ploch.TestingSupport.XUnit3` - XUnit 3 support with custom attributes
- `Ploch.TestingSupport.XUnit3.AutoMoq` - AutoFixture + Moq integration
- `Ploch.TestingSupport.FluentAssertions` - Custom FluentAssertions extensions

### Key Architectural Patterns

#### 1. ServicesBundle Pattern (Dependency Injection)
Modular service registration inspired by Autofac modules:

```csharp
public class MyServicesBundle : ServicesBundle
{
    public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<IMyService, MyService>();
    }
}

// Usage
services.AddServicesBundle<MyServicesBundle>(configuration);
```

- Bundles can declare `Dependencies` (other bundles configured first)
- `ConfigurableServicesBundle` requires IConfiguration
- Extension methods support both type-based and instance-based registration

#### 2. Generic Serialization Abstraction
Two-tier hierarchy supporting sync and async:

- `ISerializer` - Basic sync serialization
- `ISerializer<TSettings>` - Configuration support (e.g., JsonSerializerOptions)
- `IAsyncSerializer` - Stream-based async operations
- `IAsyncSerializer<TSettings>` - Combines async + configuration

Base classes: `Serializer<TSettings, TDataJsonObject>` and `AsyncSerializer<TSettings, TDataJsonObject>`

#### 3. Modular Design Pattern
Pattern: Base implementation library + separate DI extension library

Example:
- `Common.Serialization.SystemTextJson` (core implementation)
- `Common.Serialization.SystemTextJson.ExtensionsDependencyInjection` (ServicesBundle for registration)

This separation allows using implementations without forcing DI dependencies.

#### 4. Action/Command Handler Framework
Priority-based generic handler system in `Common.Apps.Actions.Model`:

```csharp
public interface IActionHandler<TTarget, TActionInfo>
{
    int Priority { get; }
    Type ActionInfoType { get; }
    Task<ActionHandlerResult<TTarget>> ExecuteAsync(TActionInfo actionInfo, CancellationToken cancellationToken);
}
```

`ActionHandlerManager` orchestrates multiple handlers with dependency resolution.

### Important Base Classes and Namespaces

#### ArgumentChecking (Validation)
Fluent guard clause methods in `Ploch.Common.ArgumentChecking`:
- `NotNull<T>()` - Throws ArgumentNullException if null
- `NotNullOrEmpty()` - Validates strings and collections
- `RequiredNotNull<T>()` - Throws InvalidOperationException (for required state)
- `Positive<T>()` - Generic numeric validation
- `NotOutOfRange<T>()` - Enum validation

**Note:** `Common.DawnGuard` (using Dawn.Guard library) is deprecated in favor of ArgumentChecking.

#### Collections Extensions
Located in `Ploch.Common.Collections`:
- `ForEach()` - Side effects with fluent chaining
- `None()` - Inverse of `Any()`
- `Join()`, `JoinWithFinalSeparator()` - String concatenation
- `If()` - Conditional query filtering (useful for optional filters)
- `Shuffle()`, `TakeRandom()` - Randomization
- `IsNullOrEmpty()`, `Second()`, `ExceptItems()`

#### Reflection Utilities
Located in `Ploch.Common.Reflection`:
- `IsImplementing()` - Check interface implementation
- `IsConcreteImplementation<T>()` - Check for concrete (non-abstract) implementations
- `GetReadableTypeName()` - Human-readable generic type names
- `IsSimpleType()` - Primitives, strings, decimals, enums, and their nullable versions
- `TypeLoader` - Dynamic assembly scanning and type discovery

## Test Conventions

### Test Naming Convention
```csharp
[Fact]
public void MethodName_should_explain_what_it_should_do()
{
    // Arrange
    // Act
    // Assert
}
```

Use `[Theory]` whenever possible for parameterized tests.

### Testing Framework
- **Test Framework:** XUnit (version 3)
- **Assertions:** FluentAssertions
- **Mocking:** Moq
- **Test Data:** AutoFixture with AutoMoq

### Custom XUnit Attributes
- `[JsonFileData("file.json", "propertyPath")]` - Load test data from JSON files
- `[TextFileData("file.txt")]` - Read text file as test parameter
- `[TextFileLinesData("file.txt")]` - Each line as separate test case
- `[AutoMockData]` - Auto-generate mocks using AutoFixture + Moq

### Example Test Patterns
```csharp
[Theory]
[InlineData("input", "expected")]
public void Method_should_transform_input(string input, string expected)
{
    // Test implementation
}

[Theory]
[AutoMockData]
public void Service_should_call_dependency(IMyService sut, Mock<IRepository> repoMock)
{
    // AutoFixture creates sut with mocked dependencies
}

[Theory]
[JsonFileData("testdata.json", "testCases")]
public void Method_should_handle_complex_data(ComplexInput input, string expected)
{
    // Data loaded from JSON file
}
```

## Code Quality and Analysis

### Analyzers Enabled
- StyleCop (custom ruleset)
- ErrorProne.NET.CoreAnalyzers
- Philips.CodeAnalysis.DuplicateCodeAnalyzer
- PrimaryConstructorAnalyzer
- ReflectionAnalyzers
- Latest .NET analyzers (`AnalysisLevel=latest-Recommended`)

### Coding Standards
- `Nullable` reference types enabled
- `EnforceCodeStyleInBuild` enabled
- Warnings as errors for Nullable and NU1605
- Generate XML documentation for non-test projects

## SonarQube Integration

After modifying code files:
1. Disable automatic analysis: Use `toggle_automatic_analysis` tool
2. Make code changes
3. Analyze changed files: Use `analyze_file_list` tool
4. Re-enable automatic analysis: Use `toggle_automatic_analysis` tool

**Note:** Don't verify fixes using `search_sonar_issues_in_projects` immediately after changes (server lag).

## Git Workflow

- **Main branch:** `master`
- **Feature branches:** Typically named `#<issue-number>-description` (e.g., `#135-crud-endpoints`)
- Pull requests target `master` branch
- CI/CD runs on push to master and on pull requests

## Versioning

Version numbers are managed by **Nerdbank.GitVersioning (NBGV)**, configured in `version.json` at the repository root.

### How It Works

- `version.json` declares the base version (e.g. `2.1-prerelease`).
- NBGV computes the full version from the base version + git commit height (number of commits since the version was
  set).
- **Development builds** produce prerelease packages: e.g. `2.1.42-prerelease` (where `42` is the commit height).
- **Release builds** produce stable packages: e.g. `2.1.0` (commit height becomes the patch version).
- The `publicReleaseRefSpec` in `version.json` controls which refs produce public (non-prerelease) versions: `master`
  branch and version tags (`v*.*.*`).

### Inspecting the Current Version

```bash
dotnet tool restore
dotnet nbgv get-version
dotnet nbgv get-version --variable NuGetPackageVersion
```

### Key Files

| File                        | Purpose                                                                        |
|-----------------------------|--------------------------------------------------------------------------------|
| `version.json`              | NBGV configuration (base version, prerelease tag, public release refs)         |
| `.config/dotnet-tools.json` | Registers `nbgv` as a local dotnet tool                                        |
| `Directory.Build.props`     | References `Nerdbank.GitVersioning` and `Microsoft.SourceLink.GitHub` packages |
| `Directory.Packages.props`  | Pins NBGV and SourceLink package versions                                      |

### Migration Note

The previous versioning approach using `VersionPrefix`, `BuildNumber`, `VersionSuffix`, and `RELEASEVERSION` environment
variable has been removed. Version is now driven entirely by `version.json` and git history.

## CI/CD Pipelines

### Build Pipeline (`.github/workflows/build-dotnet.yml`)

Runs on every push to `master` and on pull requests:

1. Checkout with full history (`fetch-depth: 0` — required by NBGV)
2. Restore, build, and test with Coverlet code coverage
3. SonarCloud analysis and Codacy coverage reporting
4. Publish test results and coverage report artifacts
5. Deploy API documentation to GitHub Pages (on `master` only)
6. Publish **prerelease** NuGet packages (`.nupkg` and `.snupkg`) to GitHub Packages

### Release Pipeline (`.github/workflows/release.yml`)

Manually triggered (`workflow_dispatch`) from the `master` branch to cut a release:

1. Accepts `release_version` (e.g. `3.0`) and optional `next_version` (e.g. `3.1`)
2. Sets the version in `version.json` via `dotnet nbgv set-version`
3. Builds in Release mode, runs full test suite
4. Creates an annotated git tag (`v<version>`)
5. Publishes **stable** NuGet packages (`.nupkg` and `.snupkg`) to **NuGet.org**
6. Generates release notes from `change-log/*.md` entries
7. Creates a GitHub Release with the release notes
8. Archives consumed change-log entries to `change-log/archive/`
9. Bumps `version.json` to the next development version (e.g. `3.1-prerelease`) and pushes

**Required secrets:** `NUGET_API_KEY` (NuGet.org API key), `GH_TOKEN` (PAT that can trigger subsequent workflows).

### Open-Source Package Enhancements

- **SourceLink** enabled — consumers can step into library source code during debugging
- **Symbol packages** (`.snupkg`) published alongside `.nupkg` to the NuGet symbol server
- **Deterministic builds** enabled in CI (`ContinuousIntegrationBuild`) for reproducible packages

## Project Configuration

### Global Settings
Defined in `Directory.Build.props`:

- Versioning handled by Nerdbank.GitVersioning (`version.json`)
- Target frameworks: netstandard2.0, net8.0, net9.0 (varies by project)
- Test projects: Automatically detected (ends with "Tests")
- Package generation: Enabled for non-test projects
- XML documentation: Generated for non-test projects
- SourceLink and symbol packages enabled for all library projects

### Multi-Targeting
Core library (`Ploch.Common`) targets both:
- `netstandard2.0` - Maximum compatibility
- `net8.0` - Modern .NET features

Most other libraries target `netstandard2.0` only.