# Resolve code analysis warnings

## Summary

Addressed a significant number of static code analysis warnings across the solution, reducing the total warning count from ~2268 to ~1686 (a reduction of ~582 warnings).

## Changes

### Source code fixes

- **SA1028**: Removed trailing whitespace in `EnumerationConverter.cs`, `JetbrainsAnnotations.cs`, `PropertyHelpers.cs`, `StopwatchUtil.cs`, `JsonFileDataAttributeTests.cs`
- **SA1502 / S1109**: Expanded single-line element bodies to multi-line (constructors, interfaces, switch expressions) across Actions.Model, Reflection exceptions, DataAnnotations, Randomizer, and other files
- **CS1711 / RCS1263**: Removed stale `<typeparam>` XML doc tags that referenced non-existent type parameters in `ActionHandlerResult.cs`, `ActionHandlerManagerResult.cs`, `ActionInfo.cs`, `IActionHandler.cs`
- **SA1612 / CS1572 / CS1573 / CC0097**: Fixed mismatched parameter names in XML documentation in `ActionHandlerResult.cs`
- **CS0108**: Added `new` keyword to `IServicesBundle.Configuration` to clarify intentional member hiding
- **CA1816 / S3881**: Fixed `IDisposable` implementation in `ScopedService` to include `GC.SuppressFinalize` and proper dispose pattern
- **CA1812**: Suppressed `NamespaceDoc` never-instantiated warning (documentation class)
- **CC0023 / CA1852**: Sealed classes that have no subtypes: `RequiredNotDefaultDateAttribute`, `AutoMockDataAttribute`, `TextFileLinesDataAttribute`, `TestPriorityAttribute`
- **CA2208**: Fixed `ArgumentException` parameter names in `JsonFileDataAttribute.GetData`
- **S1006**: Added default parameter value to `ActionHandlerManager.ExecuteAsync` override
- **S2376**: Suppressed set-only property warning on `IConfigurationConsumer.Configuration` (intentional design)
- **CS1574**: Fixed XML doc `cref` reference in `DelegatingServicesBundle`
- **CS1591**: Added missing XML documentation for `IScopedService` types

### JetbrainsAnnotations.cs

Suppressed all analyzer warnings in this vendored JetBrains file (~188 warnings) using `#pragma warning disable` since the file is not meant to be modified.

### Test project fixes

- **CA1852**: Sealed numerous private test helper classes across test files (no subtypes, leaf classes only)

## Not addressed (intentionally skipped)

- **CA1062**: Null argument validation - would change public API behaviour
- **CA5394**: Insecure randomness in `Randomizer` classes - intentional (utility, not security)
- **CA1307**: `StringComparison` overloads - not available in netstandard2.0
- **IDE0290**: Primary constructor conversion - significant refactoring
- **S1135**: TODO comments - intentional reminders
- **CA1032**: Standard exception constructors on domain-specific exceptions (e.g. `TypeConversionException`) - already suppressed, non-standard constructor parameters required

Refs: #162
