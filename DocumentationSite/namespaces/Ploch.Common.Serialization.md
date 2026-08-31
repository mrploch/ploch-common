---
uid: Ploch.Common.Serialization
summary: *content
---

### Overview

`Ploch.Common.Serialization` is an abstraction, not an implementation. It exists so that a library or
application can depend on *serialisation* without depending on *a serialiser*: consumers take an `ISerializer`
or `IAsyncSerializer`, and the choice between System.Text.Json, Newtonsoft.Json, or something else entirely
becomes a composition-root decision that can be changed without touching call sites. That matters most for
reusable libraries, which should not force their own JSON stack onto the applications that consume them.

The hierarchy is deliberately two-dimensional. `ISerializer` and `IAsyncSerializer` split synchronous
string-based work from stream-based asynchronous work, while the generic `ISerializer<TSettings>` and
`IAsyncSerializer<TSettings>` variants add per-call configuration for callers that do need to reach the
underlying serialiser's options. `Serializer<TSettings, TDataJsonObject>` and its async counterpart provide the
shared plumbing so a new backing serialiser is a small amount of code. Concrete implementations live in the
child namespaces `Ploch.Common.Serialization.SystemTextJson` and `Ploch.Common.Serialization.NewtonsoftJson`,
each with an `.ExtensionsDependencyInjection` companion package that registers it.

See the [Ploch.Common.Serialization library guide](../../docs/libraries/common-serialization.md) for
installation instructions and worked examples.
