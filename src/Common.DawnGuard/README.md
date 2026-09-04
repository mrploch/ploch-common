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
| Path validation | `Ploch.Common.ArgumentChecking.PathGuard` |

### `AssignableTo` / `AssignableToOrNull` — no direct replacement yet

**These two methods have no equivalent guard in `ArgumentChecking`.** Until one is added
(tracked in the repository's issue list), replace them by hand:

```csharp
// Before
using Dawn;
using Ploch.Common.DawnGuard;

Guard.Argument(myType, nameof(myType)).AssignableTo(typeof(IMyService));

// After
using Ploch.Common.ArgumentChecking;

myType.NotNull(nameof(myType));
if (!typeof(IMyService).IsAssignableFrom(myType))
{
    throw new ArgumentException(
        $"Instance of type specified in {nameof(myType)} - {myType.FullName} cannot be assigned to an instance of {typeof(IMyService).FullName}.",
        nameof(myType));
}
```

`AssignableToOrNull` is the same check without the null guard — it accepts a null argument
and only validates when a value is present.

> [!NOTE]
> `Ploch.Common.Reflection.TypeExtensions.IsImplementing` looks like a replacement but is
> **not** a drop-in: it returns a `bool` rather than throwing, and it returns `false` when the
> type *is* the target type, whereas `AssignableTo` succeeds in that case because
> `Type.IsAssignableFrom` is reflexive.

## Support

The package still builds and ships, so existing consumers are warned by the compiler rather
than broken. Removal is a separate decision and will be announced before it happens.
