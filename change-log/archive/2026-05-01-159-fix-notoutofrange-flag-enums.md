## Fix: `Guard.NotOutOfRange` now supports `[Flags]` enums

- **Bug fix:** `Guard.NotOutOfRange<TEnum>` previously used `Enum.IsDefined`, which returns `false` for any combined `[Flags]` value that doesn't exactly match a single declared member (e.g. `MyFlags.A | MyFlags.B`). The method now detects `[Flags]` enums via `FlagsAttribute` and validates that every set bit belongs to a declared member by combining all defined values into a mask.
- **Behaviour clarification:** Non-flags enums continue to require an exactly-defined value. Flags enums accept any value whose bits are a subset of the union of all declared members; values with bits outside that mask (e.g. `(MyFlags)0x10000` when no member uses bit 16, or `(MyFlags)(-1)` when no member equals `~0`) are still rejected with `ArgumentOutOfRangeException`.
- **Note on zero values:** For `[Flags]` enums, a value of `0` (no bits set) is now accepted even when the enum does not declare an explicit `None = 0` member. This matches the C# convention that "zero means no flags chosen". Non-flags enums without a `0` member still reject `0` as before.
- **Internal helpers:** Added private `IsFlagsEnum`, `HasOnlyDefinedFlagValues`, `GetEnumValuesMask`, `GetEnumValues` (a `netstandard2.0` polyfill for the `Enum.GetValues<T>()` generic overload introduced in .NET 5), and `ToUInt64` covering all eight enum underlying types.
- **Test coverage:** Re-enabled the previously skipped `NotOutOfRange_should_work_with_flags_enum_combined_values` test and added `GuardFlagsEnumTests` covering each underlying type (`sbyte`, `byte`, `short`, `ushort`, `int`, `uint`, `long`, `ulong`), undefined-bit rejection, and the negative-cast edge case.

Refs: #159
