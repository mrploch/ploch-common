---
uid: Ploch.Common.DependencyInjection
summary: *content
---

### Overview

`Ploch.Common.DependencyInjection` addresses a gap in `Microsoft.Extensions.DependencyInjection`: it has no
first-class notion of a *module*. Registration code therefore tends to accumulate as a long list of extension
method calls in `Program.cs`, with the ordering constraints between them recorded nowhere. The `ServicesBundle`
pattern here — inspired by Autofac modules — lets a component own its own registrations, declare the bundles it
depends on, and have those dependencies configured first, exactly once, no matter how many bundles ask for
them.

`ServicesBundle` is the base for registrations that need nothing external; `ConfigurableServicesBundle` is for
those that require an `IConfiguration`; and `DelegatingServicesBundle` composes several bundles into one
without a new class. `IScopedService<T>` is a separate convenience for resolving a scoped dependency from a
singleton without capturing it, which is the usual cause of accidental lifetime bugs. Hosting integration lives
in the child namespace `Ploch.Common.DependencyInjection.Hosting`.

See the [Ploch.Common.DependencyInjection library guide](../../docs/libraries/common-dependency-injection.md)
for installation instructions and worked examples.
