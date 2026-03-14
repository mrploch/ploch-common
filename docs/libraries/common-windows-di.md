# Ploch.Common.Windows.DependencyInjection

> Placeholder package reserved for DI registration of Windows-specific services from `Ploch.Common.Windows`.

## Overview

`Ploch.Common.Windows.DependencyInjection` is a reserved package directory in the repository, following the established Ploch.Common convention of separating a library's core implementation from its dependency injection wiring. At the time of writing the package directory exists but does not yet contain any source files.

The split mirrors the pattern used elsewhere in the suite (for example, `Ploch.Common.Serialization.SystemTextJson` paired with `Ploch.Common.Serialization.SystemTextJson.ExtensionsDependencyInjection`). When `Ploch.Common.Windows` is populated, this companion package will provide the corresponding `IServiceCollection` extension methods.

## Related Libraries

- [Ploch.Common.Windows](common-windows.md) — Windows-specific utilities (planned)
- [Ploch.Common.DependencyInjection](common-dependency-injection.md) — ServicesBundle pattern used across the suite
