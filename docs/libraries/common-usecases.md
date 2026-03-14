# Ploch.Common.UseCases

> Placeholder package reserved for use-case abstractions in the Ploch.Common library suite.

## Overview

`Ploch.Common.UseCases` is a reserved package directory in the repository that is expected to provide common abstractions for the use-case (application service) layer of applications built on Ploch.Common. At the time of writing the package directory exists but does not yet contain any source files.

When the package is populated, it will likely define interfaces such as `IUseCase<TRequest, TResponse>` and base classes or utilities that standardise how use-case classes are structured, injected, and composed.

## Related Libraries

- [Ploch.Common.Apps](common-apps.md) — Action handler framework, which models a related but lower-level command-execution pattern
- [Ploch.Common.AppServices](common-appservices.md) — Cross-cutting application service abstractions (user identity, etc.)
