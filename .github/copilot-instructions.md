# Copilot instructions

Read [`CLAUDE.md`](../CLAUDE.md) and the task-relevant documents under [`docs/ai/`](../docs/ai/README.md) before working
in this repository. They are the single source of agent guidance here; the overview below is a short summary.

## Repository overview

This repository contains the Ploch.Common family of .NET utility libraries.

- Primary solution: `Ploch.Common.slnx`
- Main targets: `netstandard2.0`, `net8.0`, and `net9.0` depending on the project
- Core library: `src/Common/Ploch.Common.csproj`
- Main patterns: extension-method utilities, serialization abstractions, `ServicesBundle` dependency injection modules,
  and modular implementation-plus-DI-extension packages

## Build and test commands

- Restore: `dotnet restore`
- Build whole solution: `dotnet build Ploch.Common.slnx`
- Build a single project: `dotnet build <path-to-csproj>`
- Run all tests: `dotnet test`
- Run a single test project: `dotnet test tests/Common.Tests/Ploch.Common.Tests.csproj`
- Run filtered tests: `dotnet test --filter "FullyQualifiedName~SomeTestName"`

## Quality bar

- Keep nullable reference types clean.
- Do not introduce architecture drift between the base implementation packages and the `.ExtensionsDependencyInjection`
  companion packages.
- Preserve existing patterns around `ISerializer`, `IAsyncSerializer`, `ServicesBundle`, and extension-based APIs unless
  the task explicitly requires a refactor.
- Prefer minimal, local changes over broad cross-project edits unless the change really spans multiple packages.

## Testing conventions

- Use xUnit and FluentAssertions.
- Prefer `[Theory]` whenever practical.
- Use this naming convention for test methods: `MethodName_should_explain_what_it_should_do()`
- For new tests, keep Arrange/Act/Assert structure clear.

## Validation expectations

- Before finishing, run the most relevant tests for the touched projects.
- If a change affects shared code or public abstractions, favor solution-level validation.
- If you cannot run a needed validation step, say so explicitly and explain what is still unverified.