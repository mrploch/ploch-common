---
uid: Ploch.Common.Apps.Model
summary: *content
---

### Overview

`Ploch.Common.Apps.Model` is the action-handler framework: a way to express "this operation may be satisfied by
one of several alternative strategies, tried in priority order, until one succeeds". Launching an application,
for instance, might be attempted first through a system API and then through a shell command; the caller should
not have to know which strategies exist, nor encode the fallback order at the call site. `ActionHandlerManager`
owns that orchestration, resolving the registered `IActionHandler` implementations, ordering them by
`Priority`, and returning the first successful result.

The framework is fully generic and imposes no base class on the domain. The thing being acted upon is described
by `IActionTargetDescriptor`, the action itself by `IActionInfo<TDescriptor>`, and outcomes flow back through
`ActionHandlerResult` and `ActionHandlerManagerResult` rather than through exceptions, so "no handler could do
this" is an ordinary result rather than a failure to catch. `ActionExecutionException` is reserved for a
handler that genuinely faulted.

> The namespace is `Ploch.Common.Apps.Model`, while the NuGet package is `Ploch.Common.Apps.Actions.Model`.

See the [Ploch.Common.Apps library guide](../../docs/libraries/common-apps.md) for installation instructions
and worked examples.
