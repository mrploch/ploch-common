# Articles

Worked examples and usage patterns for the `Ploch.Common` libraries. Each article covers one namespace
or theme, explains *why* the types exist, and shows them in scenarios drawn from production code
rather than in isolation.

Every code sample is written against the current public API. Where behaviour is surprising — a default
that differs between overloads, a method that throws where you might expect `null`, a difference
between the `netstandard2.0` and `net8.0` builds — the article says so explicitly.

For the generated per-type reference, see the [API documentation](../api/index.md). For installation
and a first tour, see [Getting Started](../docs/GETTING_STARTED.md).

## `Ploch.Common`

| Article | Covers |
|---|---|
| [Ploch.Common overview](samples.md) | What the core library contains, and where to start. |
| [Argument validation](argument-validation.md) | `Ploch.Common.ArgumentChecking` — `Guard` and `PathGuard` fluent guard clauses, and the argument-versus-state distinction. |
| [Collections and enumerable extensions](collections-samples.md) | `Ploch.Common.Collections` — conditional query composition, joining, dictionaries, arrays and randomisation. |
| [Strings, parsing and text building](strings.md) | `StringExtensions`, `StringParsingExtensions`, `StringBuilderExtensions`, `Strings` and `Chars`. |
| [Conversions and enums](conversions-and-enums.md) | `EnumerationConverter`, `EnumHelper` and the `Ploch.Common.TypeConversion` converter framework. |
| [Expressions and owned properties](expressions-and-properties.md) | `Ploch.Common.Linq` — `ExpressionExtensions` and `IOwnedPropertyInfo`. |
| [Reflection utilities](reflection.md) | `Ploch.Common.Reflection` — type inspection, property access, object-graph traversal and structural equality. |
| [Type loading and assembly scanning](type-loading.md) | `TypeLoader`, `AssemblyTypes`, `ImplementationTypes` and `AssemblyListBuilder`. |
| [Randomizers](randomizers.md) | `Ploch.Common.Randomizers` — an injectable random-value abstraction. |
| [Environment, processes and the host operating system](environment-and-processes.md) | `EnvironmentVariables`, `EnvironmentUtilities`, `OperatingSystemExtensions` and `ProcessExtensions`. |
| [Utility toolbox](utility-toolbox.md) | Paths, streams, hashing, dates, sizes, timing, matching and the other small helpers. |

## Application services

| Article | Covers |
|---|---|
| [Current-user information](user-info-provider.md) | `Ploch.Common.AppServices` — the `IUserInfoProvider` abstraction and its ASP.NET Core implementation. |

## Testing

| Article | Covers |
|---|---|
| [Testing support](testing-support.md) | The `Ploch.TestingSupport` packages — `[AutoMockData]`, file-based test data, platform-aware skipping and the FluentAssertions extensions. |
