# Add AssignableTo guards to ArgumentChecking (#347)

`Ploch.Common.ArgumentChecking.Guard` gains four assignability guards:

```csharp
// The net8.0 asset - parameterName is captured via CallerArgumentExpression
Type  AssignableTo(this Type? argument, Type targetType, string? parameterName = null);
Type  AssignableTo<TTarget>(this Type? argument, string? parameterName = null);
Type? AssignableToOrNull(this Type? argument, Type targetType, string? parameterName = null);
Type? AssignableToOrNull<TTarget>(this Type? argument, string? parameterName = null);

// The netstandard2.0 asset - no CallerArgumentExpression, so the name is required.
// Ploch.Common ships only netstandard2.0 and net8.0, so net7.0 and earlier consumers
// resolve this asset and must pass the name, despite the #if NET7_0_OR_GREATER guard.
Type  AssignableTo(this Type? argument, Type targetType, string parameterName);
Type  AssignableTo<TTarget>(this Type? argument, string parameterName);
Type? AssignableToOrNull(this Type? argument, Type targetType, string parameterName);
Type? AssignableToOrNull<TTarget>(this Type? argument, string parameterName);
```

This matches every other guard in the namespace, which splits the same way.

## Why

Deprecating `Ploch.Common.DawnGuard` (#342) rested on the claim that `ArgumentChecking`
supersedes it. That claim was **false** for the only thing `DawnGuard` actually provided.
`ArgumentChecking` had `NotNull`, `NotNullOrEmpty`, `RequiredNotNull`, `NotNullOrDefault`,
`RequiredTrue`/`RequiredFalse`, `Positive`, `NotOutOfRange` and the `PathGuard` methods — but
no type-assignability guard, which is precisely and only what `TypeGuards.AssignableTo` and
`AssignableToOrNull` did.

Consumers migrating off `DawnGuard` consequently had nothing to migrate *to*, and its README
had to document a hand-written replacement. That README now points at these guards instead.

`Ploch.Common.Reflection.TypeExtensions.IsImplementing` was **not** a usable substitute, and
saying so matters because the name invites the swap:

- it returns a `bool` rather than throwing `ArgumentException` with the parameter name; and
- it returns `false` when the type *is* the target type, whereas assignability is reflexive.

Recommending it would have silently changed behaviour for any caller passing the target type
itself.

## Behaviour

Deliberately identical to the deprecated `TypeGuards`, so the migration is mechanical:

- `AssignableTo` throws `ArgumentNullException` when the argument is null, and
  `ArgumentException` when the type is not assignable.
- `AssignableToOrNull` accepts null and validates only when a value is present.
- Both throw `ArgumentNullException` when `targetType` is null.
- Both are reflexive — passing the target type itself succeeds.
- Both return the argument, so they compose with other guards.
- The `ArgumentException` message keeps the original wording:
  `Instance of type specified in {parameterName} - {argument} cannot be assigned to an instance of {targetType}.`

One message deliberately does **not** carry over. `TypeGuards` threw its null-argument
`ArgumentNullException` with a custom message (`Argument {name} is null.`); these guards
delegate to `ArgumentChecking.NotNull`, which throws with the framework's default message. The
exception type and `ParamName` are identical either way, so code branching on either is
unaffected - but code matching on the message text will need updating. The new wording matches
every other guard in the namespace; preserving the old text would have made these four the odd
ones out.

## Notes

The message is built by a small shared private helper rather than `string.Format`, so both
target-framework partials produce identical text from one place. Every substitution is already
a string, so no format provider is involved, and it keeps CA1863 off an exception path where
caching a composite format would buy nothing.

## Tests

24 tests — 12 against the `netstandard2.0` partial (the `net8.0` leg of `Common.Tests`, which
loads the netstandard binary) and the same 12 against the `net7.0+` partial. Ported from
`Ploch.Common.DawnGuard.Tests` and extended to cover the reflexive case, a null `targetType`,
the returned value for chaining, and `AssignableToOrNull` returning null.
