# Conversions and Enums

Enums are the type most often marooned at a system boundary. Inside the application they are a closed,
type-safe set; crossing into a database column, a query string, an environment variable or a WMI
property they become strings and integers again. `Ploch.Common` provides three layers for that
crossing:

| Layer | Types | Namespace |
|---|---|---|
| Enum parsing and inspection | `EnumerationConverter`, `EnumHelper` | `Ploch.Common` |
| A pluggable converter abstraction | `ITypeConverter`, `TypeConverter`, `SingleSourceTargetTypeConverter<,>` | `Ploch.Common.TypeConversion` |
| Attribute-driven enum name mapping | `EnumMappingAttribute`, `EnumConversionAttribute`, `EnumConverter`, `EnumName` | `Ploch.Common.TypeConversion` |

```csharp
using Ploch.Common;
using Ploch.Common.TypeConversion;
```

---

## `EnumerationConverter` — parsing to and from enums

```csharp
public static TEnum  ParseToEnum<TEnum>(this string value, bool ignoreCase = false)
public static TEnum  ParseToEnum<TEnum>(this int enumValue, IFormatProvider? formatProvider = null)
public static TEnum? SafeParseToEnum<TEnum>(this string? value, bool ignoreCase = false)
public static TEnum? SafaParseToEnum<TEnum>(this int? enumValue, IFormatProvider? formatProvider = null)
```

All four are constrained `where TEnum : struct, Enum`, and they come in a throwing and a
nullable-returning pair — the same argument-versus-optional-value distinction that runs through
[argument validation](argument-validation.md).

### `ParseToEnum` — the value must be valid

`ParseToEnum` throws `ArgumentOutOfRangeException` when the string does not name a member, and
`ArgumentNullException`/`ArgumentException` when it is null or empty. Use it for values you control —
a discriminator column your own code wrote, a constant in a configuration file that ships with the
application:

```csharp
var status = row.GetString("order_status").ParseToEnum<OrderStatus>(ignoreCase: true);
```

Note that `ignoreCase` defaults to **`false`**, which is the opposite of the default on
`EnvironmentVariables.GetEnumValue`. Pass it explicitly whenever the casing of the incoming value is
not part of the contract.

### `SafeParseToEnum` — absent and invalid are the same thing

`SafeParseToEnum` accepts a `null` receiver and returns `null` for `null`, white space, or an
unrecognised name. That composes directly with `??`, which is what makes it the right tool for query
strings, headers and environment variables:

```csharp
var sortDirection = request.Query["sort"].SafeParseToEnum<SortDirection>(ignoreCase: true)
                    ?? SortDirection.Ascending;
```

[`EnvironmentVariables.GetEnumValue<TEnum>`](environment-and-processes.md#environmentvariables--typed-environment-configuration)
is a thin wrapper over exactly this call.

### Integer overloads — mind the underlying behaviour

Both integer overloads convert the number **to its string form** and then parse that string. Two
consequences follow, and neither is obvious from the signature:

- `1.ParseToEnum<Status>()` succeeds when `Status` declares a member with the value `1`, because
  `Enum.TryParse` accepts the decimal form of a declared value.
- `99.ParseToEnum<Status>()` throws `ArgumentOutOfRangeException` when no member has that value —
  unlike a plain `(Status)99` cast, which silently produces an undefined value.

> **Known issue.** `SafaParseToEnum` (note the spelling — the typo is in the shipped public API) is
> documented as returning `null` for an unmapped integer, but it delegates to `ParseToEnum`, so it
> **throws** rather than returning `null`. Only the `null` *input* case returns `null`. Until
> [issue #313](https://github.com/mrploch/ploch-common/issues/313) is resolved, treat the integer
> overload as throwing, or go through the string form:
> `code?.ToString(CultureInfo.InvariantCulture).SafeParseToEnum<Status>()`.

### Validating rather than parsing

When the value has already been cast to the enum type — a model-bound query parameter, for instance —
parsing is the wrong tool; use
[`NotOutOfRange`](argument-validation.md#notoutofrange--validate-enum-arguments) to reject an
undeclared value:

```csharp
public IActionResult List(OrderStatus status)
{
    status.NotOutOfRange();   // (OrderStatus)999 is rejected here

    // ...
}
```

---

## `EnumHelper` — enumerating members and flags

```csharp
public static IEnumerable<TEnum> GetEnumEntries<TEnum>() where TEnum : Enum
public static IEnumerable<TEnum> GetFlags<TEnum>(this TEnum value) where TEnum : Enum
```

`GetEnumEntries<TEnum>` is `Enum.GetValues` already cast to `TEnum`, which removes the
`.Cast<TEnum>()` that every call site would otherwise repeat. Populating a drop-down or a filter list
becomes a single expression:

```csharp
var statusOptions = EnumHelper.GetEnumEntries<OrderStatus>()
                              .Select(status => new SelectListItem(status.ToString(), ((int)status).ToString()))
                              .ToList();
```

`GetFlags` decomposes a `[Flags]` value into the individual flags it contains, **excluding the zero
value** — so a `None = 0` member never appears in the results, which is almost always what you want
when rendering or persisting a permission set:

```csharp
[Flags]
public enum FilePermissions
{
    None = 0,
    Read = 1,
    Write = 2,
    Execute = 4
}

var granted = FilePermissions.Read | FilePermissions.Write;

var names = granted.GetFlags().Join(", ");   // "Read, Write"
```

A practical use is expanding a packed permissions column into audit rows:

```csharp
foreach (var permission in grant.Permissions.GetFlags())
{
    await _auditLog.RecordAsync(new PermissionGranted(grant.UserId, permission), cancellationToken);
}
```

Two caveats:

- The implementation uses `Enum.HasFlag`, so a **composite** member (for example
  `ReadWrite = Read | Write`) is itself reported as present whenever both of its constituent bits are
  set. Declare composites deliberately, or filter them out.
- The zero-check goes through `Convert.ToInt32`, so an enum with a `long` underlying type whose values
  exceed `int.MaxValue` will overflow. Keep `[Flags]` enums within 32 bits when using `GetFlags`.

---

## The `TypeConversion` abstraction

`Ploch.Common.TypeConversion` is a small framework for conversions that must be **selected at
runtime** — where the source value's type and the target type are both only known when the conversion
happens. It was built for the object-mapping pipeline in `Ploch.Common.ObjectBuilder` (converting WMI
and COM property bags into POCOs), and it is useful anywhere a set of converters has to be tried in
turn.

```csharp
public interface ITypeConverter
{
    int Order { get; }

    bool CanHandle(object? value, Type targetType);
    bool CanHandle(Type sourceType, Type targetType);
    bool CanHandleSourceType(Type sourceType);
    bool CanHandleTargetType(Type targetType);
    object? ConvertValue(object? value, Type targetType);
}
```

The `CanHandle`/`ConvertValue` split is what makes a converter *chain* possible: the pipeline asks each
converter whether it applies before asking it to do the work, and `Order` decides the sequence — a
lower value is tried first, so a narrow converter can pre-empt a general one.

Two marker interfaces narrow the abstraction by direction —
`ISourceTypeConverter<TSourceType>` and `ITargetTypeConverter<TTargetType>` (which adds
`ConvertValueToTargetType(object? value)`) — and `ITypeConverter<TSourceType, TTargetType>` combines
both with strongly typed `ConvertValue` overloads.

`TypeConverter` is the abstract base. Its constructor takes whether a `null` source value is
acceptable, the supported source types, and the supported target types:

```csharp
protected TypeConverter(bool canHandleNullSourceValue,
                        IEnumerable<Type> supportedSourceTypes,
                        IEnumerable<Type> supportedTargetTypes)
```

`SingleSourceTargetTypeConverter<TSourceType, TTargetType>` is the derivation to use for the common
case — one source type, one target type — and adds the strongly typed overloads on top. Deriving from
it means implementing a single `DoConvert` method:

```csharp
public sealed class IsoDateConverter()
    : SingleSourceTargetTypeConverter<string, DateTimeOffset>(canHandleNullSourceValue: false,
                                                              canHandleDerivedSourceTypes: false,
                                                              canHandleDerivedTargetTypes: false)
{
    protected override DateTimeOffset DoConvert(string? value, Type targetType) =>
        DateTimeOffset.ParseExact(value!, "O", CultureInfo.InvariantCulture);
}

var converter = new IsoDateConverter();

var timestamp = converter.ConvertValue("2026-08-28T09:15:00.0000000+00:00");
```

The public `ConvertValue(TSourceType?, Type)` performs the guard work before delegating to
`DoConvert`: it throws `ArgumentNullException` when the value is `null` and the converter was
constructed with `canHandleNullSourceValue: false`, and `ArgumentException` when the runtime type of
the value is not one the converter declared. `DoConvert` therefore only has to implement the
conversion itself. The two `canHandleDerived*Types` flags decide whether a subclass of the declared
source or target type is also accepted; the optional `additionalSourceTypes` and
`additionalTargetTypes` parameters widen the declared sets further.

Conversion failures should be reported as `TypeConversionException`, which carries the offending value
and the target type alongside the message:

```csharp
throw new TypeConversionException($"'{value}' is not an ISO-8601 timestamp.", value, targetType, innerException);
```

> The `TypeConversion` namespace is still evolving — see
> [issue #158](https://github.com/mrploch/ploch-common/issues/158). Treat the enum-mapping pieces
> below as stable and the wider converter framework as an internal building block of
> `Ploch.Common.ObjectBuilder`.

---

## Attribute-driven enum name mapping

External systems rarely spell enum members the way C# does. WMI reports `"Auto Start"`, a legacy API
sends `"AUTOSTART"`, and a newer version of the same API sends `"autoStart"`. `EnumMappingAttribute`
attaches those alternative spellings to the member itself, so the mapping lives next to the value
rather than in a lookup table someone has to remember to update:

```csharp
[EnumConversion(caseSensitive: false)]
public enum ServiceStartMode
{
    [EnumMapping("Boot", "boot start")]
    Boot,

    [EnumMapping("Auto Start", "AUTOSTART", IncludeActualEnumName = true)]
    Automatic,

    [EnumMapping("Manual", "Demand Start")]
    Manual,

    [EnumMapping("Disabled")]
    Disabled
}
```

| Member | Purpose |
|---|---|
| `EnumMappingAttribute.Names` | The alternative string representations for this field. |
| `EnumMappingAttribute.IncludeActualEnumName` | When `true`, the C# member name is accepted as well as the listed names. Defaults to `false` — so listing names **replaces** the member name unless you opt back in. |
| `EnumMappingAttribute.CaseSensitive` | Per-field override, of type `CaseSensitivity` (`Insensitive`, `Sensitive`, `Unspecified`). Defaults to `Unspecified`, deferring to the type-level setting. |
| `EnumConversionAttribute(bool caseSensitive = false)` | The type-level default that `Unspecified` fields fall back to. |

The name/case pairing is modelled by the `EnumName` record, which has an implicit conversion from
`string`, so a plain string can be used wherever an `EnumName` is expected:

```csharp
public record EnumName(string? Name, bool CaseSensitive = false);

EnumName name = "Auto Start";                    // implicit conversion
var explicitName = EnumName.FromString("Boot");
```

`EnumerationMapExtractor.GetEnumFieldValueMap(Type enumType)` builds the
`IDictionary<EnumName, object>` from those attributes, and `EnumerationFieldValueCache` caches the
result per type so the reflection cost is paid once. `EnumConverter` is the `ITypeConverter` that puts
the three together — a `string` → nullable-enum converter that resolves through the mapping and
returns `null` when nothing matches:

```csharp
var converter = new EnumConverter();

var startMode = (ServiceStartMode?)converter.ConvertValue("Auto Start", typeof(ServiceStartMode?));
// ServiceStartMode.Automatic
```

Because `EnumConverter` returns `null` for an unmatched name rather than throwing, it behaves like
`SafeParseToEnum` with the mapping table applied on top.

---

## Choosing the right tool

| Situation | Use |
|---|---|
| A trusted string that must name a member | `ParseToEnum` |
| An optional or untrusted string | `SafeParseToEnum` |
| A value already typed as the enum, arriving from outside | [`NotOutOfRange`](argument-validation.md#notoutofrange--validate-enum-arguments) |
| Every member, for a list or drop-down | `EnumHelper.GetEnumEntries<TEnum>` |
| The individual flags set in a `[Flags]` value | `EnumHelper.GetFlags` |
| An environment variable holding an enum | [`EnvironmentVariables.GetEnumValue`](environment-and-processes.md) |
| External names that differ from the C# member names | `EnumMappingAttribute` + `EnumConverter` |
| A runtime-selected conversion between arbitrary types | `SingleSourceTargetTypeConverter<,>` |

## See also

- [Strings, parsing and text building](strings.md)
- [Argument validation](argument-validation.md)
- [Environment, processes and the host operating system](environment-and-processes.md)
- [`Ploch.Common.TypeConversion` API reference](../api/Ploch.Common.TypeConversion.html)
