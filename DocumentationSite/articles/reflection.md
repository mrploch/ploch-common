# Reflection Utilities

`Ploch.Common.Reflection` is the largest namespace in the library. It divides into four areas:

| Area | Types | Covered in |
|---|---|---|
| Type inspection | `TypeExtensions`, `TypeHelper` | this article |
| Property and member access | `PropertyHelpers`, `MemberInfoExtensions`, `ObjectReflectionExtensions`, `PropertyAccessValidators`, the `PropertyAccessException` family | this article |
| Object-graph traversal and value comparison | `ObjectGraphHelper`, `ByValueObjectComparator`, `ByValueObjectComparer<T>`, `ObjectHashCodeBuilder` | this article |
| Assembly scanning and type discovery | `TypeLoader`, `TypeLoaderConfigurator`, `AssemblyTypes`, `ImplementationTypes`, `AssemblyListBuilder`, `AssemblyExtensions` | [Type loading and assembly scanning](type-loading.md) |

```csharp
using Ploch.Common.Reflection;
```

---

## Type inspection — `TypeExtensions`

```csharp
public static bool IsImplementing(this Type type, Type baseType, bool concreteOnly = false)
public static bool IsConcreteImplementation(this Type type, Type baseType)
public static bool IsConcreteImplementation<TBaseType>(this Type type)
public static bool IsEnumerable(this Type type)
public static bool IsNullable(this Type type)
public static bool IsSimpleType(this Type type)
public static string GetReadableTypeName(this Type t)
```

### `IsImplementing` and open generic types

`Type.IsAssignableFrom` cannot answer "does this type implement `IRepository<>` for *some* `T`?" —
an open generic type definition is never assignable from a closed one. `IsImplementing` handles both:
it falls back to inspecting each implemented interface and comparing generic type definitions.

```csharp
typeof(CustomerRepository).IsImplementing(typeof(IRepository<>));      // true
typeof(CustomerRepository).IsImplementing(typeof(IRepository<Customer>)); // true
```

Two behaviours differ from `IsAssignableFrom` and matter in practice:

- **A type is never "implementing" itself** — `type == baseType` returns `false`. That makes it
  directly usable as a scanning predicate without having to filter the base type back out of the
  results.
- **`concreteOnly: true`** additionally requires the candidate to be a non-abstract class, so
  interfaces and abstract bases are excluded.

`IsConcreteImplementation` is simply `IsImplementing(baseType, concreteOnly: true)` under a name that
states the intent:

```csharp
var handlerTypes = assembly.GetTypes()
                           .Where(type => type.IsConcreteImplementation<ICommandHandler>())
                           .ToList();

foreach (var handlerType in handlerTypes)
{
    services.AddScoped(typeof(ICommandHandler), handlerType);
}
```

### `IsSimpleType` — the serialisation boundary

```csharp
public static bool IsSimpleType(this Type type)
```

Returns `true` for primitives, **all value types**, enums, `string` and `decimal`, and unwraps
`Nullable<T>` to test the underlying type. It answers the recurring question in any serialiser,
mapper or diff engine: *"can I render this as a single scalar, or must I recurse into it?"*

```csharp
private static object Describe(object? value)
{
    if (value is null)
    {
        return "null";
    }

    var type = value.GetType();

    return type.IsSimpleType()
        ? value.ToString()!
        : value.GetPropertyValues().ToDictionary(p => p.Name, p => Describe(p.Value));
}
```

Note the breadth of the value-type clause: `DateTime`, `Guid`, `TimeSpan` and *any* user-defined
`struct` are all reported as simple, even a struct with several fields. If your definition of "scalar"
is narrower, add your own check on top.

### `IsNullable` and `IsEnumerable`

`IsNullable` tests specifically for `Nullable<T>` — it is `false` for reference types, which are
nullable in the C# sense but not in the `System.Nullable` sense.

`IsEnumerable` tests assignability to the non-generic `IEnumerable`, so it is `true` for `string` as
well as for collections. Exclude `string` explicitly when "is this a collection?" is what you actually
mean:

```csharp
var isCollection = type.IsEnumerable() && type != typeof(string);
```

### `GetReadableTypeName` — names fit for humans

`Type.Name` renders a generic type as `Dictionary\`2` and an array as `Int32[]`. `GetReadableTypeName`
produces the C#-shaped form instead, recursing through generic arguments and array element types:

```csharp
typeof(List<int>).GetReadableTypeName();                        // "List<Int32>"
typeof(Dictionary<string, List<int>>).GetReadableTypeName();    // "Dictionary<String, List<Int32>>"
typeof(int[]).GetReadableTypeName();                            // "Int32[]"
```

This is what makes a diagnostics page or a start-up log readable:

```csharp
foreach (var registration in services)
{
    _logger.LogDebug("{Service} -> {Implementation}",
                     registration.ServiceType.GetReadableTypeName(),
                     registration.ImplementationType?.GetReadableTypeName() ?? "(factory)");
}
```

Names are the CLR type names (`Int32`, `String`), not the C# keywords.

---

## Property access — `PropertyHelpers`

`PropertyHelpers` is the workhorse for reading and writing properties whose names are only known at
runtime.

```csharp
public const string IndexerPropertyName = "Item";

public static PropertyInfo? GetPropertyInfo(this Type type, string propertyName, bool throwIfNotFound)
public static bool HasProperty(this object obj, string propertyName)
public static IEnumerable<PropertyInfo> GetProperties<TPropertyType>(this object obj, bool includeAssignableOrInheritedTypes = true)
public static IEnumerable<PropertyInfo> GetProperties(this object obj, bool includeAssignableOrInheritedTypes, params Type[] propertyTypes)
public static IEnumerable<(string Name, object? Value)> GetPropertyValues(this object obj)

public static object? GetPropertyValue<T>(this T obj, string propertyName, object?[]? index = null)
public static TValue? GetPropertyValue<T, TValue>(this T obj, string propertyName, object?[]? index = null)
public static object? GetPropertyValue<T>(this T? obj, PropertyInfo propertyInfo, object?[]? index = null)
public static TProperty? GetPropertyValue<TProperty, T>(this T obj, Expression<Func<T, TProperty>> propertySelector)
public static void SetPropertyValue<T>(this T obj, string propertyName, object? value)

public static object? GetStaticPropertyValue(this Type type, string propertyName)
public static TValue? GetStaticPropertyValue<TValue>(this Type type, string propertyName)
public static bool TryGetStaticPropertyValue(this Type type, string propertyName, out object? value)
public static bool IsStatic(this PropertyInfo propertyInfo)
```

### Precise failures instead of `null`

Raw reflection returns `null` from `Type.GetProperty` for a name that does not exist, and the
`NullReferenceException` surfaces somewhere unhelpful. `PropertyHelpers` validates first and throws a
specific exception from the `PropertyAccessException` family:

| Exception | Raised when |
|---|---|
| `PropertyNotFoundException` | No property with that name exists on the type. |
| `PropertyReadOnlyException` | A write was attempted on a property with no setter. |
| `PropertyWriteOnlyException` | A read was attempted on a property with no getter. |
| `PropertyIndexerMismatchException` | Index arguments do not match the property's index parameters. |

All four derive from `PropertyAccessException`, so one `catch` covers the family:

```csharp
public bool TryApplyPatch(object entity, string propertyName, object? value, out string? error)
{
    try
    {
        entity.SetPropertyValue(propertyName, value);
        error = null;

        return true;
    }
    catch (PropertyAccessException exception)
    {
        error = exception.Message;

        return false;
    }
}
```

`GetPropertyInfo(type, name, throwIfNotFound)` gives you the choice explicitly, and `HasProperty` is
the non-throwing probe:

```csharp
if (entity.HasProperty("LastModifiedUtc"))
{
    entity.SetPropertyValue("LastModifiedUtc", DateTime.UtcNow);
}
```

### Filtering properties by type

```csharp
public static IEnumerable<PropertyInfo> GetProperties<TPropertyType>(this object obj, bool includeAssignableOrInheritedTypes = true)
```

Returns the public properties whose type is `TPropertyType` — and, by default, the properties whose
type is assignable to it. Passing `false` restricts to exact type matches. The `params Type[]`
overload accepts several types at once.

A typical use is scrubbing an object before it reaches a log sink:

```csharp
public static void RedactSecrets(object target)
{
    foreach (var property in target.GetProperties<string>().Where(p => p.CanWrite))
    {
        if (property.Name.ContainsAny(StringComparison.OrdinalIgnoreCase, "Password", "Secret", "Token", "ApiKey"))
        {
            property.SetValue(target, "***");
        }
    }
}
```

### Reading every property value

```csharp
public static IEnumerable<(string Name, object? Value)> GetPropertyValues(this object obj)
```

Returns name/value tuples for every public property, **skipping indexers** — which is what makes it
safe to call on an arbitrary object without knowing whether it has an `this[...]`:

```csharp
var snapshot = order.GetPropertyValues().ToDictionary(p => p.Name, p => p.Value);
_diagnostics.Record("order-snapshot", snapshot);
```

### Static properties

Static properties are a persistent source of `TargetException` when the wrong overload is used.
`PropertyHelpers` exposes them explicitly:

```csharp
var maximum = typeof(int).GetStaticPropertyValue<int>(nameof(int.MaxValue));

if (typeof(SomeSdkClass).TryGetStaticPropertyValue("Version", out var version))
{
    _logger.LogInformation("SDK version {Version}", version);
}
```

`TryGetStaticPropertyValue` returns `false` rather than throwing when the property is absent or is not
static, so it is the right choice when probing an optional dependency loaded by reflection.

### Indexers

`PropertyHelpers.IndexerPropertyName` is the `"Item"` name the C# compiler gives an indexer, and the
`object?[]? index` parameter carries the index arguments:

```csharp
var value = configuration.GetPropertyValue(PropertyHelpers.IndexerPropertyName, [ "Logging:LogLevel:Default" ]);
```

Supplying index arguments that do not match the property's index parameters raises
`PropertyIndexerMismatchException`.

---

## `MemberInfoExtensions`

```csharp
public static object? GetValue(this MemberInfo memberInfo, object? obj, params object?[]? index)
public static bool IsIndexer(this MemberInfo memberInfo)
public static bool IsNonIndexerReadProperty(this MemberInfo memberInfo)
public static bool IsStatic(this MemberInfo memberInfo)
```

`MemberInfo` has no `GetValue` of its own — you must first test whether it is a `PropertyInfo` or a
`FieldInfo` and downcast. `MemberInfoExtensions.GetValue` collapses that, letting fields and
properties be treated uniformly:

```csharp
var members = typeof(Configuration)
              .GetMembers(BindingFlags.Public | BindingFlags.Instance)
              .Where(member => member.IsNonIndexerReadProperty() || member is FieldInfo);

foreach (var member in members)
{
    Console.WriteLine($"{member.Name} = {member.GetValue(configuration)}");
}
```

`IsNonIndexerReadProperty` is the predicate to reach for whenever you enumerate members for
*reading*: it excludes write-only properties and indexers in one test, both of which would otherwise
throw when `GetValue` is called with no index arguments.

`IsStatic` works uniformly across fields, properties, methods and events — `MemberInfo` itself offers
no such property.

---

## `ObjectReflectionExtensions` — fields and mixed member sets

```csharp
public static object? GetFieldValue(this object obj, string fieldName)
public static TValue? GetFieldValue<TValue>(this object obj, string fieldName)
public static IDictionary<string, object?> GetFieldValues<TType>(this TType? obj, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public)
public static IDictionary<string, object?> GetMemberValues<TType>(this TType? obj, BindingFlags bindingFlags = ..., MemberTypes memberTypes = MemberTypes.Field | MemberTypes.Property, Type? reflectedType = null)
public static IDictionary<string, object?> GetMemberValues<TType>(BindingFlags bindingFlags = ..., MemberTypes memberTypes = ...)
```

`GetMemberValues` returns fields **and** properties in one dictionary. Two conveniences make it
behave sensibly at the edges:

- Passing a `null` instance strips `BindingFlags.Instance`, so the call degrades cleanly to a static
  member read. The static-only overload (no `obj` parameter) does the same thing explicitly.
- Passing an instance adds `BindingFlags.DeclaredOnly`, so inherited members are not duplicated.

```csharp
// Every instance field and property, public and private.
var state = connection.GetMemberValues(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

// Static constants declared on a type.
var constants = ObjectReflectionExtensions.GetMemberValues<ApplicationConstants>(
    BindingFlags.Public,
    MemberTypes.Field);
```

`TypeHelper.GetStaticFieldValues<TType>(bindingFlags)` is a thin, intention-revealing wrapper over the
static-field case.

---

## Object-graph traversal — `ObjectGraphHelper`

```csharp
public static void ExecuteOnProperties(this object? root, Action<object> action)
public static void ExecuteOnProperties<TPropertyType>(this object? root, Action<TPropertyType> action)
```

Walks an object graph recursively, invoking the action for each property value it encounters. A
`HashSet<object>` of visited instances guards against infinite recursion on circular references, and a
`null` root is a no-op rather than an exception.

The generic overload filters by type, which turns "find every X anywhere in this graph" into a single
call:

```csharp
// Attach a correlation id to every nested entity in a deeply structured request.
request.ExecuteOnProperties<IAuditable>(auditable =>
{
    auditable.CorrelationId = correlationId;
    auditable.ModifiedOn = DateTimeOffset.UtcNow;
});

// Collect every validation message hidden anywhere in a response payload.
var messages = new List<ValidationMessage>();
response.ExecuteOnProperties<ValidationMessage>(messages.Add);
```

Because the traversal is reflection-driven and unbounded in depth, keep it out of per-request hot
paths on large graphs.

---

## Structural equality — `ByValueObjectComparator` and friends

```csharp
public static bool AreEqual(object? x, object? y, Type? type = null)                 // ByValueObjectComparator
public class ByValueObjectComparer<TObject> : IEqualityComparer<TObject>
public static int GetHashCode(object? obj)                                            // ObjectHashCodeBuilder
```

These compare two objects by their member values rather than by reference, without requiring the types
to implement `Equals`/`GetHashCode`. `ByValueObjectComparer<TObject>` packages that as an
`IEqualityComparer<TObject>`, so it drops straight into LINQ and the framework collections:

```csharp
var comparer = new ByValueObjectComparer<AddressDto>();

var newAddresses = incoming.Except(existing, comparer).ToList();
var deduplicated = new HashSet<AddressDto>(incoming, comparer);
```

The natural production use is detecting whether an inbound payload actually differs from what is
already stored, for types (DTOs, integration contracts) that have no value semantics of their own:

```csharp
if (!ByValueObjectComparator.AreEqual(storedProfile, incomingProfile))
{
    await _repository.UpdateAsync(incomingProfile, cancellationToken);
}
```

For types you own and control, C# `record` types give the same semantics with none of the reflection
cost — prefer those. Reach for these helpers when the type is generated, third-party, or otherwise not
yours to change.

---

## Choosing the right tool

| Goal | Use |
|---|---|
| Does this type implement a (possibly open generic) interface? | `TypeExtensions.IsImplementing` |
| Should I recurse into this value or render it as a scalar? | `TypeExtensions.IsSimpleType` |
| A type name for a log or a diagnostics page | `TypeExtensions.GetReadableTypeName` |
| Read or write a property named at runtime | `PropertyHelpers.GetPropertyValue` / `SetPropertyValue` |
| A property selected by a lambda, bound to its instance | [`ExpressionExtensions.GetProperty`](expressions-and-properties.md) |
| Treat fields and properties uniformly | `MemberInfoExtensions.GetValue`, `ObjectReflectionExtensions.GetMemberValues` |
| Act on every matching value in a nested graph | `ObjectGraphHelper.ExecuteOnProperties<T>` |
| Compare two instances by value | `ByValueObjectComparer<T>` |
| Find implementations across assemblies | [`TypeLoader` and `AssemblyTypes`](type-loading.md) |

## See also

- [Type loading and assembly scanning](type-loading.md)
- [Expressions and owned properties](expressions-and-properties.md)
- [`Ploch.Common.Reflection` API reference](../api/Ploch.Common.Reflection.html)
