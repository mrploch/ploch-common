# Reflection

Almost every non-trivial application ends up writing the same four pieces of reflection code:
*find every implementation of this interface so I can register it*, *read this property by name
because the name came from a query string*, *tell me whether this type is simple enough to log
inline*, and *compare these two objects by their contents rather than by reference*.

`Ploch.Common.Reflection` is that code, written once. It is not an abstraction over reflection — it
is a set of small helpers that each collapse five to fifteen lines of `BindingFlags`, LINQ over
`GetInterfaces()` and defensive null handling into a single expression, and that name their failure
modes properly instead of throwing `NullReferenceException` three frames later.

The namespace covers four distinct jobs, and it is worth knowing which one you are in:

| Job | Types |
|-----|-------|
| Ask a question about a `Type` | <xref:Ploch.Common.Reflection.TypeExtensions> |
| Find types across one or many assemblies | <xref:Ploch.Common.Reflection.ImplementationTypes>, <xref:Ploch.Common.Reflection.AssemblyTypes>, <xref:Ploch.Common.Reflection.TypeLoader>, <xref:Ploch.Common.Reflection.AssemblyListBuilder>, <xref:Ploch.Common.Reflection.AssemblyExtensions> |
| Read or write a member by name at runtime | <xref:Ploch.Common.Reflection.PropertyHelpers>, <xref:Ploch.Common.Reflection.ObjectReflectionExtensions>, <xref:Ploch.Common.Reflection.MemberInfoExtensions>, <xref:Ploch.Common.Reflection.TypeHelper>, and the <xref:Ploch.Common.Reflection.PropertyAccessException> family |
| Walk or compare a whole object graph | <xref:Ploch.Common.Reflection.ObjectGraphHelper>, <xref:Ploch.Common.Reflection.ByValueObjectComparator>, <xref:Ploch.Common.Reflection.ByValueObjectComparer`1>, <xref:Ploch.Common.Reflection.ObjectHashCodeBuilder> |

The first three jobs are the ones this namespace does well. The fourth carries real caveats, and
[Walking and comparing object graphs](#walking-and-comparing-object-graphs) sets them out rather
than burying them.

For a lambda-based alternative to string-keyed property access — `customer => customer.Name` instead
of `"Name"` — see [Expressions and owned property info](./expressions-and-owned-properties.md). Where
the member is known at the call site, that is almost always the better tool.

## Interrogating a type

### `IsImplementing` and `IsConcreteImplementation`

`Type.IsAssignableFrom` is the built-in answer, and it is awkward in two specific ways: the argument
order reads backwards, and it does not understand open generic interfaces.
<xref:Ploch.Common.Reflection.TypeExtensions.IsImplementing(System.Type,System.Type,System.Boolean)>
fixes both.

```csharp
typeof(CsvExporter).IsImplementing(typeof(IExporter));      // True
typeof(IntHandler).IsImplementing(typeof(IHandler<int>));   // True
typeof(IntHandler).IsImplementing(typeof(IHandler<>));      // True  <- the useful one
typeof(IntHandler).IsImplementing(typeof(IHandler<string>)); // False
```

The open-generic form is what makes handler registration possible: given `IHandler<>` you can find
`IntHandler`, `GenericHandler<T>` and anything deriving from them, without knowing the closed types
in advance.

Two behaviours are not obvious from the name and will bite if you assume `IsAssignableFrom`
semantics:

**A type is never reported as implementing itself.**

```csharp
typeof(IExporter).IsImplementing(typeof(IExporter));   // False
```

`IsAssignableFrom` would say `true` here. `IsImplementing` deliberately answers "is this a *different*
type that implements the given one", which is what you want when scanning an assembly — the marker
interface itself should not appear in the result set. It also means the helper cannot be used as a
general assignability check.

**`concreteOnly: true` excludes structs and static classes, not just abstract classes.** The third
parameter is implemented as `type.IsAbstract || !type.IsClass`, so anything that is not a
non-abstract class is filtered out:

```csharp
typeof(ExporterBase).IsImplementing(typeof(IExporter), concreteOnly: true);   // False — abstract
typeof(StructExporter).IsImplementing(typeof(IExporter), concreteOnly: true); // False — a struct
typeof(StructExporter).IsImplementing(typeof(IExporter), concreteOnly: false); // True
```

That is the right default for DI registration — you cannot register an abstract class — but if your
plugin contract is implemented by a `readonly struct`, `concreteOnly` will silently drop it. This
matters more than it looks, because `concreteOnly: true` is the *default* everywhere else in the
namespace: `IsConcreteImplementation`, `GetTypesImplementing`, `AssemblyTypes.GetImplementations`
and `TypeLoader` all default to concrete-only.

<xref:Ploch.Common.Reflection.TypeExtensions.IsConcreteImplementation``1(System.Type)> is exactly
`IsImplementing(baseType, concreteOnly: true)` with a generic argument, and reads better at a call
site that is already filtering:

```csharp
var exporters = assemblyTypes.Where(t => t.IsConcreteImplementation<IExporter>());
```

### `IsSimpleType` — wider than "primitive"

<xref:Ploch.Common.Reflection.TypeExtensions.IsSimpleType(System.Type)> answers "can I render this
as a single value, or do I have to recurse into it?" — the question that logging, diffing and
serialisation code asks constantly.

Its definition is **primitive, or any value type, or an enum, or `string`, or `decimal`, or a
nullable wrapper around any of those**. The "any value type" clause is the important one, and it is
much broader than the word *simple* suggests:

| Type | `IsSimpleType()` |
|------|------------------|
| `int`, `string`, `decimal`, `char`, `bool` | `true` |
| `DateTime`, `DateTimeOffset`, `TimeSpan`, `Guid` | `true` |
| any `enum` | `true` |
| `int?`, `Guid?` | `true` |
| **any user-defined `struct`**, however many fields it has | `true` |
| **any `ValueTuple`**, e.g. `(string, int)` | `true` |
| any class or interface, including `List<int>` and `int[]` | `false` |

So a `readonly struct Money(decimal Amount, string Code)` is "simple", and will be treated as an
atomic value by the members that consult `IsSimpleType` — including
<xref:Ploch.Common.Reflection.ByValueObjectComparator>, which will compare two `Money` values with
`Equals` rather than recursing into `Amount` and `Code`. Not everything in this namespace consults
it: <xref:Ploch.Common.Reflection.ObjectGraphHelper> never checks `IsSimpleType` and recurses into
any non-enumerable value, so `ExecuteOnProperties` walks a `Money` property into `Amount` and `Code`
rather than treating it atomically. For a struct that implements value equality that is correct and
fast. For a struct that does *not* override `Equals`, the runtime's default structural comparison
applies, which is usually still what you want — but it is worth knowing that the decision was made
for you.

`(string, int)` being simple is a genuine trap for code that walks tuples expecting to enumerate the
items.

### `IsNullable`, `IsEnumerable`

Two one-liners that say what they mean:

```csharp
typeof(int?).IsNullable();      // True
typeof(string).IsNullable();    // False — reference-type nullability is not a runtime concept

typeof(int[]).IsEnumerable();   // True
typeof(string).IsEnumerable();  // True  <- string is IEnumerable<char>
```

`IsNullable` tests for `Nullable<T>` specifically. It has nothing to say about nullable reference
types, which are erased at runtime.

`IsEnumerable` returning `true` for `string` is correct — `string` really does implement
`IEnumerable` — but it is the wrong predicate for "should I iterate this?". Guard for `string`
separately, exactly as <xref:Ploch.Common.Reflection.ObjectHashCodeBuilder> does internally.

### `GetReadableTypeName`

`Type.Name` returns the metadata name, and for a generic type that means an arity suffix:
`` List`1 ``. That is unusable in a log line, an error message or a diagnostics page.
<xref:Ploch.Common.Reflection.TypeExtensions.GetReadableTypeName(System.Type)> renders the name the
way it is written in source:

| Type | `Type.Name` | `GetReadableTypeName()` |
|------|-------------|-------------------------|
| `List<int>` | `` List`1 `` | `List<Int32>` |
| `Dictionary<string, List<int>>` | `` Dictionary`2 `` | `Dictionary<String, List<Int32>>` |
| `int?` | `` Nullable`1 `` | `Nullable<Int32>` |
| `List<>` (open) | `` List`1 `` | `List<T>` |
| `int[]` | `Int32[]` | `Int32[]` |
| `int[][]` | `Int32[][]` | `Int32[][]` |
| `Outer.Inner` | `Inner` | `Inner` |

Three things to expect:

- **CLR type names, not C# keywords.** You get `Int32`, not `int`; `String`, not `string`. The
  method renders *shape*, not C# syntax.
- **No namespace, and no declaring type for a nested type.** `Outer.Inner` renders as `Inner`. If
  two nested types share a short name your message will be ambiguous — use `Type.FullName` when the
  reader needs to locate the type rather than recognise it.
- **Multi-dimensional arrays lose their rank.** `typeof(int[,]).GetReadableTypeName()` returns
  `Int32[]`, not `Int32[,]` — the implementation appends a fixed `"[]"` per element-type hop and
  never inspects `Type.GetArrayRank()`. Jagged arrays (`int[][]`) are fine because each hop really
  is one dimension. Multi-dimensional arrays are rare in the domain models this helper is aimed at,
  but do not use it to render a signature that has to round-trip.

This is the same helper the
[expressions article](./expressions-and-owned-properties.md) uses to build validation messages, and
for the same reason: a guard message reading `` Wrapper`1.Label must not be blank `` is a bug report
waiting to happen, while `Wrapper<Int32>.Label must not be blank` is not.

## Discovering implementations across assemblies

There are three doors into assembly scanning, and they differ in ways that matter operationally.
Picking by name alone is how you end up with a start-up crash in production that never reproduced
locally.

| | <xref:Ploch.Common.Reflection.ImplementationTypes> | <xref:Ploch.Common.Reflection.AssemblyTypes> | <xref:Ploch.Common.Reflection.TypeLoader> |
|---|---|---|---|
| Scope | one assembly | many assemblies, or the whole `AppDomain` | many assemblies, added incrementally |
| Filters | one base type | one base type | several base types, plus assembly-name and type-name globs |
| Execution | deferred (LINQ) | deferred (LINQ) | eager, results accumulated in a set |
| Unloadable types | **throws `ReflectionTypeLoadException`** | **recovers — returns what loaded** | **throws `ReflectionTypeLoadException`** |
| Default | concrete only | concrete only (`concreteOnly: true`) | concrete only |

### The `ReflectionTypeLoadException` problem

`Assembly.GetTypes()` throws `ReflectionTypeLoadException` when *any* type in the assembly cannot be
resolved — a missing optional dependency, a plugin built against an older contract, a dynamic proxy
assembly. It is an all-or-nothing call: one bad type and you get nothing, even though the other
ninety-nine loaded perfectly.

This is not hypothetical. Given an assembly `DepB.dll` containing `DerivedThing : BaseThing`,
`Standalone`, `IPluginMarker` and `GoodPlugin`, deployed *without* the `DepA.dll` that defines
`BaseThing`, the three doors behave differently:

```csharp
var broken = Assembly.LoadFrom(pluginPath);

broken.GetTypes();
// ReflectionTypeLoadException: Unable to load one or more of the requested types.

TypeLoader.Configure(c => c.WithBaseType<object>()).LoadTypes(broken);
// ReflectionTypeLoadException: Unable to load one or more of the requested types.

broken.GetTypesImplementing(typeof(object)).ToList();
// ReflectionTypeLoadException: Unable to load one or more of the requested types.

AssemblyTypes.GetImplementations(typeof(object), true, new[] { broken }).ToList();
// Standalone, GoodPlugin
```

Only `AssemblyTypes` survives, because it is the one that catches `ReflectionTypeLoadException` and
falls back to the non-null entries of `ReflectionTypeLoadException.Types`. It quietly drops
`DerivedThing`, which is the correct trade for a plugin host — a plugin you cannot load should not
take down the ones you can.

**If you scan assemblies you do not own, use `AssemblyTypes`.** If you use `TypeLoader` for its
filtering, and the assemblies come from a plugin directory rather than from your own build, wrap the
call:

```csharp
public static TypeLoader LoadTypesSafely(this TypeLoader loader, Assembly assembly)
{
    try
    {
        return loader.LoadTypes(assembly);
    }
    catch (ReflectionTypeLoadException ex)
    {
        logger.LogWarning(ex,
                          "Skipping {Assembly}: {Count} type(s) failed to load",
                          assembly.GetName().Name,
                          ex.LoaderExceptions.Length);

        return loader;
    }
}
```

Note what this wrapper cannot do: `TypeLoader` has no way to feed it the partial type list, so a
single bad type costs you the whole assembly. Recovering the good types means scanning that assembly
through `AssemblyTypes` instead.

### `ImplementationTypes` — one assembly, no ceremony

<xref:Ploch.Common.Reflection.ImplementationTypes> is the smallest of the three and the right choice
when the assembly is one of your own, so `ReflectionTypeLoadException` is not a realistic concern:

```csharp
// Every concrete IExporter in the assembly that declares CsvExporter.
foreach (var exporter in typeof(CsvExporter).GetTypesImplementing(typeof(IExporter)))
{
    services.AddTransient(typeof(IExporter), exporter);
}

// Or from the assembly directly, including abstract bases.
var all = typeof(CsvExporter).Assembly.GetTypesImplementing<IExporter>(includeAbstract: true);
```

`includeAbstract` defaults to `false`, and is passed straight through to `IsImplementing` as
`concreteOnly: !includeAbstract` — so, per the earlier warning, the default also excludes struct
implementations.

### `AssemblyTypes` — many assemblies, resilient, and deferred

```csharp
var handlers = AssemblyTypes.GetImplementations<IMessageHandler>(
    concreteOnly: true,
    typeof(OrderModule).Assembly,
    typeof(BillingModule).Assembly);
```

There is also an extension form that reads better when you already hold a collection, and an
`AppDomain`-wide form:

```csharp
IEnumerable<Assembly> assemblies = LoadPluginAssemblies();

var handlers = assemblies.GetImplementations<IMessageHandler>();
var everywhere = AssemblyTypes.GetAppDomainImplementations<IMessageHandler>();
```

Two properties to keep in mind.

**The result is deferred.** Every overload is a LINQ pipeline, so nothing is scanned until you
enumerate:

```csharp
var query = AssemblyTypes.GetImplementations<IExporter>(true, assemblies);
// nothing has been scanned yet — this is a SelectManySingleSelectorIterator

var count = query.Count();   // the scan happens here
```

Enumerate it twice and you scan twice. Materialise with `ToList()` if the result is used more than
once — which, for a registration list, it usually is.

**`GetAppDomainImplementations` scans everything currently loaded.** In a typical ASP.NET Core host
that is well over a hundred assemblies including the whole framework, and every one of them gets a
full `GetTypes()`. It is fine once at start-up; it is not fine per request. Prefer naming the
assemblies you actually care about.

One compilation detail: the assembly-taking parameters are declared `params IEnumerable<Assembly>`,
which is a **C# 13** params-collection. Callers compiling against C# 12 or earlier cannot use the
expanded form and must pass a collection explicitly:

```csharp
// C# 13+ only:
AssemblyTypes.GetImplementations<IExporter>(true, assemblyA, assemblyB);

// Works on any language version:
AssemblyTypes.GetImplementations<IExporter>(true, new[] { assemblyA, assemblyB });
```

On C# 12 the expanded form fails to compile with
`CS1503: Argument 2: cannot convert from 'System.Reflection.Assembly' to 'params System.Collections.Generic.IEnumerable<System.Reflection.Assembly>'`,
which is an unhelpful message for what is really a language-version problem.

### `TypeLoader` — filtered scanning

<xref:Ploch.Common.Reflection.TypeLoader> is the one to reach for when "every implementation of
`IHandler<>`" is too blunt and you need to constrain the search by assembly name or type name as
well. It is configured once and then fed assemblies:

```csharp
var loader = TypeLoader
    .Configure(c => c.WithBaseTypes(typeof(IMessageHandler))
                     .WithAssemblyGlob(m => m.AddInclude("Contoso.*").AddExclude("*.Tests"))
                     .WithTypeNameGlob(m => m.AddInclude("**/*Handler")))
    .LoadTypes<OrderModule>()
    .LoadTypes<BillingModule>();

foreach (var handler in loader.LoadedTypes)
{
    services.AddScoped(typeof(IMessageHandler), handler);
}
```

A single contract is configured here on purpose. Because multiple base types are OR'd (below),
`WithBaseTypes(typeof(IMessageHandler), typeof(IHandler<>))` would put implementations of *either*
interface into `LoadedTypes`, and the blanket `AddScoped(typeof(IMessageHandler), handler)` above
would then register types that do not implement `IMessageHandler` — producing descriptors that only
fail when something resolves them. With more than one contract, register each type against the
interface it actually implements.

The details that are not obvious from the fluent API:

- **Multiple base types are OR, not AND.** A type is kept if it implements *any* of the configured
  base types. Configuring `WithBaseTypes(typeof(IExporter), typeof(IHandler<>))` over an assembly
  containing `CsvExporter`, `JsonExporter`, `IntHandler`, `DerivedHandler` and `GenericHandler<>`
  yields all five, not their intersection. `WithBaseType<T>()` and `WithBaseTypes(…)` accumulate
  across calls rather than replacing.
- **Abstract types, interfaces and structs are excluded unless you say otherwise.** Add
  `IncludeAbstractTypes()` to keep them.
- **No filters at all means every type in the assembly** — including compiler-generated display
  classes and iterator state machines. `TypeLoader.Configure(c => { }).LoadTypes<Program>()` returned
  every one of the 30 types `Assembly.GetTypes()` reports for a scratch assembly that declares 24.
- **The type-name glob matches `Type.FullName`, and it is `StringComparison.Ordinal`.** The matcher
  is `Microsoft.Extensions.FileSystemGlobbing.Matcher`, so it thinks in paths: `*Csv*`,
  `Contoso.Exporters.*` and `**/*Exporter` all work, but the match is case-sensitive. `*csv*`
  matched nothing where `*Csv*` matched `CsvExporter`. Glob patterns lifted from a case-insensitive
  file matcher will silently return an empty set.
- **The assembly glob matches the assembly's simple name**, not its path or full name. A pattern
  that matches nothing is not an error — the assembly is skipped and `LoadedTypes` stays empty, which
  is easy to mistake for "there are no handlers".
- **Results are a set.** Loading the same assembly twice does not duplicate entries.
- **`LoadTypes(params Type[])` rejects an empty array** with
  `ArgumentException: Argument cannot be null or empty. (Parameter 'assemblyTypes')`. If your
  assembly list is built dynamically, guard for empty before calling.

`TypeLoader` is eager: `LoadedTypes` is a materialised `HashSet<Type>` populated as each `LoadTypes`
call runs. That makes it a start-up tool. It also means a `ReflectionTypeLoadException` from one
assembly aborts that call and leaves earlier results intact.

### `AssemblyListBuilder` and `GetAssemblyDirectory`

<xref:Ploch.Common.Reflection.AssemblyListBuilder> exists to make the assembly list itself readable
when it is assembled from several kinds of anchor. It de-duplicates, so naming two types from the
same assembly is harmless:

```csharp
IEnumerable<Assembly> assemblies = new AssemblyListBuilder()
    .AddFromType<OrderModule>()
    .AddFromType<BillingModule>()
    .AddFromObject(currentPlugin)
    .AddAssemblies(Directory.GetFiles(pluginDirectory, "*.dll").Select(Assembly.LoadFrom))
    .Build();

var handlers = assemblies.GetImplementations<IMessageHandler>().ToList();
```

<xref:Ploch.Common.Reflection.AssemblyExtensions.GetAssemblyDirectory(System.Reflection.Assembly)>
answers "where is this assembly on disk?", which is how you locate a configuration file or a plugin
folder that ships alongside a library rather than alongside the entry point:

```csharp
var directory = typeof(ReportRenderer).Assembly.GetAssemblyDirectory()
                ?? throw new InvalidOperationException("Assembly has no on-disk location.");

var templates = Path.Combine(directory, "Templates");
```

**It returns `null` for an assembly with no on-disk location** — one loaded from a byte array, and
in single-file publishes. Verified: `Assembly.Load(bytes).GetAssemblyDirectory()` returns `null`
because `Assembly.Location` is the empty string, not `null`, so the internal
`?? throw new InvalidOperationException` never fires and `Path.GetDirectoryName("")` returns `null`
instead. Treat the return as nullable and have a fallback; do not rely on the exception.

## Reading and writing members by name

This is the group you reach for when the member name is data — a sort field from a query string, a
column name from a CSV header, a field list from a saved report definition.

### Reading a property

```csharp
object? value = order.GetPropertyValue("Reference");
decimal total = order.GetPropertyValue<Order, decimal>("Total");
```

The failure modes are named, which is the point — every one of these is a `PropertyAccessException`
carrying a `PropertyName`, so a single `catch` can turn any of them into a 400 response naming the
offending field:

| Situation | Exception | Message |
|-----------|-----------|---------|
| No such property (including wrong case) | `PropertyNotFoundException` | `Property Nope was not found.` |
| Property has no getter | `PropertyWriteOnlyException` | `Property Secret is write-only.` |
| Indexer read with no index supplied | `PropertyIndexerMismatchException` | `Index parameters are required for indexed properties.` |
| Index of the wrong type | `PropertyIndexerMismatchException` | `Argument 0 is not of the expected type System.Int32` |

```csharp
public static object? ReadField(Order order, string field)
{
    try
    {
        return order.GetPropertyValue(field);
    }
    catch (PropertyAccessException ex)
    {
        throw new ArgumentException($"Unknown or unreadable field '{ex.PropertyName}'.", nameof(field));
    }
}
```

Property lookup is **case-sensitive**: `order.GetPropertyValue("reference")` throws
`PropertyNotFoundException` even though `Reference` exists. If your input is user-supplied, resolve
the name against a whitelist first rather than passing it through — which is exactly the
`SortableFields<T>` pattern from the
[expressions article](./expressions-and-owned-properties.md).

Two leaks worth knowing about:

- The typed overload `GetPropertyValue<T, TValue>` performs a plain cast, so a wrong `TValue`
  surfaces as `InvalidCastException: Unable to cast object of type 'System.Decimal' to type
  'System.Int32'` — not as a `PropertyAccessException`. Catch both, or check the property type first.
- Passing an index for a *non*-indexed property escapes the validator entirely and throws
  `IndexOutOfRangeException: Index was outside the bounds of the array.` from inside the validation
  loop. Only pass `index` when you know the property is an indexer.

### Indexers

Indexers are properties named `Item`, and
<xref:Ploch.Common.Reflection.PropertyHelpers.IndexerPropertyName> exists so you do not have to
hard-code that string:

```csharp
var second = basket.GetPropertyValue(PropertyHelpers.IndexerPropertyName, new object?[] { 1 });
```

<xref:Ploch.Common.Reflection.MemberInfoExtensions.IsIndexer(System.Reflection.MemberInfo)> and
`IsNonIndexerReadProperty` are the predicates for filtering them out of a member walk.

### `SetPropertyValue` reads the *static* type — the sharpest edge here

`GetPropertyValue` resolves the property against `obj.GetType()`. `SetPropertyValue` resolves it
against `typeof(T)` — the *compile-time* type of the argument. When the two differ, reads work and
writes fail:

```csharp
var customer = new Customer { Name = "Ada" };
object boxed = customer;

boxed.GetPropertyValue("Name");        // "Ada"
boxed.SetPropertyValue("Name", "Grace");
// PropertyNotFoundException: Property Name was not found.

customer.SetPropertyValue("Name", "Grace");   // works — T is Customer
```

The same happens whenever the variable is typed as an interface, a base class, or `dynamic`-free
`object` — which is precisely the situation reflective code is usually in. Keep the concrete type in
the variable, or resolve the `PropertyInfo` yourself and call `SetValue` on it.

### Enumerating properties

<xref:Ploch.Common.Reflection.PropertyHelpers.GetPropertyValues(System.Object)> flattens an object
into name/value pairs — the shape you want for an audit record or a structured log entry:

```csharp
foreach (var (name, value) in order.GetPropertyValues())
{
    audit.Add(name, value);
}
```

For `new Customer { Id = 3, Name = "Bob" }` the pairs are `Id=3` and `Name=Bob`.

It skips indexers, so a type with an `Item` property yields only its ordinary properties. It does
**not** skip write-only properties, and reading one throws
`ArgumentException: Property Get method was not found.` from `PropertyInfo.GetValue` — an exception
from outside the `PropertyAccessException` family, so it will not be caught by the handler above.
Filter first if the type might have a setter-only property:

```csharp
var readable = order.GetType()
                    .GetProperties()
                    .Where(p => p.IsNonIndexerReadProperty())
                    .Select(p => (p.Name, Value: p.GetValue(order)));
```

`GetProperties<TPropertyType>()` filters by property *type*, which is how you find "every `string`
property on this entity" for a bulk trim or redact pass:

```csharp
foreach (var property in entity.GetProperties<string>())
{
    property.SetValue(entity, ((string?)property.GetValue(entity))?.Trim());
}
```

Its `includeAssignableOrInheritedTypes` parameter defaults to `true`, meaning assignable types are
included — so `GetProperties<object>()` returns *every* property. Passing `false` demands an exact
type match, and `GetProperties<object>(false)` therefore returns nothing at all on a type with no
literally-`object`-typed property. The filter is type-based only: a write-only `string` property is
included, so check `CanRead` before reading.

### Static members

```csharp
typeof(Settings).GetStaticPropertyValue("Region");            // "eu-west"
typeof(Settings).GetStaticPropertyValue<string>("Region");    // "eu-west"

typeof(Settings).TryGetStaticPropertyValue("Region", out var region);   // True
```

`TryGetStaticPropertyValue` is `BindingFlags.Static | BindingFlags.Public` — **public statics only**.
A private static property returns `false` with a `null` value rather than throwing, which is easy to
misread as "the property is null".

The throwing overloads report through `InvalidOperationException`, not the `PropertyAccessException`
family:

```text
Static property Missing was not found in Contoso.Settings
Static property Region in Contoso.Settings is not of System.Int32 type
```

<xref:Ploch.Common.Reflection.TypeHelper.GetStaticFieldValues``1(System.Reflection.BindingFlags)>
does the same for fields, and is genuinely useful for turning a class of `public const`/
`static readonly` values into a lookup — permission names, feature-flag keys, well-known
identifiers:

```csharp
IDictionary<string, object?> knownRoles = TypeHelper.GetStaticFieldValues<Roles>();
```

Its default is `BindingFlags.Public`, to which `BindingFlags.Static` is added internally, so you get
public static fields. Pass `BindingFlags.NonPublic` for private ones — and be aware that this also
surfaces auto-property backing fields under their mangled names (`<Region>k__BackingField`).

One structural limitation: `GetStaticFieldValues<TType>` takes its target as a *type parameter*, and
C# forbids a static class as a type argument. A constants class declared `static` cannot be passed
at all — `CS0718: 'Roles': static types cannot be used as type arguments`. Declare such a holder as
a non-static `sealed` class if you intend to read it reflectively.

### Fields, including private ones

<xref:Ploch.Common.Reflection.ObjectReflectionExtensions.GetFieldValue(System.Object,System.String)>
searches public, non-public, instance and static fields in one call, and returns `null` when the
field does not exist rather than throwing:

```csharp
var retries = handler.GetFieldValue<int>("_retryCount");
```

This is a test-assertion and diagnostics tool. Reaching into another type's private state from
production code couples you to its implementation; use it to assert that a retry counter advanced,
not to drive behaviour.

`GetFieldValues` and `GetMemberValues` take `BindingFlags` and return dictionaries. Their defaults
are `Instance | Public`, which for a class whose state lives in auto-properties returns **nothing**,
because backing fields are private:

```csharp
invoice.GetFieldValues();
// empty

invoice.GetFieldValues(BindingFlags.Instance | BindingFlags.NonPublic);
// _field=42, <Number>k__BackingField=INV-2, <Total>k__BackingField=1
```

`GetMemberValues` covers fields *and* properties, and walks the inheritance chain so members
declared on a base class are included alongside the derived ones. Indexers are handled specially:
rather than being invoked, the entry's value is the `PropertyInfo` itself, so a caller that wants
indexed values can supply the index and read it.

## Walking and comparing object graphs

The remaining four types are the ones to approach with a clear head. Each solves a real problem, and
each has a boundary you have to stay inside.

### `ExecuteOnProperties`

<xref:Ploch.Common.Reflection.ObjectGraphHelper.ExecuteOnProperties``1(System.Object,System.Action{``0})>
walks an object and everything reachable from it, invoking an action on each value whose type
matches. The intended use is a sweep over an ORM-shaped aggregate — stamping identifiers or audit
fields across a root and its children in one line:

```csharp
order.ExecuteOnProperties<IHasTenantId>(entity => entity.TenantId = currentTenant);
```

Within that shape it works well, and it is what the library's own tests exercise. Outside it, three
behaviours will surprise you, and all three show up in the sequence of values the untyped overload
passes to the action for a two-level object
`new Order { Reference = "AB", Line = new Line { Sku = "X" } }`:

```text
Order  "AB"  'A'  'B'  Line  Line  "X"  'X'
```

- **Non-enumerable property values are visited twice.** `Line` appears twice: once as a property
  value, and once again as the root of its own recursive walk. An action that increments a counter,
  appends to a list or applies a delta will apply it twice. Idempotent actions — assigning a value —
  are unaffected, which is why the tests do not catch it.
- **Strings are walked as sequences.** `string` is `IEnumerable<char>`, and the walker treats it as
  a collection, so the action fires once per character. The typed
  `ExecuteOnProperties<TPropertyType>` overload hides this behind an `is TPropertyType` test, but
  the untyped overload sees every `char`, and the cost is proportional to total text length.
- **Cyclic graphs do not terminate.** A `visited` set is allocated and populated but never consulted
  before recursing. Two objects referring to each other recurse until the stack is exhausted: a
  bounded probe that threw after 5,000 visits confirmed the walk was still going. Bidirectional
  navigation properties — `Order.Lines` where each `Line` has an `Order` back-reference, the default
  shape in most EF Core models — are exactly this case.

So: use it on trees, not on graphs, and prefer the typed overload. If the model has back-references,
write the walk by hand with your own visited set.

### Comparing by value

<xref:Ploch.Common.Reflection.ByValueObjectComparator.AreEqual(System.Object,System.Object,System.Type)>
compares two objects property by property, recursing into anything `IsSimpleType` says is not simple,
and <xref:Ploch.Common.Reflection.ByValueObjectComparer`1> wraps it as an `IEqualityComparer<T>` so
it can be handed to `Distinct`, `GroupBy` or a `HashSet<T>`.

For flat DTOs — the case it is built for — it does exactly what you want, and saves writing an
`Equals` override on a type that has no business having one:

```csharp
var a = new Customer { Id = 1, Name = "Ada" };
var b = new Customer { Id = 1, Name = "Ada" };

a.Equals(b);                          // False — reference equality
ByValueObjectComparator.AreEqual(a, b); // True
```

Beyond flat DTOs there are three hard limits, and they compound.

**Collections are compared by their own properties, not by their contents.** A `List<string>` is not
a simple type, so the comparator recurses into it — and what it finds are `Count` and `Capacity`. Two
baskets with completely different items compare as equal:

```csharp
var b1 = new Basket { Items = new List<string> { "a", "b", "c" } };
var b2 = new Basket { Items = new List<string> { "x", "y", "z" } };

ByValueObjectComparator.AreEqual(b1, b2);   // True  <- same Count, same Capacity

var b3 = new Basket { Items = new List<string> { "a", "b" } };
ByValueObjectComparator.AreEqual(b1, b3);   // False <- different Count
```

**That makes `ByValueObjectComparer<T>` unsafe as a hash-based comparer for collection-bearing
types.** `GetHashCode` delegates to <xref:Ploch.Common.Reflection.ObjectHashCodeBuilder>, which
*does* enumerate sequences. So for `b1` and `b2` above, `Equals` returns `true` while the two hash
codes differ — a direct violation of the `IEqualityComparer<T>` contract. Dropped into a
`HashSet<Basket>` or a `GroupBy`, the pair lands in different buckets and is never compared, so the
duplicate is silently kept. For a type whose properties are all simple, `Equals` and `GetHashCode`
agree and the comparer is sound.

> [!WARNING]
> **Cyclic graphs cause a `StackOverflowException`.** `AreEqual` has no cycle detection at all, and
> unlike the two problems above this one is not recoverable — a `StackOverflowException` cannot be
> caught and terminates the process. Never point it at an entity graph with back-references.

Use it for flat, cycle-free DTOs whose collections you do not need compared. For anything richer,
write `Equals` or use a structural-comparison library.

### `ObjectHashCodeBuilder`

<xref:Ploch.Common.Reflection.ObjectHashCodeBuilder.GetHashCode(System.Object)> is the sturdiest
member of this group, and can be used on its own to derive a best-effort, in-process cache key from
the shape of a request object:

```csharp
var cacheKey = ObjectHashCodeBuilder.GetHashCode(query);
```

Verified behaviour:

- `null` hashes to `0`.
- Two distinct instances with equal property values hash equally, and changing a value will normally
  change the hash — but **this is a 32-bit `int`, not a content address.** Simple values contribute
  `object.GetHashCode()` and results are combined with unchecked arithmetic, so it is not
  collision-resistant: distinct request shapes can share a key. Use it to *find* a candidate entry,
  then confirm with an equality check on the request itself; never as the sole identity of a cached
  value, and never persisted or shared across processes (string hashing is randomised per process).
- Sequences contribute their **elements**, so two lists with different contents hash differently.
- **Cycles are handled.** A reference-identity `visited` set short-circuits a repeat visit with a
  fixed constant, so a self-referencing node hashes without recursing forever.
- **A throwing property getter is swallowed.** The property's name still contributes to the hash;
  its value does not. Hashing a half-initialised object will not blow up.

Two caveats. Properties are ordered by name with `StringComparer.Ordinal`, so the hash is stable
within a process, but it is built on `object.GetHashCode()` for simple values — and `string`'s hash
code is randomised per process by default on .NET Core. **Do not persist these hashes or compare
them across processes.** And the whole computation is reflective: fine for a cache key computed once
per request, wrong inside a loop over a large result set.

## Cost and caching

Everything here goes through `System.Reflection`. Member lookup by name is a metadata search;
`GetValue`/`SetValue` are indirect invocations; `Assembly.GetTypes()` materialises every type in the
assembly.

The practical rules:

- **Scan once, at start-up.** `TypeLoader`, `AssemblyTypes` and `ImplementationTypes` belong in
  composition-root code, and their results belong in a field. `GetAppDomainImplementations` in
  particular touches every loaded assembly.
- **Materialise deferred queries you use twice.** `AssemblyTypes` returns a lazy pipeline; a second
  enumeration is a second full scan.
- **Cache `PropertyInfo`, not just the result.** If the same property name is read for every row of a
  result set, resolve it once with `GetPropertyInfo` and call the `GetPropertyValue(obj, propertyInfo)`
  overload per row — it skips the name lookup.
- **Prefer a lambda when the member is known at compile time.** See
  [Expressions and owned property info](./expressions-and-owned-properties.md); a `nameof` or a
  cached selector beats a string lookup on both safety and speed.

## Quick reference

| I want to… | Use |
|------------|-----|
| Test whether a type implements an interface, including an open generic one | `type.IsImplementing(typeof(IHandler<>))` |
| The same, restricted to instantiable classes | `type.IsConcreteImplementation<IHandler>()` |
| Decide whether a value can be rendered inline | `type.IsSimpleType()` |
| Render a type name for a message or log | `type.GetReadableTypeName()` |
| Find implementations in one assembly I own | `assembly.GetTypesImplementing<T>()` |
| Find implementations across assemblies I do not control | `AssemblyTypes.GetImplementations<T>(true, assemblies)` |
| Find implementations filtered by assembly or type name | `TypeLoader.Configure(…).LoadTypes(…)` |
| Build an assembly list from mixed anchors | `new AssemblyListBuilder().AddFromType<T>()…Build()` |
| Read a property whose name is data | `obj.GetPropertyValue(name)` |
| Flatten an object into name/value pairs | `obj.GetPropertyValues()` |
| Read a private field in a test | `obj.GetFieldValue<int>("_count")` |
| Read a class of static constants into a dictionary | `TypeHelper.GetStaticFieldValues<T>()` |
| Stamp a value across an aggregate | `root.ExecuteOnProperties<IHasTenantId>(…)` (trees only) |
| Compare two flat DTOs by content | `ByValueObjectComparator.AreEqual(a, b)` |
| Derive an in-process cache key from an object's shape | `ObjectHashCodeBuilder.GetHashCode(obj)` |

## See also

- <xref:Ploch.Common.Reflection> — the full API reference for the namespace.
- [Expressions and owned property info](./expressions-and-owned-properties.md) — the compile-time-safe
  alternative to string-keyed member access.
- [Ploch.Common library guide](../../docs/libraries/common.md) — the other namespaces in the package.
