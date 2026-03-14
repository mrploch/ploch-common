# Ploch.Common.Apps

> A priority-based, composable action handler framework for orchestrating multiple processing strategies against a common target descriptor.

## Overview

`Ploch.Common.Apps` provides two related packages for building extensible, priority-ordered processing pipelines:

- **`Ploch.Common.Apps.Actions.Model`** — The core action handler framework: interfaces, base classes, result types, and the `ActionHandlerManager` orchestrator.
- **`Ploch.Common.Apps.Shared`** — A lightweight `AppInfo` class for carrying application metadata (name, description, version, args).

The action handler framework is useful in scenarios where a single logical operation (an "action") may be handled by one of several alternative strategies, tried in priority order. A typical use case is dispatching a command (e.g. "launch this application") to a set of registered handlers (e.g. one that uses a system API, another that uses a shell command) and stopping as soon as one succeeds.

The framework is fully generic. The entity receiving the action is modelled by `IActionTargetDescriptor`; the information about the action itself is modelled by `IActionInfo<TDescriptor>`. This lets you define strongly typed domains without any dependency on the framework beyond the interfaces.

## Installation

```shell
# Action handler framework
dotnet add package Ploch.Common.Apps.Actions.Model

# AppInfo helper
dotnet add package Ploch.Common.Apps.Shared
```

## Key Types

### `Ploch.Common.Apps.Actions.Model` (namespace: `Ploch.Common.Apps.Model`)

| Type | Kind | Description |
|---|---|---|
| `IActionTargetDescriptor` | Interface | Describes the target of an action. Requires a `Name` property. |
| `IActionInfo` | Interface | Base action info — carries the action `Name`. |
| `IActionInfo<TDescriptor>` | Interface | Extends `IActionInfo` with a `Descriptor` property of type `TDescriptor`. |
| `ActionInfo<TDescriptor>` | Class | Default implementation of `IActionInfo<TDescriptor>`. |
| `IActionHandler<TTarget, TActionInfo, TResult>` | Interface | Core handler contract. `ExecuteAsync(actionInfo, ct)` returns `Task<TResult>`. Also exposes `ActionInfoType`. |
| `IActionHandler<TTarget, TActionInfo>` | Interface | Convenience alias that fixes `TResult` to `ActionHandlerResult<TTarget>`. Adds a `Priority` property. |
| `ActionHandler<TTarget, TActionInfo>` | Abstract class | Base class for concrete handlers. Implements `Priority` as abstract; provides `GetExecutionId`. |
| `ActionHandler<TTarget, TActionInfo, TResult>` | Abstract class | Base class for handlers with a custom result type. |
| `IActionHandlerManager<TTarget, TActionInfo>` | Interface | Manager contract — extends the handler hierarchy with `ActionHandlerManagerResult<TTarget>` as the result type. |
| `ActionHandlerManager<TTarget, TActionInfo, THandler>` | Class | Concrete manager. Runs handlers in ascending `Priority` order and returns on first success. |
| `ActionHandlerResult<TTarget>` | Class | Result of a single handler execution: `IsSuccess`, `ExecutionId`, and optional `Errors`. Factory methods: `ActionHandlerResult.Success(...)` and `ActionHandlerResult.Failure(...)`. |
| `ActionHandlerManagerResult<TTarget>` | Class | Extends `ActionHandlerResult<TTarget>` with `HandlerResults` — the outcome of every individual handler. Factory methods: `ActionHandlerManagerResult.Success(...)` and `ActionHandlerManagerResult.Failure(...)`. |
| `ActionExecutionId<TTarget>` | Class | Uniquely identifies an execution: captures the `IActionInfo` and the handler `Type`. |
| `ActionExecutionException` | Class | Exception thrown when an unrecoverable error occurs inside a handler. |

### `Ploch.Common.Apps.Shared`

| Type | Kind | Description |
|---|---|---|
| `AppInfo` | Class | Carries `Name`, `Description`, `Version`, and `Args` for a console or hosted application. **Note:** despite being in the `Ploch.Common.Apps.Shared` package, the class is in the `Ploch.CommandLine.Spectre` namespace. |

## Usage Examples

### Defining a target descriptor and action info

```csharp
public class SystemApplication : IActionTargetDescriptor
{
    public string Name { get; init; } = string.Empty;
    public string ExecutablePath { get; init; } = string.Empty;
}

public class LaunchActionInfo(SystemApplication app, string name)
    : ActionInfo<SystemApplication>(app, name)
{
    public string[] Arguments { get; init; } = [];
}
```

### Implementing handlers with priority

```csharp
public class ShellLaunchHandler : ActionHandler<SystemApplication, LaunchActionInfo>
{
    public override int Priority => 10; // tried second

    public override Task<ActionHandlerResult<SystemApplication>> ExecuteAsync(
        LaunchActionInfo actionInfo, CancellationToken ct)
    {
        var id = GetExecutionId(actionInfo);
        try
        {
            Process.Start(actionInfo.Descriptor.ExecutablePath, actionInfo.Arguments);
            return Task.FromResult(ActionHandlerResult.Success(id));
        }
        catch (Exception ex)
        {
            return Task.FromResult(ActionHandlerResult.Failure(id, ex));
        }
    }
}

public class ApiLaunchHandler : ActionHandler<SystemApplication, LaunchActionInfo>
{
    public override int Priority => 0; // tried first (lower value = higher priority)

    public override Task<ActionHandlerResult<SystemApplication>> ExecuteAsync(
        LaunchActionInfo actionInfo, CancellationToken ct)
    {
        var id = GetExecutionId(actionInfo);
        // ... call a system API ...
        return Task.FromResult(ActionHandlerResult.Success(id));
    }
}
```

### Registering and using the manager

```csharp
// DI registration
services.AddScoped<IActionHandler<SystemApplication, LaunchActionInfo>, ApiLaunchHandler>();
services.AddScoped<IActionHandler<SystemApplication, LaunchActionInfo>, ShellLaunchHandler>();
services.AddScoped<
    IActionHandlerManager<SystemApplication, LaunchActionInfo>,
    ActionHandlerManager<SystemApplication, LaunchActionInfo,
        IActionHandler<SystemApplication, LaunchActionInfo>>>();

// Usage
public class AppLauncher(IActionHandlerManager<SystemApplication, LaunchActionInfo> manager)
{
    public async Task LaunchAsync(SystemApplication app, CancellationToken ct)
    {
        var actionInfo = new LaunchActionInfo(app, "Launch");
        var result = await manager.ExecuteAsync(actionInfo, ct);

        if (!result.IsSuccess)
        {
            foreach (var error in result.Errors ?? [])
                Console.Error.WriteLine(error.Message);
        }
    }
}
```

The manager runs `ApiLaunchHandler` first (priority 0). If it succeeds, `ShellLaunchHandler` is never called. If it fails, the manager continues to `ShellLaunchHandler`. If all handlers fail, `ActionHandlerManagerResult.IsSuccess` is `false` and `HandlerResults` contains the outcome of every attempted handler.

### Using AppInfo

```csharp
var info = new AppInfo(args)
{
    Name = "My Tool",
    Description = "Processes files.",
    Version = new Version(1, 0, 0)
};
```

## Related Libraries

- [Ploch.Common.UseCases](common-usecases.md) — Use-case abstractions that complement the action handler pattern
