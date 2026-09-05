# Ploch.Common.DawnGuard

> [!WARNING]
> **This package is deprecated and will not receive further development.**
>
> Its public API is marked `[Obsolete]`. Argument validation is now provided directly by
> **[`Ploch.Common`](https://www.nuget.org/packages/Ploch.Common/)**, in the
> `Ploch.Common.ArgumentChecking` namespace, with no third-party dependency.

## Why it is deprecated

This package exists only to add type guards to the third-party
[Dawn.Guard](https://www.nuget.org/packages/Dawn.Guard/) library. Argument validation across
the rest of `Ploch.Common` has moved to `Ploch.Common.ArgumentChecking`, which needs no
external dependency, so maintaining a second validation style on top of Dawn.Guard is no
longer worthwhile.

## Migrating

`Ploch.Common` is already a dependency of this package, so the replacements below are
available without adding a package reference.

### Argument validation

`ArgumentChecking` covers the general validation cases, as extension methods on the argument
itself rather than on a `Guard.Argument(...)` wrapper:

| Need | Replacement |
|------|-------------|
| Null argument | `argument.NotNull(nameof(argument))` |
| Null or empty string / collection | `argument.NotNullOrEmpty(nameof(argument))` |
| Required state, not an argument fault | `argument.RequiredNotNull(...)` — throws `InvalidOperationException` |
| Positive number | `argument.Positive(nameof(argument))` |
| Enum range | `argument.NotOutOfRange(nameof(argument))` |
| Path is well-formed | `path.IsValidPath(nameof(path))` — throws `ArgumentException` |
| Path is well-formed, or fail as a required state (not an argument fault) | `path.RequiredIsValidPath()` — throws `InvalidOperationException` (net7.0+; `PathGuard.RequireValidPath` on netstandard2.0) |
| Path is well-formed and the file must already exist | `path.EnsureFileExists()` — throws `ArgumentException` |
| Same, but as a required state | `path.RequiredFileExists()` — throws `InvalidOperationException` (net7.0+ only) |

### `AssignableTo` / `AssignableToOrNull`

`ArgumentChecking` provides both, as extension methods on the `Type` itself. The semantics are
unchanged, so the migration is mechanical:

```csharp
// Before
using Dawn;
using Ploch.Common.DawnGuard;

Guard.Argument(myType, nameof(myType)).AssignableTo(typeof(IMyService));
Guard.Argument(myType, nameof(myType)).AssignableTo<IMyService>();
Guard.Argument(myType, nameof(myType)).AssignableToOrNull<IMyService>();
```

`Ploch.Common` ships two assets, `netstandard2.0` and `net8.0`, and which one you get decides
whether the parameter name has to be passed. **Pick the block that matches the asset your
project resolves to.**

```csharp
// After - projects resolving the net8.0 asset (target net8.0 or later).
// CallerArgumentExpression captures the name, so it can be omitted.
using Ploch.Common.ArgumentChecking;

myType.AssignableTo(typeof(IMyService));
myType.AssignableTo<IMyService>();
myType.AssignableToOrNull<IMyService>();
```

```csharp
// After - projects resolving the netstandard2.0 asset. That includes net7.0 and earlier,
// which have no dedicated asset here, plus netstandard2.0 consumers themselves.
// There is no CallerArgumentExpression, so the name is required.
using Ploch.Common.ArgumentChecking;

myType.AssignableTo(typeof(IMyService), nameof(myType));
myType.AssignableTo<IMyService>(nameof(myType));
myType.AssignableToOrNull<IMyService>(nameof(myType));
```

Passing the name explicitly is valid on both assets, so it is the safe form if you multi-target.
This is the same split every other guard in the namespace uses.

Behaviour, unchanged from this package:

- `AssignableTo` throws `ArgumentNullException` when the argument is null, and
  `ArgumentException` when the type is not assignable.
- `AssignableToOrNull` accepts a null argument and validates only when a value is present.
- Both are **reflexive** — passing the target type itself succeeds, matching
  `Type.IsAssignableFrom`.
- Both return the argument, so they compose.
- The `ArgumentException` message for a non-assignable type is unchanged.

The one difference: a null argument now produces `ArgumentNullException` with the framework's
default message rather than this package's custom `Argument {name} is null.` text. The
**exception type and `ParamName` are unchanged**, so code branching on either is unaffected —
but code that matches on the message text will need updating. The new wording matches every
other guard in `ArgumentChecking`.

> [!NOTE]
> `Ploch.Common.Reflection.TypeExtensions.IsImplementing` is **not** an equivalent, despite the
> similar name: it returns a `bool` rather than throwing, and it returns `false` when the type
> *is* the target type. Use the guards above rather than that predicate when you want an
> argument check.

## Support

The package still builds and ships, so existing consumers are warned by the compiler rather
than broken. Removal is a separate decision and will be announced before it happens.
